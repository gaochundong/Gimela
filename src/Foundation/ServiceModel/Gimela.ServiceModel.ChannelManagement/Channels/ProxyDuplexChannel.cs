using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Gimela.ServiceModel.ChannelManagement.ServiceIdentity;

namespace Gimela.ServiceModel.ChannelManagement.Channels
{
  /// <summary>
  /// 双向代理通道，基于Callback模式
  /// </summary>
  /// <typeparam name="TChannel">服务通道类型</typeparam>
  internal class ProxyDuplexChannel<TChannel> : DuplexClientBase<TChannel>, IProxyChannel<TChannel> where TChannel : class
  {
    #region Ctors

    /// <summary>
    /// 双向代理通道
    /// </summary>
    /// <param name="context">客户端实例上下文</param>
    public ProxyDuplexChannel(InstanceContext context)
      : base(context)
    {
    }

    /// <summary>
    /// 双向代理通道
    /// </summary>
    /// <param name="context">客户端实例上下文</param>
    /// <param name="endpoint">服务通道终结点</param>
    public ProxyDuplexChannel(InstanceContext context, ServiceEndpoint endpoint)
      : base(context, endpoint.Binding, endpoint.Address)
    {
      if (endpoint == null)
        throw new ArgumentNullException("endpoint");

      MessageFixer.Fix(ChannelFactory, endpoint);
    }

    /// <summary>
    /// 双向代理通道
    /// </summary>
    /// <param name="context">客户端实例上下文</param>
    /// <param name="endpoint">服务通道终结点</param>
    /// <param name="messageHeader">定制的消息头</param>
    public ProxyDuplexChannel(InstanceContext context, ServiceEndpoint endpoint, CustomizedMessageHeaderData messageHeader)
      : base(context, endpoint.Binding, endpoint.Address)
    {
      if (endpoint == null)
        throw new ArgumentNullException("endpoint");

      MessageFixer.Fix(ChannelFactory, endpoint, messageHeader);
    }

    /// <summary>
    /// 双向代理通道
    /// </summary>
    /// <param name="context">客户端实例上下文</param>
    /// <param name="binding">终结点绑定方式</param>
    /// <param name="remoteAddress">远端终结点地址</param>
    public ProxyDuplexChannel(InstanceContext context, Binding binding, EndpointAddress remoteAddress)
      : base(context, binding, remoteAddress)
    {
      if (remoteAddress == null)
        throw new ArgumentNullException("remoteAddress");

      MessageFixer.Fix(ChannelFactory);
    }

    /// <summary>
    /// 双向代理通道
    /// </summary>
    /// <param name="context">客户端实例上下文</param>
    /// <param name="binding">终结点绑定方式</param>
    /// <param name="remoteAddress">远端终结点地址</param>
    /// <param name="messageHeader">定制的消息头</param>
    public ProxyDuplexChannel(InstanceContext context, Binding binding, EndpointAddress remoteAddress, CustomizedMessageHeaderData messageHeader)
      : base(context, binding, remoteAddress)
    {
      if (remoteAddress == null)
        throw new ArgumentNullException("remoteAddress");

      MessageFixer.Fix(ChannelFactory, messageHeader);
    }

    #endregion

    #region IProxyChannel<TChannel> Members

    public new TChannel Channel
    {
      get
      {
        return base.Channel;
      }
    }

    public new IClientChannel InnerChannel
    {
      get
      {
        return base.InnerChannel;
      }
    }

    #endregion
  }
}
