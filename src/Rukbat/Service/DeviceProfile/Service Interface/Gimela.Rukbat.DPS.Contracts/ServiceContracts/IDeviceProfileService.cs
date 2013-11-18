using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Gimela.Rukbat.DPS.Contracts.FaultContracts;
using Gimela.Rukbat.DPS.Contracts.MessageContracts;
using Gimela.ServiceModel.ManagedService.Attributes;

namespace Gimela.Rukbat.DPS.Contracts.ServiceContracts
{
  /// <summary>
  /// 设备信息服务接口
  /// </summary>
  [ManagedServiceContractAttribute]
  [ServiceContract(SessionMode = SessionMode.Allowed)]
  public interface IDeviceProfileService
  {
    /// <summary>
    /// 获取全部摄像机的信息
    /// </summary>
    /// <param name="request">获取全部摄像机的请求信息</param>
    /// <returns>全部摄像机的响应信息</returns>
    [OperationContract]
    [FaultContract(typeof(DeviceProfileServiceFault))]
    GetCamerasResponse GetCameras(GetCamerasRequest request);

    /// <summary>
    /// 获取摄像机的信息
    /// </summary>
    /// <param name="request">获取摄像机的请求信息</param>
    /// <returns>摄像机的响应信息</returns>
    [OperationContract]
    [FaultContract(typeof(DeviceProfileServiceFault))]
    GetCameraResponse GetCamera(GetCameraRequest request);
  }
}
