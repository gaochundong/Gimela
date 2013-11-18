
namespace Gimela.ServiceModel.ChannelManagement.AsynchronousConnections
{
  /// <summary>
  /// 异步服务连接基类，这是一个抽象类。
  /// </summary>
  public abstract class AsynchronousConnectionBase : IAsynchronousConnectionMonitor
  {
    #region Establish Connection

    /// <summary>
    /// 异步服务连接包装器
    /// </summary>
    protected IAsynchronousConnectionWrapper Connection { get; set; }

    /// <summary>
    /// 建立服务连接通道
    /// </summary>
    protected virtual void EstablishConnection()
    {
      // assign value to connection property
    }

    #endregion

    #region IAsynchronousConnectionMonitor Members

    /// <summary>
    /// 监听服务通道连接状态，当通道建立连接时触发。
    /// </summary>
    /// <param name="e">通道状态参数</param>
    public virtual void OnConnectionConnected(ChannelConnectedEventArgs e)
    {
    }

    /// <summary>
    /// 监听服务通道连接状态，当通道重新建立连接时触发。
    /// </summary>
    /// <param name="e">通道状态参数</param>
    public virtual void OnConnectionReconnected(ChannelConnectedEventArgs e)
    {
    }

    /// <summary>
    /// 监听服务通道连接状态，当通道断开连接时触发。
    /// </summary>
    /// <param name="e">通道状态参数</param>
    public virtual void OnConnectionDisconnected(ChannelDisconnectedEventArgs e)
    {
    }

    /// <summary>
    /// 监听服务通道连接状态，当通道发生异常时触发。
    /// </summary>
    /// <param name="e">通道状态参数</param>
    public virtual void OnConnectionExceptionRaised(ChannelExceptionRaisedEventArgs e)
    {
    }

    #endregion
  }
}
