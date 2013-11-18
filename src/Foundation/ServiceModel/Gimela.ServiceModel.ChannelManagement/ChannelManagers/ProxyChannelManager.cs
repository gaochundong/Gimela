using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using Gimela.ServiceModel.ChannelManagement.Factories;
using Gimela.Common.ExceptionHandling;
using Gimela.Common.Logging;
using Gimela.ServiceModel.ManagedDiscovery;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 单向通道管理器
  /// </summary>
  /// <typeparam name="TContract">服务契约类型</typeparam>
  internal class ProxyChannelManager<TContract> : IProxyChannelManager<TContract> where TContract : class
  {
    #region Fields

    private delegate void Connect();

    private IProxyChannel<TContract> _proxyChannel;
    private object _proxyChannelLocker = new object();

    private ContractInfo _contract;
    private Connect _connectAction;
    private object _channelConnectingLocker = new object();
    private bool _isChannelConnecting = false;        // whether we are connecting the channel
    private bool _isChannelConnectedFirstTime = true; // fire channel connected event when first time connected successfully
    private int _connectWaitingTime = 30000;          // wait connection until timeout
    private int _retryConnectBreakTime = 10000;       // sleep time when every time retry to connect channel
    private int _retryConnectCount = 0;               // the count of retrying to connect the channel

    #endregion

    #region Constructors

    /// <summary>
    /// 单向通道管理器
    /// </summary>
    /// <param name="contract">服务契约描述</param>
    public ProxyChannelManager(ContractInfo contract)
    {
      if (contract == null)
        throw new ArgumentNullException("contract");

      _contract = contract;
    }

    #endregion

    #region IProxyChannelManager<TContract> Members

    #region Connection Events
    
    /// <summary>
    /// 通道已连接成功
    /// </summary>
    public event EventHandler<ChannelConnectedEventArgs> ChannelConnected;

    /// <summary>
    /// 通道已重新连接成功
    /// </summary>
    public event EventHandler<ChannelConnectedEventArgs> ChannelReconnected;

    /// <summary>
    /// 通道连接断开
    /// </summary>
    public event EventHandler<ChannelDisconnectedEventArgs> ChannelDisconnected;

    /// <summary>
    /// 通道产生异常
    /// </summary>
    public event EventHandler<ChannelExceptionRaisedEventArgs> ChannelExceptionRaised;

    #endregion

    /// <summary>
    /// 服务契约描述
    /// </summary>
    public ContractInfo ContractInfo
    {
      get { return _contract; }
    }

    /// <summary>
    /// 获取指定服务契约的通道
    /// </summary>
    /// <returns>指定服务契约的通道</returns>
    public TContract GetChannel()
    {
      if (!IsConnected())
      {
        GetChannelAsync();

        lock (this)
        {
          Monitor.Wait(this, _connectWaitingTime);
        }

        if (_proxyChannel == null || _isChannelConnecting)
        {
          throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to get channel or service is still down for [{0}].", typeof(TContract).Name));
        }
      }

      return _proxyChannel.Channel;
    }

    /// <summary>
    /// 异步获取指定服务契约的通道
    /// </summary>
    public void GetChannelAsync()
    {
      if (_connectAction == null)
      {
        _connectAction = new Connect(OnConnect);
      }

      _connectAction.BeginInvoke(null, null);
    }

    #endregion

    #region Create Channel

    protected IProxyChannel<TContract> ProxyChannel
    {
      get
      {
        return this._proxyChannel;
      }
      set
      {
        this._proxyChannel = value;
      }
    }

    protected virtual void CreateChannel()
    {
      _proxyChannel = ProxyChannelFactory.CreateProxyChannel<TContract>(_contract.HostName);
    }

    protected void ReleaseChannel()
    {
      if (_proxyChannel != null)
      {
        lock (_proxyChannelLocker)
        {
          try
          {
            if (_proxyChannel != null)
            {
              try
              {
                _proxyChannel.InnerChannel.Faulted -= new EventHandler(OnChannelCommunnicationFaulted);
                _proxyChannel.Close();
              }
              catch (Exception ex)
              {
                ExceptionHandler.Handle(ex);
                _proxyChannel.Abort();
              }

              (_proxyChannel as IDisposable).Dispose();
            }
          }
          catch (Exception ex)
          {
            ExceptionHandler.Handle(ex);
          }
          finally
          {
            _proxyChannel = null;
          }
        }
      }
    }

    #endregion

    #region Connect Service

    private bool IsConnected()
    {
      if (_proxyChannel == null)
      {
        return false;
      }
      else
      {
        return (_proxyChannel.State == CommunicationState.Opened);
      }
    }

    private void OnConnect()
    {
      try
      {
        bool connecting = false;
        lock (_channelConnectingLocker)
        {
          connecting = _isChannelConnecting;
          if (!_isChannelConnecting)
          {
            _isChannelConnecting = true;
          }
        }

        // another thread is connecting
        if (connecting == true)
        {
          return;
        }

        WaitingConnection();

        if (_proxyChannel != null)
        {
          lock (_channelConnectingLocker)
          {
            _isChannelConnecting = false;
          }

          OnConnectEnd(null);
          RaiseChannelConnectedEvent();
        }
      }
      catch (Exception ex)
      {
        Logger.Debug(string.Format(CultureInfo.InvariantCulture, "Failed on trying to establish connection to [{0}], {1}", _contract.ContractType, ex.Message));
        ExceptionHandler.Handle(ex);
      }
    }

    private void OnConnectEnd(IAsyncResult result)
    {
      try
      {
        lock (_channelConnectingLocker)
        {
          if (!_isChannelConnecting)
          {
            lock (this)
            {
              Monitor.PulseAll(this);
            }
          }
        }
        if (result != null)
        {
          _connectAction.EndInvoke(result);
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    private void WaitingConnection()
    {
      while (_proxyChannel == null || (_proxyChannel.State != CommunicationState.Opened))
      {
        try
        {
          ConnectService();
        }
        catch (EndpointNotFoundException ex)
        {
          ExceptionHandler.Handle(ex);
          Logger.Error(string.Format(CultureInfo.InvariantCulture, "Check if the service providing this contract has reported an alarm or logged an error and destination network connectivity, error : {0}", ex.ToString()));
        }
        catch (ContractNotFoundException ex)
        {
          ExceptionHandler.Handle(ex);
          Logger.Error(string.Format(CultureInfo.InvariantCulture, "Check if the service providing this contract is installed and running without error, error : {0}", ex.ToString()));
        }
        catch (Exception ex)
        {
          RaiseChannelExceptionRaisedEvent(ex);
        }
        finally
        {
          if (!IsConnected())
          {
            _retryConnectCount++;
            Thread.Sleep(_retryConnectBreakTime);
          }
          else
          {
            _retryConnectCount = 0;
          }
        }
      }
    }

    private void ConnectService()
    {
      lock (_proxyChannelLocker)
      {
        if (_proxyChannel != null)
        {
          ReleaseChannel();
        }

        CreateChannel();
      }

      if (_proxyChannel != null && _proxyChannel.State == CommunicationState.Created)
      {
        _proxyChannel.Open();
      }
      else
      {
        if (_proxyChannel != null && _proxyChannel.State != CommunicationState.Created)
        {
          _proxyChannel = null;
        }
      }

      if (_proxyChannel != null)
      {
        _proxyChannel.InnerChannel.Faulted += new EventHandler(OnChannelCommunnicationFaulted);
      }
    }

    #endregion

    #region Channel Error

    private void OnChannelCommunnicationFaulted(object sender, EventArgs e)
    {
      ThreadPool.QueueUserWorkItem(new WaitCallback(HandleChannelCommunicationError));
    }

    private void HandleChannelCommunicationError(object state)
    {
      try
      {
        lock (_channelConnectingLocker)
        {
          _isChannelConnecting = true;
        }

        ReleaseChannel();
        RaiseChannelDisconnectedEvent();

        WaitingConnection();

        lock (_channelConnectingLocker)
        {
          _isChannelConnecting = false;
        }

        OnConnectEnd(null);
        RaiseChannelReconnectedEvent();
      }
      catch (Exception ex)
      {
        RaiseChannelExceptionRaisedEvent(new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Unexpected error has occured, {0}", ex.Message), ex));
      }
    }

    #endregion

    #region Raise Events

    private void RaiseChannelConnectedEvent()
    {
      if (_isChannelConnectedFirstTime)
      {
        if (ChannelConnected != null)
        {
          object channel = _proxyChannel.Channel;
          foreach (EventHandler<ChannelConnectedEventArgs> function in ChannelConnected.GetInvocationList())
          {
            try
            {
              function.BeginInvoke(this, new ChannelConnectedEventArgs(_contract, channel), OnNotifyChannelConnectedEventCompleted, function);
            }
            catch (Exception ex)
            {
              ExceptionHandler.Handle(ex);
            }
          }
        }
        _isChannelConnectedFirstTime = false;
      }
    }

    private void RaiseChannelReconnectedEvent()
    {
      if (ChannelReconnected != null)
      {
        object channel = _proxyChannel.Channel;
        foreach (EventHandler<ChannelConnectedEventArgs> function in ChannelReconnected.GetInvocationList())
        {
          try
          {
            function.BeginInvoke(this, new ChannelConnectedEventArgs(_contract, channel), OnNotifyChannelConnectedEventCompleted, function);
          }
          catch (Exception ex)
          {
            ExceptionHandler.Handle(ex);
          }
        }
      }
    }

    private void RaiseChannelDisconnectedEvent()
    {
      if (ChannelDisconnected != null)
      {
        try
        {
          foreach (EventHandler<ChannelDisconnectedEventArgs> function in ChannelDisconnected.GetInvocationList())
          {
            try
            {
              function.BeginInvoke(this, new ChannelDisconnectedEventArgs(_contract, "The service has unexpectedly disconnected."), OnNotifyChannelDisconnectedEventCompleted, function);
            }
            catch (Exception ex)
            {
              ExceptionHandler.Handle(ex);
            }
          }
        }
        catch (Exception ex)
        {
          ExceptionHandler.Handle(ex);
        }
      }
    }

    private void RaiseChannelExceptionRaisedEvent(Exception exception)
    {
      try
      {
        if (ChannelExceptionRaised != null)
        {
          foreach (EventHandler<ChannelExceptionRaisedEventArgs> function in ChannelExceptionRaised.GetInvocationList())
          {
            try
            {
              function.BeginInvoke(this, new ChannelExceptionRaisedEventArgs(_contract, exception), OnNotifyChannelExceptionRaisedEventCompleted, function);
            }
            catch (Exception ex)
            {
              ExceptionHandler.Handle(ex);
            }
          }
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    private void OnNotifyChannelConnectedEventCompleted(IAsyncResult result)
    {
      try
      {
        EventHandler<ChannelConnectedEventArgs> del = result.AsyncState as EventHandler<ChannelConnectedEventArgs>;
        if (del != null)
        {
          del.EndInvoke(result);
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    private void OnNotifyChannelDisconnectedEventCompleted(IAsyncResult result)
    {
      try
      {
        EventHandler<ChannelDisconnectedEventArgs> del = result.AsyncState as EventHandler<ChannelDisconnectedEventArgs>;
        if (del != null)
        {
          del.EndInvoke(result);
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    private void OnNotifyChannelExceptionRaisedEventCompleted(IAsyncResult result)
    {
      try
      {
        EventHandler<ChannelExceptionRaisedEventArgs> del = result.AsyncState as EventHandler<ChannelExceptionRaisedEventArgs>;

        if (del != null)
        {
          del.EndInvoke(result);
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        ReleaseChannel();
      }
    }

    #endregion
  }
}
