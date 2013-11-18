using System;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 通道已连接事件参数
  /// </summary>
  public class ChannelConnectedEventArgs : EventArgs
  {
    /// <summary>
    /// 通道已连接事件参数
    /// </summary>
    /// <param name="contract">契约信息</param>
    public ChannelConnectedEventArgs(ContractInfo contract)
    {
      if (contract == null)
      {
        throw new ArgumentNullException("contract");
      }

      ContractInfo = contract;
    }

    /// <summary>
    /// 通道已连接事件参数
    /// </summary>
    /// <param name="contract">契约信息</param>
    /// <param name="channel">通道</param>
    public ChannelConnectedEventArgs(ContractInfo contract, object channel)
      : this(contract)
    {
      Channel = channel;
    }

    /// <summary>
    /// 契约信息
    /// </summary>
    public ContractInfo ContractInfo { get; private set; }

    /// <summary>
    /// 通道
    /// </summary>
    public object Channel { get; private set; }
  }
}
