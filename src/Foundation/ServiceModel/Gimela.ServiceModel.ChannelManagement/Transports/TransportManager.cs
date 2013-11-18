using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Gimela.ServiceModel.ChannelManagement.Factories;
using Gimela.Common.ExceptionHandling;
using Gimela.ServiceModel.ManagedDiscovery;

namespace Gimela.ServiceModel.ChannelManagement.Transports
{
  /// <summary>
  /// 传输通道管理器，负责管理建立通道，订阅通道状态等。
  /// </summary>
  internal class TransportManager : ITransportManager
  {
    #region Fields

    /// <summary>
    /// 注册各服务契约通道的管理器，Key为服务契约的唯一索引
    /// </summary>
    private readonly Dictionary<string, object> _proxyChannelManagerContainer = new Dictionary<string, object>();
    private readonly object _proxyChannelManagerContainerLocker = new object();

    private readonly Dictionary<string, List<EventHandler<ChannelConnectedEventArgs>>> _channelConnectedEventSubscribers = new Dictionary<string, List<EventHandler<ChannelConnectedEventArgs>>>();
    private readonly object _channelConnectedEventsSubscribersLocker = new object();

    private readonly Dictionary<string, List<EventHandler<ChannelConnectedEventArgs>>> _channelReconnectedEventSubscribers = new Dictionary<string, List<EventHandler<ChannelConnectedEventArgs>>>();
    private readonly object _channelReconnectedEventsSubscribersLocker = new object();

    private readonly Dictionary<string, List<EventHandler<ChannelDisconnectedEventArgs>>> _channelDisconnectedEventSubscribers = new Dictionary<string, List<EventHandler<ChannelDisconnectedEventArgs>>>();
    private readonly object _channelDisconnectedEventsSubscribersLocker = new object();

    private readonly Dictionary<string, List<EventHandler<ChannelExceptionRaisedEventArgs>>> _channelExceptionRaisedEventSubscribers = new Dictionary<string, List<EventHandler<ChannelExceptionRaisedEventArgs>>>();
    private readonly object _channelExceptionRaisedEventsSubscribersLocker = new object();

    private readonly Dictionary<string, List<EventHandler<ChannelEstablishedEventArgs>>> _asyncChannelEstablishedCallbacks = new Dictionary<string, List<EventHandler<ChannelEstablishedEventArgs>>>();
    private readonly object _asyncChannelEstablishedCallbacksLocker = new object();

    #endregion

    #region Ctors

    /// <summary>
    /// 传输通道管理器
    /// </summary>
    public TransportManager()
    {
    }

    #endregion

    #region ITransportManager Members

    #region Subscribe

    /// <summary>
    /// 订阅通道连接状态事件
    /// </summary>
    /// <typeparam name="TContract">需要订阅的服务契约</typeparam>
    /// <typeparam name="TEventArgs">订阅通道状态参数类型</typeparam>
    /// <param name="eventType">通道状态类型</param>
    /// <param name="callback">通道状态变化回调函数</param>
    public void Subscribe<TContract, TEventArgs>(ChannelConnectionEventType eventType, EventHandler<TEventArgs> callback)
      where TContract : class
      where TEventArgs : EventArgs
    {
      SubscribeInternal<TEventArgs>(new ContractInfo(typeof(TContract)), eventType, callback);
    }

    /// <summary>
    /// 取消订阅通道连接状态事件
    /// </summary>
    /// <typeparam name="TContract">需要订阅的服务契约</typeparam>
    /// <typeparam name="TEventArgs">订阅通道状态参数类型</typeparam>
    /// <param name="eventType">通道状态类型</param>
    /// <param name="callback">通道状态变化回调函数</param>
    public void Unsubscribe<TContract, TEventArgs>(ChannelConnectionEventType eventType, EventHandler<TEventArgs> callback)
      where TContract : class
      where TEventArgs : EventArgs
    {
      UnSubscribeInternal<TEventArgs>(new ContractInfo(typeof(TContract)), eventType, callback);
    }

    #endregion

    #region GetChannel

    /// <summary>
    /// 获取指定服务契约类型的通道连接
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <returns>指定服务契约类型的通道连接</returns>
    public TContract GetChannel<TContract>() where TContract : class
    {
      return GetChannelInternal<TContract>(new ContractInfo(typeof(TContract)));
    }

    /// <summary>
    /// 异步获取指定服务契约类型的通道连接，通过回调接口通知
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="callback">回调函数</param>
    public void GetChannelAsync<TContract>(EventHandler<ChannelEstablishedEventArgs> callback) where TContract : class
    {
      GetChannelAsyncInternal<TContract>(new ContractInfo(typeof(TContract)), callback);
    }

    /// <summary>
    /// 获取指定服务契约类型的所有通道连接
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <returns>指定服务契约类型的所有通道连接</returns>
    public IList<TContract> GetChannels<TContract>() where TContract : class
    {
      List<TContract> list = new List<TContract>();

      IList<ServiceEndpoint> endpoints = EndpointProvider.GetEndpoints<TContract>();
      List<IProxyChannelManager<TContract>> channelManagerList = new List<IProxyChannelManager<TContract>>();
      List<ServiceEndpoint> channelExistEndpoints = new List<ServiceEndpoint>();

      List<object> proxyChannelManagers = new List<object>(_proxyChannelManagerContainer.Values);
      foreach (var item in proxyChannelManagers)
      {
        if (item is ProxyChannelManager<TContract>)
        {
          IProxyChannelManager<TContract> channelManager = item as IProxyChannelManager<TContract>;
          if (channelManager != null)
          {
            channelManagerList.Add(channelManager);

            foreach (ServiceEndpoint endpoint in endpoints)
            {
              if (endpoint.Address.Uri.Host == channelManager.ContractInfo.HostName)
              {
                channelExistEndpoints.Add(endpoint);
                break;
              }
            }
          }
        }
      }

      foreach (ServiceEndpoint endpoint in channelExistEndpoints)
      {
        endpoints.Remove(endpoint);
      }

      if (endpoints.Count > 0)
      {
        // create proxy channel manager for those endpoints who have not been created
        foreach (ServiceEndpoint endpoint in endpoints)
        {
          ContractInfo contract = new ContractInfo(typeof(TContract), endpoint.Address.Uri.Host);
          IProxyChannelManager<TContract> proxyChannelManager = GetProxyChannelManager<TContract>(contract.Key);
          if (proxyChannelManager == null)
          {
            lock (_proxyChannelManagerContainerLocker)
            {
              proxyChannelManager = GetProxyChannelManager<TContract>(contract.Key);
              if (proxyChannelManager == null)
              {
                proxyChannelManager = ProxyChannelManagerFactory.Create<TContract>(contract);
                RegisterProxyChannelManager(proxyChannelManager, contract.Key);
              }
            }
          }

          if (proxyChannelManager != null)
          {
            channelManagerList.Add(proxyChannelManager);
          }
        }
      }

      foreach (ProxyChannelManager<TContract> channelManager in channelManagerList)
      {
        list.Add(channelManager.GetChannel());
      }

      return list;
    }

    #endregion

    #region GetDuplexChannel

    /// <summary>
    /// 获取指定服务契约类型的双向通道连接
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="instanceContext">客户端实例上下文</param>
    /// <returns>指定服务契约类型的双向通道连接</returns>
    public TContract GetDuplexChannel<TContract>(InstanceContext instanceContext) where TContract : class
    {
      return GetDuplexChannelInternal<TContract>(instanceContext, new ContractInfo(typeof(TContract)));
    }

    /// <summary>
    /// 异步获取指定服务契约类型的双向通道连接，通过回调接口通知
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="instanceContext">客户端实例上下文</param>
    /// <param name="callback">回调函数</param>
    public void GetDuplexChannelAsync<TContract>(InstanceContext instanceContext, EventHandler<ChannelEstablishedEventArgs> callback) where TContract : class
    {
      GetDuplexChannelAsyncInternal<TContract>(instanceContext, new ContractInfo(typeof(TContract)), callback);
    }

    #endregion

    #region RegisterCallback

    /// <summary>
    /// 注册回调契约通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <param name="callbackInstanceContext">回调契约通道连接</param>
    public void RegisterCallback<TContract>(TContract callbackInstanceContext) where TContract : class
    {
      RegisterCallbackInternal<TContract>(callbackInstanceContext, new ContractInfo(typeof(TContract)));
    }

    /// <summary>
    /// 注册回调契约通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <param name="callbackInstanceContext">回调契约通道连接</param>
    /// <param name="hostName">客户端主机唯一识别名</param>
    public void RegisterCallback<TContract>(TContract callbackInstanceContext, string hostName) where TContract : class
    {
      RegisterCallbackInternal<TContract>(callbackInstanceContext, new ContractInfo(typeof(TContract), hostName));
    }

    /// <summary>
    /// 取消注册回调契约通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    public void UnregisterCallback<TContract>() where TContract : class
    {
      UnregisterCallbackInternal<TContract>(new ContractInfo(typeof(TContract)));
    }

    /// <summary>
    /// 取消注册回调契约通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <param name="hostName">客户端主机唯一识别名</param>
    public void UnregisterCallback<TContract>(string hostName) where TContract : class
    {
      UnregisterCallbackInternal<TContract>(new ContractInfo(typeof(TContract), hostName));
    }

    #endregion

    #region GetCallbackChannel

    /// <summary>
    /// 获取指定回调契约类型的通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <returns>指定回调契约类型的通道连接</returns>
    public TContract GetCallbackChannel<TContract>() where TContract : class
    {
      return GetCallbackChannelInternal<TContract>(new ContractInfo(typeof(TContract)));
    }

    /// <summary>
    /// 获取指定回调契约类型的通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <param name="hostName">客户端主机唯一识别名</param>
    /// <returns>指定回调契约类型的通道连接</returns>
    public TContract GetCallbackChannel<TContract>(string hostName) where TContract : class
    {
      return GetCallbackChannelInternal<TContract>(new ContractInfo(typeof(TContract), hostName));
    }

    /// <summary>
    /// 获取指定回调契约类型的所有通道连接
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <returns>指定回调契约类型的所有通道连接</returns>
    public IList<TContract> GetCallbackChannels<TContract>() where TContract : class
    {
      List<TContract> list = new List<TContract>();

      List<object> proxyChannelManagers = new List<object>(_proxyChannelManagerContainer.Values);
      foreach (var item in proxyChannelManagers)
      {
        if (item is IProxyChannelManager<TContract>)
        {
          IProxyChannelManager<TContract> channelManager = item as IProxyChannelManager<TContract>;
          if (channelManager != null)
          {
            Type channelType = channelManager.GetChannel().GetType();
            Type expectedType = typeof(TContract);
            TContract callback = default(TContract);

            if (channelType.FullName != expectedType.FullName)
            {
              channelType = channelType.GetInterface(expectedType.FullName);
            }

            if (channelType != null && channelType.FullName == expectedType.FullName)
            {
              callback = channelManager.GetChannel();
              if (!list.Contains<TContract>(callback))
              {
                list.Add(callback);
              }
            }
          }
        }
      }

      return list;
    }

    #endregion

    #endregion

    #region Private Methods

    #region Subscribe

    private void SubscribeInternal<TEventArgs>(ContractInfo contract, ChannelConnectionEventType eventType, EventHandler<TEventArgs> callback)
      where TEventArgs : EventArgs
    {
      if (callback == null)
      {
        throw new ArgumentNullException("callback");
      }

      switch (eventType)
      {
        case ChannelConnectionEventType.Connected:
          List<EventHandler<ChannelConnectedEventArgs>> connectedListeners;
          lock (_channelConnectedEventsSubscribersLocker)
          {
            if (!_channelConnectedEventSubscribers.TryGetValue(contract.Key, out connectedListeners))
            {
              connectedListeners = new List<EventHandler<ChannelConnectedEventArgs>>();
              _channelConnectedEventSubscribers.Add(contract.Key, connectedListeners);
            }
          }
          connectedListeners.Add(callback as EventHandler<ChannelConnectedEventArgs>);
          break;
        case ChannelConnectionEventType.Reconnected:
          List<EventHandler<ChannelConnectedEventArgs>> reconnectedListeners;
          lock (_channelReconnectedEventsSubscribersLocker)
          {
            if (!_channelReconnectedEventSubscribers.TryGetValue(contract.Key, out reconnectedListeners))
            {
              reconnectedListeners = new List<EventHandler<ChannelConnectedEventArgs>>();
              _channelReconnectedEventSubscribers.Add(contract.Key, reconnectedListeners);
            }
          }
          reconnectedListeners.Add(callback as EventHandler<ChannelConnectedEventArgs>);
          break;
        case ChannelConnectionEventType.Disconnected:
          List<EventHandler<ChannelDisconnectedEventArgs>> disconnectedListeners;
          lock (_channelDisconnectedEventsSubscribersLocker)
          {
            if (!_channelDisconnectedEventSubscribers.TryGetValue(contract.Key, out disconnectedListeners))
            {
              disconnectedListeners = new List<EventHandler<ChannelDisconnectedEventArgs>>();
              _channelDisconnectedEventSubscribers.Add(contract.Key, disconnectedListeners);
            }
          }
          disconnectedListeners.Add(callback as EventHandler<ChannelDisconnectedEventArgs>);
          break;
        case ChannelConnectionEventType.ExceptionRaised:
          List<EventHandler<ChannelExceptionRaisedEventArgs>> exceptionRaisedListeners;
          lock (_channelExceptionRaisedEventsSubscribersLocker)
          {
            if (!_channelExceptionRaisedEventSubscribers.TryGetValue(contract.Key, out exceptionRaisedListeners))
            {
              exceptionRaisedListeners = new List<EventHandler<ChannelExceptionRaisedEventArgs>>();
              _channelExceptionRaisedEventSubscribers.Add(contract.Key, exceptionRaisedListeners);
            }
          }
          exceptionRaisedListeners.Add(callback as EventHandler<ChannelExceptionRaisedEventArgs>);
          break;
      }
    }

    private void UnSubscribeInternal<TEventArgs>(ContractInfo contract, ChannelConnectionEventType eventType, EventHandler<TEventArgs> callback)
      where TEventArgs : EventArgs
    {
      if (callback == null)
      {
        throw new ArgumentNullException("callback");
      }

      switch (eventType)
      {
        case ChannelConnectionEventType.Connected:
          List<EventHandler<ChannelConnectedEventArgs>> connectedListeners;
          if (_channelConnectedEventSubscribers.TryGetValue(contract.Key, out connectedListeners))
          {
            connectedListeners.Remove(callback as EventHandler<ChannelConnectedEventArgs>);
          }
          break;
        case ChannelConnectionEventType.Reconnected:
          List<EventHandler<ChannelConnectedEventArgs>> reconnectedListeners;
          if (_channelReconnectedEventSubscribers.TryGetValue(contract.Key, out reconnectedListeners))
          {
            reconnectedListeners.Remove(callback as EventHandler<ChannelConnectedEventArgs>);
          }
          break;
        case ChannelConnectionEventType.Disconnected:
          List<EventHandler<ChannelDisconnectedEventArgs>> disconnectedListeners;
          if (_channelDisconnectedEventSubscribers.TryGetValue(contract.Key, out disconnectedListeners))
          {
            disconnectedListeners.Remove(callback as EventHandler<ChannelDisconnectedEventArgs>);
          }
          break;
        case ChannelConnectionEventType.ExceptionRaised:
          List<EventHandler<ChannelExceptionRaisedEventArgs>> exceptionRaisedListeners;
          if (_channelExceptionRaisedEventSubscribers.TryGetValue(contract.Key, out exceptionRaisedListeners))
          {
            exceptionRaisedListeners.Remove(callback as EventHandler<ChannelExceptionRaisedEventArgs>);
          }
          break;
      }
    }

    #endregion

    #region GetChannel

    private TContract GetChannelInternal<TContract>(ContractInfo contract) where TContract : class
    {
      // get the proxy channel manager by the specified contract type and key
      IProxyChannelManager<TContract> proxyChannelManager = GetProxyChannelManager<TContract>(contract.Key);
      if (proxyChannelManager == null)
      {
        lock (_proxyChannelManagerContainerLocker)
        {
          // double check the channel manager value
          proxyChannelManager = GetProxyChannelManager<TContract>(contract.Key);
          if (proxyChannelManager == null)
          {
            proxyChannelManager = ProxyChannelManagerFactory.Create<TContract>(contract);
            RegisterProxyChannelManager(proxyChannelManager, contract.Key);
          }
        }
      }

      return proxyChannelManager.GetChannel();
    }

    private void GetChannelAsyncInternal<TContract>(ContractInfo contract, EventHandler<ChannelEstablishedEventArgs> callback) where TContract : class
    {
      IProxyChannelManager<TContract> proxyChannelManager = GetProxyChannelManager<TContract>(contract.Key);
      if (proxyChannelManager == null)
      {
        lock (_proxyChannelManagerContainerLocker)
        {
          proxyChannelManager = GetProxyChannelManager<TContract>(contract.Key);
          if (proxyChannelManager == null)
          {
            proxyChannelManager = ProxyChannelManagerFactory.Create<TContract>(contract);
            RegisterProxyChannelManager(proxyChannelManager, contract.Key);
          }
        }
      }

      SaveAsyncChannelEstablishedCallback(contract.Key, callback);
      proxyChannelManager.GetChannelAsync();
    }

    #endregion

    #region GetDuplexChannel

    private TContract GetDuplexChannelInternal<TContract>(InstanceContext instanceContext, ContractInfo contract) where TContract : class
    {
      IProxyChannelManager<TContract> proxyChannelManager = GetProxyChannelManager<TContract>(contract.Key);
      if (proxyChannelManager == null)
      {
        lock (_proxyChannelManagerContainerLocker)
        {
          proxyChannelManager = GetProxyChannelManager<TContract>(contract.Key);
          if (proxyChannelManager == null)
          {
            proxyChannelManager = ProxyChannelManagerFactory.CreateDuplex<TContract>(instanceContext, contract);
            RegisterProxyChannelManager(proxyChannelManager, contract.Key);
          }
        }
      }

      return proxyChannelManager.GetChannel();
    }

    private void GetDuplexChannelAsyncInternal<TContract>(InstanceContext instanceContext, ContractInfo contract, EventHandler<ChannelEstablishedEventArgs> callback) where TContract : class
    {
      IProxyChannelManager<TContract> proxyChannelManager = GetProxyChannelManager<TContract>(contract.Key);
      if (proxyChannelManager == null)
      {
        lock (_proxyChannelManagerContainerLocker)
        {
          proxyChannelManager = GetProxyChannelManager<TContract>(contract.Key);
          if (proxyChannelManager == null)
          {
            proxyChannelManager = ProxyChannelManagerFactory.CreateDuplex<TContract>(instanceContext, contract);
            RegisterProxyChannelManager(proxyChannelManager, contract.Key);
          }
        }
      }

      SaveAsyncChannelEstablishedCallback(contract.Key, callback);
      proxyChannelManager.GetChannelAsync();
    }

    #endregion

    #region RegisterCallback

    private void RegisterCallbackInternal<TContract>(TContract callbackInstanceContext, ContractInfo contract) where TContract : class
    {
      lock (_proxyChannelManagerContainerLocker)
      {
        IProxyChannelManager<TContract> channelMgr = ProxyChannelManagerFactory.CreateCallback<TContract>(callbackInstanceContext, contract);
        _proxyChannelManagerContainer[contract.Key] = channelMgr;
      }
    }

    private void UnregisterCallbackInternal<TContract>(ContractInfo contract) where TContract : class
    {
      lock (_proxyChannelManagerContainerLocker)
      {
        object channelObj;
        if (_proxyChannelManagerContainer.TryGetValue(contract.Key, out channelObj))
        {
          _proxyChannelManagerContainer.Remove(contract.Key);
          ProxyChannelFactory.CloseChannel(channelObj as IChannel);
        }
      }
    }

    #endregion

    #region GetCallbackChannel

    private TContract GetCallbackChannelInternal<TContract>(ContractInfo contract)
    {
      IProxyChannelManager<TContract> channelManager = GetProxyChannelManager<TContract>(contract.Key);
      if (channelManager == null)
      {
        lock (_proxyChannelManagerContainerLocker)
        {
          channelManager = GetProxyChannelManager<TContract>(contract.Key);
        }
      }
      TContract channel = default(TContract);
      if (channelManager != null)
      {
        channel = channelManager.GetChannel();
      }
      return channel;
    }

    #endregion

    #region RegisterProxyChannelManager

    private void RegisterProxyChannelManager<TContract>(IProxyChannelManager<TContract> proxyChannelManager, string contractKey)
    {
      if (!_proxyChannelManagerContainer.ContainsKey(contractKey))
      {
        proxyChannelManager.ChannelConnected += new EventHandler<ChannelConnectedEventArgs>(OnProxyChannelManagerChannelConnected);
        proxyChannelManager.ChannelReconnected += new EventHandler<ChannelConnectedEventArgs>(OnProxyChannelManagerChannelReconnected);
        proxyChannelManager.ChannelDisconnected += new EventHandler<ChannelDisconnectedEventArgs>(OnProxyChannelManagerChannelDisconnected);
        proxyChannelManager.ChannelExceptionRaised += new EventHandler<ChannelExceptionRaisedEventArgs>(OnProxyChannelManagerChannelExceptionRaised);

        _proxyChannelManagerContainer.Add(contractKey, proxyChannelManager);
      }
      else
      {
        proxyChannelManager = GetProxyChannelManager<TContract>(contractKey);
      }
    }

    private IProxyChannelManager<TContract> GetProxyChannelManager<TContract>(string contractKey)
    {
      IProxyChannelManager<TContract> channelManager;
      object obj;
      _proxyChannelManagerContainer.TryGetValue(contractKey, out obj);
      channelManager = obj as IProxyChannelManager<TContract>;
      return channelManager;
    }

    #endregion

    #region AsyncChannelEstablished

    private void SaveAsyncChannelEstablishedCallback(string contractKey, EventHandler<ChannelEstablishedEventArgs> callback)
    {
      if (callback != null)
      {
        List<EventHandler<ChannelEstablishedEventArgs>> callbacks;
        lock (_asyncChannelEstablishedCallbacksLocker)
        {
          if (!_asyncChannelEstablishedCallbacks.TryGetValue(contractKey, out callbacks))
          {
            callbacks = new List<EventHandler<ChannelEstablishedEventArgs>>();
            _asyncChannelEstablishedCallbacks.Add(contractKey, callbacks);
          }
          callbacks.Add(callback);
        }
      }
    }

    private void OnAsyncChannelEstablished(ChannelEstablishedEventArgs e)
    {
      List<EventHandler<ChannelEstablishedEventArgs>> callbacks;
      if (_asyncChannelEstablishedCallbacks.TryGetValue(e.ContractInfo.Key, out callbacks))
      {
        foreach (EventHandler<ChannelEstablishedEventArgs> callback in callbacks)
        {
          try
          {
            if (callback != null)
            {
              callback(this, e);
            }
          }
          catch (Exception ex)
          {
            ExceptionHandler.Handle(ex);
          }
        }
        lock (_asyncChannelEstablishedCallbacksLocker)
        {
          callbacks.Clear();
        }
      }
    }

    #endregion

    #endregion

    #region Handle Connection Events

    private void OnProxyChannelManagerChannelConnected(object sender, ChannelConnectedEventArgs e)
    {
      OnAsyncChannelEstablished(new ChannelEstablishedEventArgs(e.ContractInfo, e.Channel));

      List<EventHandler<ChannelConnectedEventArgs>> subscribers;
      if (_channelConnectedEventSubscribers.TryGetValue(e.ContractInfo.Key, out subscribers))
      {
        foreach (EventHandler<ChannelConnectedEventArgs> listener in subscribers)
        {
          try
          {
            if (listener != null)
            {
              listener(this, e);
            }
          }
          catch (Exception ex)
          {
            ExceptionHandler.Handle(ex);
          }
        }
      }
    }

    private void OnProxyChannelManagerChannelReconnected(object sender, ChannelConnectedEventArgs e)
    {
      List<EventHandler<ChannelConnectedEventArgs>> subscribers;
      if (_channelReconnectedEventSubscribers.TryGetValue(e.ContractInfo.Key, out subscribers))
      {
        foreach (EventHandler<ChannelConnectedEventArgs> listener in subscribers)
        {
          try
          {
            if (listener != null)
            {
              listener(this, e);
            }
          }
          catch (Exception ex)
          {
            ExceptionHandler.Handle(ex);
          }
        }
      }
    }

    private void OnProxyChannelManagerChannelDisconnected(object sender, ChannelDisconnectedEventArgs e)
    {
      List<EventHandler<ChannelDisconnectedEventArgs>> subscribers;
      if (_channelDisconnectedEventSubscribers.TryGetValue(e.ContractInfo.Key, out subscribers))
      {
        foreach (EventHandler<ChannelDisconnectedEventArgs> listener in subscribers)
        {
          try
          {
            if (listener != null)
            {
              listener(this, e);
            }
          }
          catch (Exception ex)
          {
            ExceptionHandler.Handle(ex);
          }
        }
      }
    }

    private void OnProxyChannelManagerChannelExceptionRaised(object sender, ChannelExceptionRaisedEventArgs e)
    {
      OnAsyncChannelEstablished(new ChannelEstablishedEventArgs(e.ContractInfo, null, e.Exception));

      List<EventHandler<ChannelExceptionRaisedEventArgs>> subscribers;
      if (_channelExceptionRaisedEventSubscribers.TryGetValue(e.ContractInfo.Key, out subscribers))
      {
        foreach (EventHandler<ChannelExceptionRaisedEventArgs> listener in subscribers)
        {
          try
          {
            if (listener != null)
            {
              listener(this, e);
            }
          }
          catch (Exception ex)
          {
            ExceptionHandler.Handle(ex);
          }
        }
      }
    }

    #endregion
  }
}
