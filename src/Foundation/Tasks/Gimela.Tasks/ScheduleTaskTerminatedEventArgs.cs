using System;

namespace Gimela.Tasks
{
  /// <summary>
  /// 时程任务结束事件参数
  /// </summary>
  public class ScheduleTaskTerminatedEventArgs : EventArgs
  {
    /// <summary>
    /// 时程任务结束事件参数
    /// </summary>
    /// <param name="scheduleTaskId">时程任务ID</param>
    public ScheduleTaskTerminatedEventArgs(string scheduleTaskId)
      : base()
    {
      ScheduleTaskId = scheduleTaskId;
    }

    /// <summary>
    /// 时程任务ID
    /// </summary>
    public string ScheduleTaskId { get; private set; }
  }
}
