using System;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 回调通道管理器，对应双向回调服务中的回调契约
  /// </summary>
  /// <typeparam name="TContract">回调契约</typeparam>
  internal class CallbackChannelManager<TContract> : IProxyChannelManager<TContract> where TContract : class
  {
    #region Fields

    private TContract _callback;
    private ContractInfo _contract;

    #endregion

    #region Constructors

    /// <summary>
    /// 回调通道管理器，对应双向回调服务中的回调契约
    /// </summary>
    /// <param name="callbackInstanceContext">回调契约实例上下文</param>
    /// <param name="contract">回调契约描述</param>
    public CallbackChannelManager(TContract callbackInstanceContext, ContractInfo contract)
    {
      if (callbackInstanceContext == null)
      {
        throw new ArgumentNullException("callbackInstanceContext");
      }
      if (contract == null)
      {
        throw new ArgumentNullException("contract");
      }

      _callback = callbackInstanceContext;
      _contract = contract;
    }

    #endregion

    #region IProxyChannelManager Members

    #region Connection Events

    /// <summary>
    /// 通道已连接成功
    /// </summary>
    public event EventHandler<ChannelConnectedEventArgs> ChannelConnected
    {
      add
      {
        throw new NotSupportedException();
      }
      remove
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// 通道已重新连接成功
    /// </summary>
    public event EventHandler<ChannelConnectedEventArgs> ChannelReconnected
    {
      add
      {
        throw new NotSupportedException();
      }
      remove
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// 通道连接断开
    /// </summary>
    public event EventHandler<ChannelDisconnectedEventArgs> ChannelDisconnected
    {
      add
      {
        throw new NotSupportedException();
      }
      remove
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// 通道产生异常
    /// </summary>
    public event EventHandler<ChannelExceptionRaisedEventArgs> ChannelExceptionRaised
    {
      add
      {
        throw new NotSupportedException();
      }
      remove
      {
        throw new NotSupportedException();
      }
    }

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
      return _callback;
    }

    /// <summary>
    /// 异步获取指定服务契约的通道
    /// </summary>
    public void GetChannelAsync()
    {
      throw new NotSupportedException();
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
        _callback = default(TContract);
      }
    }

    #endregion
  }
}
