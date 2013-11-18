using System.ServiceModel;
using Gimela.ServiceModel.ManagedService.Contracts.MessageContracts;

namespace Gimela.ServiceModel.ManagedService
{
  /// <summary>
  /// 心跳接口
  /// </summary>
  [ServiceContract]
  public interface IHeartBeat
  {
    /// <summary>
    /// Ping服务
    /// </summary>
    /// <param name="request">Ping请求消息</param>
    /// <returns>Ping响应消息</returns>
    [OperationContract]
    PingResponse Ping(PingRequest request);
  }
}
