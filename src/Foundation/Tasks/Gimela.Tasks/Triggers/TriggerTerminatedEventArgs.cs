using System;

namespace Gimela.Tasks
{
  /// <summary>
  /// 时程触发器结束事件参数
  /// </summary>
  public class TriggerTerminatedEventArgs : EventArgs
  {
    /// <summary>
    /// 时程触发器结束事件参数
    /// </summary>
    /// <param name="triggerId">触发器ID</param>
    public TriggerTerminatedEventArgs(string triggerId)
      : base()
    {
      TriggerId = triggerId;
    }

    /// <summary>
    /// 触发器ID
    /// </summary>
    public string TriggerId { get; private set; }
  }
}
