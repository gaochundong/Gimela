
namespace Gimela.ServiceModel.ChannelManagement.AsynchronousConnections
{
  /// <summary>
  /// 异步服务连接包装器接口
  /// </summary>
  public interface IAsynchronousConnectionWrapper
  {
    /// <summary>
    /// 获取指定服务接口类型的服务通道
    /// </summary>
    /// <typeparam name="TInterface">指定服务接口类型</typeparam>
    /// <returns>指定服务接口类型的服务通道</returns>
    TInterface GetChannel<TInterface>() where TInterface : class;

    /// <summary>
    /// 连接指定的服务接口
    /// </summary>
    void Connect();

    /// <summary>
    /// 指定服务接口类型的服务通道是否已连接
    /// </summary>
    bool IsConnected { get; }
  }
}
