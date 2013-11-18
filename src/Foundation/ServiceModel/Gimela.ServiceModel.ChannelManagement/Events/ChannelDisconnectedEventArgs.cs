using System;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 通道已重新连接事件参数
  /// </summary>
  public class ChannelDisconnectedEventArgs : EventArgs
  {
    /// <summary>
    /// 通道已重新连接事件参数
    /// </summary>
    /// <param name="contract">契约信息</param>
    public ChannelDisconnectedEventArgs(ContractInfo contract)
    {
      if (contract == null)
      {
        throw new ArgumentNullException("contract");
      }

      ContractInfo = contract;
    }

    /// <summary>
    /// 通道已重新连接事件参数
    /// </summary>
    /// <param name="contract">契约信息</param>
    /// <param name="reason">连接断开原因</param>
    public ChannelDisconnectedEventArgs(ContractInfo contract, string reason)
      : this(contract)
    {
      Reason = reason;
    }

    /// <summary>
    /// 契约信息
    /// </summary>
    public ContractInfo ContractInfo { get; private set; }

    /// <summary>
    /// 连接断开原因
    /// </summary>
    public string Reason { get; private set; }
  }
}
