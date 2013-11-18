
namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 通道连接事件类型
  /// </summary>
  public enum ChannelConnectionEventType
  {
    /// <summary>
    /// 通道已连接事件
    /// </summary>
    Connected,
    /// <summary>
    /// 通道已重新连接事件
    /// </summary>
    Reconnected,
    /// <summary>
    /// 通道已断开连接事件
    /// </summary>
    Disconnected,
    /// <summary>
    /// 通道连接异常事件
    /// </summary>
    ExceptionRaised
  }
}
