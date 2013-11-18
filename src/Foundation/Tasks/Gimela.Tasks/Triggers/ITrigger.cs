using System;

namespace Gimela.Tasks
{
  /// <summary>
  /// 触发器接口
  /// </summary>
  public interface ITrigger
  {
    /// <summary>
    /// 触发器Id
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// 触发器名称
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// 被触发执行的作业
    /// </summary>
    IJob TargetJob { get; set; }

    /// <summary>
    /// 触发器已终结事件
    /// </summary>
    event EventHandler<TriggerTerminatedEventArgs> TriggerTerminatedEvent;

    /// <summary>
    /// 触发器是否已终结
    /// </summary>
    bool IsTerminated { get; }

    /// <summary>
    /// 终结触发器
    /// </summary>
    void Terminate();

    /// <summary>
    /// 运行触发器
    /// </summary>
    void Run();
  }
}
