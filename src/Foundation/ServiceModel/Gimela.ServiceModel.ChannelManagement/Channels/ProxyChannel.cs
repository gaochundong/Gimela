using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Gimela.ServiceModel.ChannelManagement.ServiceIdentity;

namespace Gimela.ServiceModel.ChannelManagement.Channels
{
  /// <summary>
  /// 单向代理通道
  /// </summary>
  /// <typeparam name="TChannel">服务通道类型</typeparam>
  internal class ProxyChannel<TChannel> : ClientBase<TChannel>, IProxyChannel<TChannel> where TChannel : class
  {
    #region Ctors

    /// <summary>
    /// 单向代理通道
    /// </summary>
    public ProxyChannel()
      : base()
    {
    }

    /// <summary>
    /// 单向代理通道
    /// </summary>
    /// <param name="endpoint">服务通道终结点</param>
    public ProxyChannel(ServiceEndpoint endpoint)
      : base(endpoint.Binding, endpoint.Address)
    {
      if (endpoint == null)
        throw new ArgumentNullException("endpoint");

      MessageFixer.Fix(ChannelFactory, endpoint);
    }

    /// <summary>
    /// 单向代理通道
    /// </summary>
    /// <param name="endpoint">服务通道终结点</param>
    /// <param name="messageHeader">定制的消息头</param>
    public ProxyChannel(ServiceEndpoint endpoint, CustomizedMessageHeaderData messageHeader)
      : base(endpoint.Binding, endpoint.Address)
    {
      if (endpoint == null)
        throw new ArgumentNullException("endpoint");

      MessageFixer.Fix(ChannelFactory, endpoint, messageHeader);
    }

    /// <summary>
    /// 单向代理通道
    /// </summary>
    /// <param name="binding">终结点绑定方式</param>
    /// <param name="remoteAddress">远端终结点地址</param>
    public ProxyChannel(Binding binding, EndpointAddress remoteAddress)
      : base(binding, remoteAddress)
    {
      if (remoteAddress == null)
        throw new ArgumentNullException("remoteAddress");

      MessageFixer.Fix(ChannelFactory);
    }

    /// <summary>
    /// 单向代理通道
    /// </summary>
    /// <param name="binding">终结点绑定方式</param>
    /// <param name="remoteAddress">远端终结点地址</param>
    /// <param name="messageHeader">定制的消息头</param>
    public ProxyChannel(Binding binding, EndpointAddress remoteAddress, CustomizedMessageHeaderData messageHeader)
      : base(binding, remoteAddress)
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
