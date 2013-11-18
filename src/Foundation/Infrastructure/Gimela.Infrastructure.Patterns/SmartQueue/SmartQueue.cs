using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Gimela.Infrastructure.Patterns
{
  public class SmartQueue<T> : SmartQueueBase, IDisposable
  {
    #region Fields

    private readonly object _engineLock = new object();
    private Thread _engine;
    private Action<T> _action;
    private ConcurrentQueue<T> _queue;
    private ManualResetEventSlim _notifier;
    private SemaphoreSlim _workerCounter = null;

    #endregion

    #region Constructors

    public SmartQueue(string name, Action<T> action)
      : this(name, action, false)
    {
    }

    public SmartQueue(string name, Action<T> action, bool isQueueReclaimed)
      : this(name, action, isQueueReclaimed, DefaultMaxWorkerCount)
    {
    }

    public SmartQueue(string name, Action<T> action, bool isQueueReclaimed, int maxWorkerCount)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("name");
      if (action == null)
        throw new ArgumentNullException("action");
      if (maxWorkerCount <= 0)
        throw new ArgumentException("The value of max worker count is not available.");

      _queue = new ConcurrentQueue<T>();
      _notifier = new ManualResetEventSlim(false);
      Name = name;
      _action = action;
      IsQueueReclaimed = isQueueReclaimed;
      IsActionAsyncMode = false;
      UseThreadPool = false;
      MaxWorkerCount = maxWorkerCount > DefaultMaxWorkerCount ? DefaultMaxWorkerCount : maxWorkerCount;
      _workerCounter = new SemaphoreSlim(MaxWorkerCount, MaxWorkerCount);
    }

    #endregion

    #region Properties

    public string Name { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsActionAsyncMode { get; set; }
    public bool IsQueueReclaimed { get; private set; }
    public bool UseThreadPool { get; set; }
    public bool IsHoldup { get; private set; }
    public int Length { get { return _queue.Count; } }
    public int MaxWorkerCount { get; private set; }
    public int WorkerCount { get { return MaxWorkerCount - _workerCounter.CurrentCount; } }

    #endregion

    #region Control

    public void Start()
    {
      lock (_engineLock)
      {
        OnStart();
      }
    }

    private void OnStart()
    {
      if (!IsRunning)
      {
        IsRunning = true;
        _notifier.Reset();
        NewEngine();
      }
    }

    public void Stop()
    {
      lock (_engineLock)
      {
        if (IsRunning)
        {
          IsRunning = false;
          _notifier.Reset();
        }
      }
    }

    private void Reclaim()
    {
      lock (_engineLock)
      {
        if (IsRunning)
        {
          if (IsQueueReclaimed && !_queue.IsEmpty)
          {
            // double check queue empty
          }
          else
          {
            IsRunning = false;
            _notifier.Reset();
          }
        }
      }
    }

    #endregion

    #region Enqueue

    public void Enqueue(T item)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      lock (_engineLock)
      {
        if (!IsRunning) // queue is not active
        {
          if (IsQueueReclaimed) // set to reclaim thread
          {
            OnStart();
          }
          else
          {
            throw new InvalidProgramException("This smart queue has not been started.");
          }
        }

        _queue.Enqueue(item);

        if (!IsHoldup) // enqueue but does not process it
        {
          _notifier.Set();
        }
      }
    }

    public void Pause()
    {
      IsHoldup = true;
      _notifier.Reset();
    }

    public void Resume()
    {
      IsHoldup = false;
      if (!_queue.IsEmpty)
      {
        _notifier.Set();
      }
    }

    #endregion

    #region Run

    private void NewEngine()
    {
      try
      {
        try
        {
          if (_engine != null && _engine.IsAlive)
          {
            _engine.Join(); // wait the old thread was terminated
            _engine.Abort(); // ensure that thread was aborted
          }
        }
        catch (ThreadAbortException) { }

        _engine = new Thread(Run) { Name = this.Name, IsBackground = true };
        _engine.Start(); // create new thread
      }
      catch (ThreadStateException) { }
    }

    private void Run()
    {
      while (IsRunning)
      {
        _notifier.Wait(); // wait a work item
        _notifier.Reset();

        while (!_queue.IsEmpty)
        {
          T item = default(T);
          if (_queue.TryDequeue(out item))
          {
            CanWork(); // check worker count
            BeginWork(item);
          }
          if (IsHoldup) break; // hold up when paused
        }

        if (IsQueueReclaimed && _queue.IsEmpty)
        {
          Reclaim(); // reclaim thread
        }
      }
    }

    private void BeginWork(T item)
    {
      if (IsActionAsyncMode) // asynchronous worker
      {
        if (UseThreadPool)
        {
          // use thread pool task
          Task.Factory.StartNew(() => { Work(item); });
        }
        else
        {
          // control thread creation even its heavy
          new Thread(() => { Work(item); })
          {
            Name = string.Format(CultureInfo.InvariantCulture, "{0}#{1}", this.Name, item.ToString()),
            IsBackground = true
          }.Start();
        }
      }
      else
      {
        Work(item); // synchronous worker
      }
    }

    private void Work(T item)
    {
      try
      {
        _action.Invoke(item);
      }
      finally
      {
        OnWorkDone();
      }
    }

    private void CanWork()
    {
      try
      {
        if (_workerCounter != null)
        {
          _workerCounter.Wait(); // control max thread workers
        }
      }
      catch { } // when you are doing this the counter could has been disposed
    }

    private void OnWorkDone()
    {
      try
      {
        if (_workerCounter != null)
        {
          _workerCounter.Release(); // release the current worker
        }
      }
      catch { } // when you are doing this the counter could has been disposed
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        Stop();
        _notifier.Dispose();
        _notifier = null;
        _workerCounter.Dispose();
        _workerCounter = null;
      }
    }

    #endregion
  }
}
