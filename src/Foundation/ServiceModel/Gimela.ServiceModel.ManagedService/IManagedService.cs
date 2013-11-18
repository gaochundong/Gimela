using System.ServiceModel;
using Gimela.ServiceModel.ManagedService.Contracts.MessageContracts;

namespace Gimela.ServiceModel.ManagedService
{
  /// <summary>
  /// 托管的服务接口
  /// </summary>
  [ServiceContract]
  public interface IManagedService : IHeartBeat
  {
    /// <summary>
    /// 获取托管服务状态
    /// </summary>
    /// <returns>托管服务状态响应</returns>
    [OperationContract]
    ServiceStateResponse GetState();

    /// <summary>
    /// 启动托管服务
    /// </summary>
    /// <returns>托管服务状态响应</returns>
    [OperationContract]
    ServiceStateResponse Start();

    /// <summary>
    /// 停止托管服务
    /// </summary>
    /// <returns>托管服务状态响应</returns>
    [OperationContract]
    ServiceStateResponse Stop();

    /// <summary>
    /// 重启托管服务
    /// </summary>
    /// <returns>托管服务状态响应</returns>
    [OperationContract]
    ServiceStateResponse Restart();
  }
}
