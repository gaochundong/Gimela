using System;
using System.Net.Security;
using System.ServiceModel;
using Gimela.ServiceModel.ManagedService.Attributes;
using Gimela.Rukbat.DVC.Contracts.FaultContracts;
using Gimela.Rukbat.DVC.Contracts.MessageContracts;

namespace Gimela.Rukbat.DVC.Contracts.ServiceContracts
{
  [ManagedServiceContractAttribute]
  [ServiceContract(SessionMode = SessionMode.Allowed, CallbackContract = typeof(IDeviceConnectorCallbackService))]
  public interface IDeviceConnectorService
  {
    /// <summary>
    /// 获取摄像机源列表
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    GetCameraFiltersResponse GetCameraFilters(GetCameraFiltersRequest request);
    
    /// <summary>
    /// 获取桌面源列表
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    GetDesktopFiltersResponse GetDesktopFilters(GetDesktopFiltersRequest request);

    /// <summary>
    /// 获取指定的摄像机
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    GetCameraResponse GetCamera(GetCameraRequest request);

    /// <summary>
    /// 获取全部的摄像机
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    GetCamerasResponse GetCameras(GetCamerasRequest request);

    /// <summary>
    /// 创建摄像机
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    CreateCameraResponse CreateCamera(CreateCameraRequest request);

    /// <summary>
    /// 修改摄像机
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    UpdateCameraResponse UpdateCamera(UpdateCameraRequest request);

    /// <summary>
    /// 删除摄像机
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    DeleteCameraResponse DeleteCamera(DeleteCameraRequest request);

    /// <summary>
    /// 检测摄像机
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    PingCameraResponse PingCamera(PingCameraRequest request);

    /// <summary>
    /// 获取摄像机最新的快照图像
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    GetCameraSnapshotResponse GetCameraSnapshot(GetCameraSnapshotRequest request);

    /// <summary>
    /// 获取已发布摄像机的信息
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    GetPublishedCamerasResponse GetPublishedCameras(GetPublishedCamerasRequest request);

    /// <summary>
    /// 发布摄像机视频流至指定接收器
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    PublishCameraResponse PublishCamera(PublishCameraRequest request);

    /// <summary>
    /// 取消发布摄像机视频流
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    UnpublishCameraResponse UnpublishCamera(UnpublishCameraRequest request);

    /// <summary>
    /// 发布的摄像机保活机制
    /// </summary>
    /// <param name="request">摄像机保活机制请求消息</param>
    /// <returns>摄像机保活机制响应消息</returns>
    [OperationContract]
    [FaultContract(typeof(DeviceConnectorServiceFault))]
    KeepPublishedCameraAliveResponse KeepPublishedCameraAlive(KeepPublishedCameraAliveRequest request);
  }
}
