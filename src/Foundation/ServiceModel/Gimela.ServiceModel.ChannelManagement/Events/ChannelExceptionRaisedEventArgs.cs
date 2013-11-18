using System;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 通道连接异常事件参数
  /// </summary>
  public class ChannelExceptionRaisedEventArgs : EventArgs
  {
    /// <summary>
    /// 通道连接异常事件参数
    /// </summary>
    /// <param name="contract">契约信息</param>
    public ChannelExceptionRaisedEventArgs(ContractInfo contract)
    {
      if (contract == null)
      {
        throw new ArgumentNullException("contract");
      }

      ContractInfo = contract;
    }

    /// <summary>
    /// 通道连接异常事件参数
    /// </summary>
    /// <param name="contract">契约信息</param>
    /// <param name="exception">连接异常</param>
    public ChannelExceptionRaisedEventArgs(ContractInfo contract, Exception exception)
      : this(contract)
    {
      Exception = exception;
    }

    /// <summary>
    /// 契约信息
    /// </summary>
    public ContractInfo ContractInfo { get; private set; }

    /// <summary>
    /// 连接异常
    /// </summary>
    public Exception Exception { get; private set; }
  }
}
