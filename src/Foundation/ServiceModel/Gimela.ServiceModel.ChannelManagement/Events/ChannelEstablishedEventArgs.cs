using System;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 通道连接已建立事件参数
  /// </summary>
  public class ChannelEstablishedEventArgs : EventArgs
  {
    /// <summary>
    /// 通道连接已建立事件参数
    /// </summary>
    /// <param name="contract">契约信息</param>
    public ChannelEstablishedEventArgs(ContractInfo contract)
    {
      if (contract == null)
      {
        throw new ArgumentNullException("contract");
      }

      ContractInfo = contract;
    }

    /// <summary>
    /// 通道连接已建立事件参数
    /// </summary>
    /// <param name="contract">契约信息</param>
    /// <param name="channel">连接通道</param>
    public ChannelEstablishedEventArgs(ContractInfo contract, object channel)
      : this(contract)
    {
      Channel = channel;
    }

    /// <summary>
    /// 通道连接已建立事件参数
    /// </summary>
    /// <param name="contract">契约信息</param>
    /// <param name="channel">连接通道</param>
    /// <param name="exception">连接异常</param>
    public ChannelEstablishedEventArgs(ContractInfo contract, object channel, Exception exception)
      : this(contract, channel)
    {
      Exception = exception;
    }

    /// <summary>
    /// 契约信息
    /// </summary>
    public ContractInfo ContractInfo { get; private set; }

    /// <summary>
    /// 连接通道
    /// </summary>
    public object Channel { get; private set; }

    /// <summary>
    /// 连接异常
    /// </summary>
    public Exception Exception { get; private set; }
  }
}
