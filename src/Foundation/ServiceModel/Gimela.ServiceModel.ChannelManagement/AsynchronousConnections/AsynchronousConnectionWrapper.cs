using System;
using System.ServiceModel;
using Gimela.ServiceModel.ChannelManagement.Factories;

namespace Gimela.ServiceModel.ChannelManagement.AsynchronousConnections
{
  /// <summary>
  /// 异步服务连接包装器
  /// </summary>
  /// <typeparam name="TContract">服务契约类型</typeparam>
  public class AsynchronousConnectionWrapper<TContract> : IAsynchronousConnectionWrapper where TContract : class
  {
    #region Fields
    
    private readonly IAsynchronousConnectionMonitor _monitor;
    private ITransportManager _transporter;
    private InstanceContext _instanceContext;
    private bool _isConnected = false;

    #endregion

    #region Ctors

    /// <summary>
    /// 异步服务连接包装器
    /// </summary>
    /// <param name="monitor">异步服务连接状态监听器</param>
    public AsynchronousConnectionWrapper(IAsynchronousConnectionMonitor monitor)
      : this(monitor, null)
    {
    }

    /// <summary>
    /// 异步服务连接包装器
    /// </summary>
    /// <param name="monitor">异步服务连接状态监听器</param>
    /// <param name="instanceContext">双向通道回调实例上下文</param>
    public AsynchronousConnectionWrapper(IAsynchronousConnectionMonitor monitor, InstanceContext instanceContext)
    {
      if (monitor == null)
      {
        throw new ArgumentNullException("monitor");
      }

      _monitor = monitor;
      _instanceContext = instanceContext;

      Connect();
    }

    #endregion

    #region IAsynchronousConnectionHolder Members

    /// <summary>
    /// 获取指定服务接口类型的服务通道
    /// </summary>
    /// <typeparam name="TInterface">指定服务接口类型</typeparam>
    /// <returns>指定服务接口类型的服务通道</returns>
    public TInterface GetChannel<TInterface>() where TInterface : class
    {
      return _transporter.GetChannel<TInterface>();
    }

    /// <summary>
    /// 连接指定的服务接口
    /// </summary>
    public void Connect()
    {
      if (!IsConnected)
      {
        _transporter = TransportManagerFactory.Create();
        _transporter.Subscribe<TContract, ChannelConnectedEventArgs>(ChannelConnectionEventType.Connected, OnConnectionConnected);
        _transporter.Subscribe<TContract, ChannelConnectedEventArgs>(ChannelConnectionEventType.Reconnected, OnConnectionReconnected);
        _transporter.Subscribe<TContract, ChannelDisconnectedEventArgs>(ChannelConnectionEventType.Disconnected, OnConnectionDisconnected);
        _transporter.Subscribe<TContract, ChannelExceptionRaisedEventArgs>(ChannelConnectionEventType.ExceptionRaised, OnConnectionExceptionRaised);

        StartConnect();
      }
    }

    /// <summary>
    /// 指定服务接口类型的服务通道是否已连接
    /// </summary>
    public bool IsConnected
    {
      get { return _isConnected; }
    }

    #endregion

    #region Private Methods
    
    private void StartConnect()
    {
      if (_instanceContext != null)
      {
        _transporter.GetDuplexChannelAsync<TContract>(_instanceContext, null);        
      }
      else
      {
        _transporter.GetChannelAsync<TContract>(null);
      }
    }

    #endregion

    #region Events Handlers
    
    private void OnConnectionConnected(object sender, ChannelConnectedEventArgs e)
    {
      _isConnected = true;
      _monitor.OnConnectionConnected(e);
    }

    private void OnConnectionReconnected(object sender, ChannelConnectedEventArgs e)
    {
      _isConnected = true;
      _monitor.OnConnectionReconnected(e);
    }

    private void OnConnectionDisconnected(object sender, ChannelDisconnectedEventArgs e)
    {
      _isConnected = false;
      _monitor.OnConnectionDisconnected(e);
    }

    private void OnConnectionExceptionRaised(object sender, ChannelExceptionRaisedEventArgs e)
    {
      _isConnected = false;
      _monitor.OnConnectionExceptionRaised(e);
    }

    #endregion
  }
}
