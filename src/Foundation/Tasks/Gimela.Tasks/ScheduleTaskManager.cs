using System;
using System.Threading;
using Gimela.Infrastructure.Patterns;

namespace Gimela.Tasks
{
  /// <summary>
  /// 计划任务管理器
  /// </summary>
  public sealed class ScheduleTaskManager : IDisposable
  {
    #region Variables

    private SmartQueue<IScheduleTask> _queue = null;
    private static readonly object _singletonLocker = new object();
    private static ScheduleTaskManager _singletonInstance = null;

    #endregion

    #region Singleton

    /// <summary>
    /// 计划任务管理器
    /// </summary>
    private ScheduleTaskManager()
    {
      _queue = new SmartQueue<IScheduleTask>("ScheduleTaskManager", PreSchedule, true, 25);
    }

    /// <summary>
    /// 计划任务管理器单例
    /// </summary>
    public static ScheduleTaskManager Instance
    {
      get
      {
        if (_singletonInstance == null)
        {
          lock (_singletonLocker)
          {
            if (_singletonInstance == null)
            {
              _singletonInstance = new ScheduleTaskManager();
            }
          }
        }

        return _singletonInstance;
      }
    }

    #endregion

    #region Events

    /// <summary>
    /// 计划任务已终结事件
    /// </summary>
    public event EventHandler<ScheduleTaskTerminatedEventArgs> ScheduleTaskTerminatedEvent;

    #endregion

    #region Schedule

    /// <summary>
    /// 执行计划任务
    /// </summary>
    /// <param name="scheduleTask">计划任务</param>
    public void Schedule(IScheduleTask scheduleTask)
    {
      if (scheduleTask == null)
        throw new ArgumentNullException("scheduleTask");

      _queue.Enqueue(scheduleTask);
    }

    /// <summary>
    /// 删除计划任务的执行
    /// </summary>
    /// <param name="scheduleTask">计划任务</param>
    public void RemoveSchedule(IScheduleTask scheduleTask)
    {
      if (scheduleTask == null)
        throw new ArgumentNullException("scheduleTask");

      RemoveSchedule(scheduleTask.Id);
    }

    /// <summary>
    /// 删除计划任务的执行
    /// </summary>
    /// <param name="scheduleTaskId">计划任务Id</param>
    public void RemoveSchedule(string scheduleTaskId)
    {
      PerformRemoveSchedule(scheduleTaskId);
    }

    #endregion

    #region Methods

    /// <summary>
    /// 计划任务预执行
    /// </summary>
    /// <param name="state">计划任务</param>
    private void PreSchedule(IScheduleTask state)
    {
      if (state == null) return;
      PerformSchedule(state);
    }

    /// <summary>
    /// 执行计划任务
    /// </summary>
    /// <param name="scheduleTask">计划任务</param>
    private void PerformSchedule(IScheduleTask scheduleTask)
    {
      // 如果之前已经执行该任务则删除
      PerformRemoveSchedule(scheduleTask.Id);

      // 将任务缓存至集合中
      ScheduleTaskCollection.Instance.Add(scheduleTask);

      // 订阅终结事件
      scheduleTask.ScheduleTaskTerminatedEvent += new EventHandler<ScheduleTaskTerminatedEventArgs>(OnScheduleTaskTerminatedEvent);

      // 为Trigger分配线程执行
      scheduleTask.Start();
    }

    /// <summary>
    /// 删除计划任务的执行
    /// </summary>
    /// <param name="scheduleTaskId">计划任务Id</param>
    private void PerformRemoveSchedule(string scheduleTaskId)
    {
      IScheduleTask scheduleTask = null;
      if (ScheduleTaskCollection.Instance.TryFind(scheduleTaskId, out scheduleTask))
      {
        // 取消订阅终结事件
        scheduleTask.ScheduleTaskTerminatedEvent -= new EventHandler<ScheduleTaskTerminatedEventArgs>(OnScheduleTaskTerminatedEvent);

        // 关闭Trigger线程
        scheduleTask.Stop();

        // 将任务从缓存集合中删除
        ScheduleTaskCollection.Instance.Remove(scheduleTaskId);
      }
    }

    /// <summary>
    /// 计划任务终结事件处理器
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="e">The <see cref="Gimela.Tasks.ScheduleTaskTerminatedEventArgs"/> instance containing the event data.</param>
    void OnScheduleTaskTerminatedEvent(object sender, ScheduleTaskTerminatedEventArgs e)
    {
      if (ScheduleTaskTerminatedEvent != null)
      {
        ScheduleTaskTerminatedEvent(this, e);
      }

      PerformRemoveSchedule(e.ScheduleTaskId);
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
    private void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_queue != null)
        {
          _queue.Dispose();
        }
      }
    }

    #endregion
  }
}
