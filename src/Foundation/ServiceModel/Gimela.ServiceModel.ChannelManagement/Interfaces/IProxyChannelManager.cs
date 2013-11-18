using System;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 代理通道管理器接口
  /// </summary>
  /// <typeparam name="TContract">代理通道类型</typeparam>
  public interface IProxyChannelManager<TContract> : IDisposable
  {
    #region Connection Events

    /// <summary>
    /// 通道已连接成功
    /// </summary>
    event EventHandler<ChannelConnectedEventArgs> ChannelConnected;

    /// <summary>
    /// 通道已重新连接成功
    /// </summary>
    event EventHandler<ChannelConnectedEventArgs> ChannelReconnected;

    /// <summary>
    /// 通道连接断开
    /// </summary>
    event EventHandler<ChannelDisconnectedEventArgs> ChannelDisconnected;

    /// <summary>
    /// 通道产生异常
    /// </summary>
    event EventHandler<ChannelExceptionRaisedEventArgs> ChannelExceptionRaised;

    #endregion

    /// <summary>
    /// 服务契约描述
    /// </summary>
    ContractInfo ContractInfo { get; }

    /// <summary>
    /// 获取指定服务契约的通道
    /// </summary>
    /// <returns>指定服务契约的通道</returns>
    TContract GetChannel();

    /// <summary>
    /// 异步获取指定服务契约的通道
    /// </summary>
    void GetChannelAsync();
  }
}
