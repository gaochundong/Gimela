using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace Gimela.Infrastructure.AsyncModel
{
  /// <summary>
  /// 异步工作器
  /// </summary>
  /// <typeparam name="T">工作器结果数据泛型</typeparam>
  public class AsyncWorker<T> : IDisposable
  {
    #region Fields

    /// <summary>
    /// 异步工作器后台工作的线程
    /// </summary>
    private BackgroundWorker worker;
    /// <summary>
    /// 异步工作器超时时间计时器
    /// </summary>
    private Timer workerTimeoutTimer;
    /// <summary>
    /// 异步工作器锁
    /// </summary>
    private object locker = new object();
    /// <summary>
    /// 异步工作器引用句柄
    /// </summary>
    private AsyncWorkerHandle<T> handle;

    #endregion

    #region Ctors

    /// <summary>
    /// 异步工作器实例
    /// </summary>
    /// <param name="workerMethod">需要异步工作的方法</param>
    /// <param name="workerProgressChangedCallback">异步工作进度上报回调方法</param>
    /// <param name="workerMethodCompletedCallback">异步工作已完成的回调方法</param>
    public AsyncWorker(DoWorkEventHandler workerMethod,
        ProgressChangedEventHandler workerProgressChangedCallback,
        EventHandler<AsyncWorkerCallbackEventArgs<T>> workerMethodCompletedCallback)
    {
      if (workerMethod == null)
        throw new ArgumentNullException("workerMethod");

      WorkerMethod = workerMethod;
      WorkerProgressChangedCallback = workerProgressChangedCallback;
      WorkerMethodCompletedCallback = workerMethodCompletedCallback;

      this.worker = new BackgroundWorker();
      this.worker.WorkerSupportsCancellation = true;
      this.worker.DoWork += new DoWorkEventHandler(WorkerMethod);
      this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompletedAdapter);

      if (WorkerProgressChangedCallback != null)
      {
        this.worker.ProgressChanged += new ProgressChangedEventHandler(WorkerProgressChangedCallback);
        this.worker.WorkerReportsProgress = true;
      }
    }

    #endregion

    #region Properties

    /// <summary>
    /// 需要异步工作的方法
    /// </summary>
    public DoWorkEventHandler WorkerMethod { get; private set; }

    /// <summary>
    /// 异步工作进度上报回调方法
    /// </summary>
    public ProgressChangedEventHandler WorkerProgressChangedCallback { get; private set; }

    /// <summary>
    /// 异步工作已完成的回调方法
    /// </summary>
    public EventHandler<AsyncWorkerCallbackEventArgs<T>> WorkerMethodCompletedCallback { get; private set; }

    /// <summary>
    /// 异步工作器后台工作的线程
    /// </summary>
    public BackgroundWorker Worker
    {
      get
      {
        return this.worker;
      }
    }

    /// <summary>
    /// 异步工作器是否正在工作
    /// </summary>
    public bool IsBusy
    {
      get
      {
        if (this.worker != null)
        {
          return this.worker.IsBusy;
        }

        return false;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 异步工作器开始工作
    /// </summary>
    /// <returns>异步工作器句柄</returns>
    public AsyncWorkerHandle<T> StartWork()
    {
      int timeoutSeconds = 10;
      return StartWork(timeoutSeconds);
    }

    /// <summary>
    /// 异步工作器开始工作
    /// </summary>
    /// <param name="timeoutSeconds">设置工作器超时时长，单位为秒，默认为10秒。工作器超时之后工作将被取消。</param>
    /// <returns>异步工作器句柄</returns>
    public AsyncWorkerHandle<T> StartWork(int timeoutSeconds)
    {
      if (timeoutSeconds < 0)
        throw new ArgumentException("Invalid timeout seconds.", "timeoutSeconds");

      this.worker.RunWorkerAsync();
      SetTimeoutTimer(timeoutSeconds);
      handle = new AsyncWorkerHandle<T>(this);

      return handle;
    }

    /// <summary>
    /// 取消异步工作器工作
    /// </summary>
    public void CancelWork()
    {
      if (this.worker != null && this.worker.IsBusy)
      {
        lock (locker)
        {
          if (this.worker != null && this.worker.IsBusy)
          {
            this.worker.CancelAsync();
          }
        }
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 异步工作已完成的回调方法
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RunWorkerCompletedAdapter(object sender, RunWorkerCompletedEventArgs e)
    {
      T data = default(T);
      if (!e.Cancelled && e.Error == null && e.Result != null)
      {
        data = (T)e.Result; // worker result is our data
      }

      if (this.WorkerMethodCompletedCallback != null)
      {
        AsyncWorkerCallbackEventArgs<T> args = new AsyncWorkerCallbackEventArgs<T>(handle, data, e.Error);
        WorkerMethodCompletedCallback(this, args);
      }

      ResetTimeoutTimer();
    }

    /// <summary>
    /// 设置异步工作方法超时计时器
    /// </summary>
    /// <param name="timeoutSeconds">超时时间</param>
    private void SetTimeoutTimer(int timeoutSeconds)
    {
      if (timeoutSeconds > 0)
      {
        this.workerTimeoutTimer = new Timer(OnTimeoutTimerCallback, this.worker, (timeoutSeconds * 1000), Timeout.Infinite);
      }
    }

    /// <summary>
    /// 异步工作方法超时计时器回调方法
    /// </summary>
    /// <param name="state"></param>
    private void OnTimeoutTimerCallback(object state)
    {
      BackgroundWorker backgroundWorker = state as BackgroundWorker;
      if (backgroundWorker != null)
      {
        backgroundWorker.CancelAsync();
      }

      if (this.WorkerMethodCompletedCallback != null)
      {
        AsyncWorkerCallbackEventArgs<T> args = new AsyncWorkerCallbackEventArgs<T>(
            handle, default(T),
            new TimeoutException(string.Format(CultureInfo.InvariantCulture, 
              @"Asynchronous worker [{0}] is timeout.", this.WorkerMethod.Method.Name)));
        WorkerMethodCompletedCallback(this, args);
      }

      ResetTimeoutTimer();
    }

    /// <summary>
    /// 重置异步工作方法超时计时器
    /// </summary>
    private void ResetTimeoutTimer()
    {
      if (workerTimeoutTimer != null)
      {
        lock (locker)
        {
          if (workerTimeoutTimer != null)
          {
            workerTimeoutTimer.Dispose();
            workerTimeoutTimer = null;
          }
        }
      }
    }

    #endregion

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        lock (locker)
        {
          if (workerTimeoutTimer != null)
          {
            workerTimeoutTimer.Dispose();
            workerTimeoutTimer = null;
          }
          if (worker != null)
          {
            worker.Dispose();
            worker = null;
          }
        }
      }
    }

    #endregion
  }
}
