using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Gimela.Infrastructure.Patterns
{
  public class SmartQueueMapper<TMember, TItem> : ISmartQueueMapper<TMember, TItem>
  {
    #region Ctor

    private readonly Action<TItem> _action;
    private ConcurrentDictionary<TMember, SmartQueue<TItem>> _mapping;
    private System.Threading.Timer _maintainer;
    private static int RecyclePeriodByMinutes = 30;
    private readonly object _syncRoot = new object();

    public SmartQueueMapper(string name, Action<TItem> action)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("name");
      if (action == null)
        throw new ArgumentNullException("action");

      Name = name;
      _action = action;
      _mapping = new ConcurrentDictionary<TMember, SmartQueue<TItem>>();

      TimeSpan recyclePeriod = TimeSpan.FromMinutes(RecyclePeriodByMinutes);
      _maintainer = new System.Threading.Timer((s) => { RecycleMembers(); }, null, TimeSpan.FromMinutes(1), recyclePeriod);
    }

    #endregion

    #region Properties

    public string Name { get; private set; }
    public ReadOnlyCollection<TMember> Members { get { return _mapping.Keys.ToList().AsReadOnly(); } }
    public int MemberCount { get { return _mapping.Count; } }
    public bool IsHoldup { get; private set; }

    public int ActiveMemberCount
    {
      get
      {
        int count = 0;

        foreach (var item in _mapping)
        {
          if (item.Value.IsRunning) count++;
        }

        return count;
      }
    }

    public int ActiveMemberQueueAverageLength
    {
      get
      {
        int count = 0;
        int length = 0;
        int average = 0;

        foreach (var item in _mapping)
        {
          if (item.Value.IsRunning)
          {
            count++;
            length += item.Value.Length;
          }
        }

        if (count > 0)
        {
          average = length / count;
        }

        return average;
      }
    }

    public int ActiveMemberQueueMaxLength
    {
      get
      {
        int max = 0;

        foreach (var item in _mapping)
        {
          if (item.Value.IsRunning)
          {
            int count = item.Value.Length;
            if (count > max)
            {
              max = count;
            }
          }
        }

        return max;
      }
    }

    public int ActiveMemberQueueTotalLength
    {
      get
      {
        int total = 0;

        foreach (var item in _mapping)
        {
          if (item.Value.IsRunning)
          {
            total += item.Value.Length;
          }
        }

        return total;
      }
    }

    public int TotalWorkerCount
    {
      get
      {
        int count = 0;

        foreach (var item in _mapping)
        {
          count += item.Value.WorkerCount;
        }

        return count;
      }
    }

    #endregion

    #region Enqueue

    public void Enqueue(TMember member, TItem item)
    {
      lock (_syncRoot)
      {
        if (!_mapping.ContainsKey(member))
        {
          string queueName = string.Format(CultureInfo.InvariantCulture, "{0}#{1}", Name, member.ToString());
          var queue = new SmartQueue<TItem>(queueName, _action, true) { IsActionAsyncMode = false };
          _mapping.Add(member, queue); // default queue reclaimed
          if (IsHoldup)
          {
            queue.Pause();
          }
        }

        _mapping[member].Enqueue(item);
      }
    }

    public void Remove(TMember member)
    {
      lock (_syncRoot)
      {
        if (_mapping.ContainsKey(member))
        {
          SmartQueue<TItem> queue = _mapping[member];
          _mapping.Remove(member);
          queue.Dispose();
        }
      }
    }

    public ISmartQueueMapper<TMember, TItem> Pause()
    {
      IsHoldup = true;
      lock (_syncRoot)
      {
        _mapping.Values.ToList().ForEach(m => { m.Pause(); });
      }
      return this;
    }

    public ISmartQueueMapper<TMember, TItem> Resume()
    {
      IsHoldup = false;
      lock (_syncRoot)
      {
        _mapping.Values.ToList().ForEach(m => { m.Resume(); });
      }
      return this;
    }

    #endregion

    #region Recycle

    public void RecycleMembers()
    {
      List<TMember> wouldBeRemovedMembers = new List<TMember>();
      foreach (var item in _mapping)
      {
        if (!item.Value.IsRunning && item.Value.Length <= 0)
        {
          wouldBeRemovedMembers.Add(item.Key);
        }
      }

      lock (_syncRoot)
      {
        foreach (var member in wouldBeRemovedMembers)
        {
          SmartQueue<TItem> queue = _mapping[member];
          if (!queue.IsRunning && queue.Length <= 0)
          {
            _mapping.Remove(member);
            queue.Dispose();
          }
        }
      }
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
        if (_maintainer != null)
        {
          _maintainer.Change(Timeout.Infinite, Timeout.Infinite);
          _maintainer.Dispose();
          _maintainer = null;
        }
        foreach (var map in _mapping)
        {
          map.Value.Dispose();
        }
        _mapping.Clear();
      }
    }

    #endregion
  }
}
