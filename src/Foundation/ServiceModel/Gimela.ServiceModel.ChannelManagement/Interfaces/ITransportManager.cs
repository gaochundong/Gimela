using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 传输通道管理器接口，负责管理建立通道，订阅通道状态等。
  /// </summary>
  public interface ITransportManager
  {
    #region Subscribe

    /// <summary>
    /// 订阅通道连接状态事件
    /// </summary>
    /// <typeparam name="TContract">需要订阅的服务契约</typeparam>
    /// <typeparam name="TEventArgs">订阅通道状态参数类型</typeparam>
    /// <param name="eventType">通道状态类型</param>
    /// <param name="callback">通道状态变化回调函数</param>
    void Subscribe<TContract, TEventArgs>(ChannelConnectionEventType eventType, EventHandler<TEventArgs> callback)
      where TContract : class
      where TEventArgs : EventArgs;

    /// <summary>
    /// 取消订阅通道连接状态事件
    /// </summary>
    /// <typeparam name="TContract">需要订阅的服务契约</typeparam>
    /// <typeparam name="TEventArgs">订阅通道状态参数类型</typeparam>
    /// <param name="eventType">通道状态类型</param>
    /// <param name="callback">通道状态变化回调函数</param>
    void Unsubscribe<TContract, TEventArgs>(ChannelConnectionEventType eventType, EventHandler<TEventArgs> callback)
      where TContract : class
      where TEventArgs : EventArgs;

    #endregion

    #region GetChannel

    /// <summary>
    /// 获取指定服务契约类型的通道连接
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <returns>指定服务契约类型的通道连接</returns>
    TContract GetChannel<TContract>() where TContract : class;

    /// <summary>
    /// 异步获取指定服务契约类型的通道连接，通过回调接口通知
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="callback">回调函数</param>
    void GetChannelAsync<TContract>(EventHandler<ChannelEstablishedEventArgs> callback) where TContract : class;

    /// <summary>
    /// 获取指定服务契约类型的所有通道连接
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <returns>指定服务契约类型的所有通道连接</returns>
    IList<TContract> GetChannels<TContract>() where TContract : class;

    #endregion

    #region GetDuplexChannel

    /// <summary>
    /// 获取指定服务契约类型的双向通道连接
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="instanceContext">客户端实例上下文</param>
    /// <returns>指定服务契约类型的双向通道连接</returns>
    TContract GetDuplexChannel<TContract>(InstanceContext instanceContext) where TContract : class;

    /// <summary>
    /// 异步获取指定服务契约类型的双向通道连接，通过回调接口通知
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="instanceContext">客户端实例上下文</param>
    /// <param name="callback">回调函数</param>
    void GetDuplexChannelAsync<TContract>(InstanceContext instanceContext, EventHandler<ChannelEstablishedEventArgs> callback) where TContract : class;

    #endregion

    #region RegisterCallback

    /// <summary>
    /// 注册回调契约通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <param name="callbackInstanceContext">回调契约通道连接</param>
    void RegisterCallback<TContract>(TContract callbackInstanceContext) where TContract : class;

    /// <summary>
    /// 注册回调契约通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <param name="callbackInstanceContext">回调契约通道连接</param>
    /// <param name="hostName">客户端主机唯一识别名</param>
    void RegisterCallback<TContract>(TContract callbackInstanceContext, string hostName) where TContract : class;

    /// <summary>
    /// 取消注册回调契约通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    void UnregisterCallback<TContract>() where TContract : class;

    /// <summary>
    /// 取消注册回调契约通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <param name="hostName">客户端主机唯一识别名</param>
    void UnregisterCallback<TContract>(string hostName) where TContract : class;

    #endregion

    #region GetCallbackChannel

    /// <summary>
    /// 获取指定回调契约类型的通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <returns>指定回调契约类型的通道连接</returns>
    TContract GetCallbackChannel<TContract>() where TContract : class;

    /// <summary>
    /// 获取指定回调契约类型的通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <param name="hostName">客户端主机唯一识别名</param>
    /// <returns>指定回调契约类型的通道连接</returns>
    TContract GetCallbackChannel<TContract>(string hostName) where TContract : class;

    /// <summary>
    /// 获取指定回调契约类型的所有通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <returns>指定回调契约类型的所有通道连接</returns>
    IList<TContract> GetCallbackChannels<TContract>() where TContract : class;

    #endregion
  }
}
