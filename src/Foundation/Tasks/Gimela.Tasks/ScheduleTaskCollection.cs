using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Gimela.Tasks
{
  /// <summary>
  /// 计划任务集合
  /// </summary>
  internal sealed class ScheduleTaskCollection
  {
    private readonly ObservableCollection<IScheduleTask> _collection = new ObservableCollection<IScheduleTask>();
    private readonly object _lock = new object();
    private static readonly object _singletonLocker = new object();
    private static ScheduleTaskCollection _singletonInstance = null;

    #region Singleton

    /// <summary>
    /// 计划任务集合
    /// </summary>
    private ScheduleTaskCollection()
    {

    }

    /// <summary>
    /// 计划任务集合
    /// </summary>
    public static ScheduleTaskCollection Instance
    {
      get
      {
        if (_singletonInstance == null)
        {
          lock (_singletonLocker)
          {
            if (_singletonInstance == null)
            {
              _singletonInstance = new ScheduleTaskCollection();
            }
          }
        }

        return _singletonInstance;
      }
    }

    #endregion

    /// <summary>
    /// 计划任务集合
    /// </summary>
    public ObservableCollection<IScheduleTask> Collection { get { return this._collection; } }

    /// <summary>
    /// 添加计划任务
    /// </summary>
    /// <param name="scheduleTask">计划任务</param>
    public void Add(IScheduleTask scheduleTask)
    {
      if (scheduleTask == null)
        throw new ArgumentNullException("scheduleTask");

      lock (_lock)
      {
        IScheduleTask find = null;
        if (this.TryFind(scheduleTask.Id, out find))
        {
          throw new ArgumentException(
              string.Format(CultureInfo.InvariantCulture, "ScheduleTask already exists"), "scheduleTask");
        }
      }

      lock (_lock)
      {
        _collection.Add(scheduleTask);
      }
    }

    /// <summary>
    /// 根据计划任务ID删除计划任务
    /// </summary>
    /// <param name="scheduleTaskId">计划任务ID</param>
    public void Remove(string scheduleTaskId)
    {
      lock (_lock)
      {
        IScheduleTask find = null;
        if (this.TryFind(scheduleTaskId, out find))
        {
          _collection.Remove(find);
        }
      }
    }

    /// <summary>
    /// 删除计划任务
    /// </summary>
    /// <param name="scheduleTask">计划任务</param>
    public void Remove(IScheduleTask scheduleTask)
    {
      Remove(scheduleTask.Id);
    }

    /// <summary>
    /// 清除集合
    /// </summary>
    public void Clear()
    {
      lock (_lock)
      {
        _collection.Clear();
      }
    }

    /// <summary>
    /// 尝试根据计划任务ID查找指定的计划任务
    /// </summary>
    /// <param name="scheduleTaskId">计划任务ID</param>
    /// <param name="scheduleTask">指定的计划任务</param>
    /// <returns>指定的计划任务是否存在</returns>
    public bool TryFind(string scheduleTaskId, out IScheduleTask scheduleTask)
    {
      bool result = false;
      scheduleTask = null;

      lock (_lock)
      {
        foreach (var item in _collection)
        {
          if (item.Id == scheduleTaskId)
          {
            scheduleTask = item;
            result = true;
            break;
          }
        }
      }

      return result;
    }
  }
}
