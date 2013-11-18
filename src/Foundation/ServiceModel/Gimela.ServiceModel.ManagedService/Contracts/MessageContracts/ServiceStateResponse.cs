using System.ServiceModel;
using Gimela.ServiceModel.ManagedService.Contracts.DataContracts;

namespace Gimela.ServiceModel.ManagedService.Contracts.MessageContracts
{
  /// <summary>
  /// 服务状态响应消息
  /// </summary>
  [MessageContract]
  public class ServiceStateResponse
  {
    /// <summary>
    /// 服务状态
    /// </summary>
    [MessageBodyMember]
    public ServiceState State { get; set; }

    /// <summary>
    /// 服务状态信息
    /// </summary>
    [MessageBodyMember]
    public string StateMessage { get; set; }
  }
}
