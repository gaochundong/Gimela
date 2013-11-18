using System.ServiceModel;
using Gimela.ServiceModel.ManagedService.Contracts.DataContracts;

namespace Gimela.ServiceModel.ManagedService.Contracts.MessageContracts
{
  /// <summary>
  /// Ping响应消息
  /// </summary>
  [MessageContract]
  public class PingResponse
  {
    /// <summary>
    /// 心跳状态
    /// </summary>
    [MessageBodyMember]
    public HeartBeatStatus Status { get; set; }

    /// <summary>
    /// 状态信息
    /// </summary>
    [MessageBodyMember]
    public string StatusMessage { get; set; }
  }
}
