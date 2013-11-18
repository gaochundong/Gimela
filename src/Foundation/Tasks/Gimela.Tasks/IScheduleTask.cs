using System;

namespace Gimela.Tasks
{
  /// <summary>
  /// 计划任务接口
  /// </summary>
  public interface IScheduleTask
  {
    /// <summary>
    /// 计划任务Id
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// 计划任务组
    /// </summary>
    string Group { get; set; }

    /// <summary>
    /// 计划任务已终结事件
    /// </summary>
    event EventHandler<ScheduleTaskTerminatedEventArgs> ScheduleTaskTerminatedEvent;

    /// <summary>
    /// 计划任务是否已终结
    /// </summary>
    bool IsTerminated { get; set; }

    /// <summary>
    /// 开始执行计划任务
    /// </summary>
    void Start();

    /// <summary>
    /// 停止执行计划任务
    /// </summary>
    void Stop();
  }
}
