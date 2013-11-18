using System;
using System.ServiceModel;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 代理通道接口
  /// </summary>
  /// <typeparam name="TChannel">服务通道类型</typeparam>
  public interface IProxyChannel<TChannel> : ICommunicationObject, IDisposable
  {
    /// <summary>
    /// 获取用于发送消息的通道
    /// </summary>
    TChannel Channel { get; }

    /// <summary>
    /// 获取用于发送消息的内部通道
    /// </summary>
    IClientChannel InnerChannel { get; }
  }
}
