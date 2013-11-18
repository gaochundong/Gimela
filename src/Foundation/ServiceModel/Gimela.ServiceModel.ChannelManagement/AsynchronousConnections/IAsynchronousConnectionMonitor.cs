
namespace Gimela.ServiceModel.ChannelManagement.AsynchronousConnections
{
  /// <summary>
  /// 异步服务连接状态监听器接口
  /// </summary>
  public interface IAsynchronousConnectionMonitor
  {
    /// <summary>
    /// 监听服务通道连接状态，当通道建立连接时触发。
    /// </summary>
    /// <param name="e">通道状态参数</param>
    void OnConnectionConnected(ChannelConnectedEventArgs e);

    /// <summary>
    /// 监听服务通道连接状态，当通道重新建立连接时触发。
    /// </summary>
    /// <param name="e">通道状态参数</param>
    void OnConnectionReconnected(ChannelConnectedEventArgs e);

    /// <summary>
    /// 监听服务通道连接状态，当通道断开连接时触发。
    /// </summary>
    /// <param name="e">通道状态参数</param>
    void OnConnectionDisconnected(ChannelDisconnectedEventArgs e);

    /// <summary>
    /// 监听服务通道连接状态，当通道发生异常时触发。
    /// </summary>
    /// <param name="e">通道状态参数</param>
    void OnConnectionExceptionRaised(ChannelExceptionRaisedEventArgs e);    
  }
}
