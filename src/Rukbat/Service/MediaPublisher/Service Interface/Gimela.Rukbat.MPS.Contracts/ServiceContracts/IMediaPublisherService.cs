using System;
using System.Net.Security;
using System.ServiceModel;
using Gimela.ServiceModel.ManagedService.Attributes;
using Gimela.Rukbat.MPS.Contracts.FaultContracts;
using Gimela.Rukbat.MPS.Contracts.MessageContracts;

namespace Gimela.Rukbat.MPS.Contracts.ServiceContracts
{
  [ManagedServiceContractAttribute]
  [ServiceContract(SessionMode = SessionMode.Allowed)]
  public interface IMediaPublisherService
  {
    /// <summary>
    /// 获取已发布摄像机的信息
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(MediaPublisherServiceFault))]
    GetPublishedCamerasResponse GetPublishedCameras(GetPublishedCamerasRequest request);

    /// <summary>
    /// 发布摄像机视频流至指定接收器
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(MediaPublisherServiceFault))]
    PublishCameraResponse PublishCamera(PublishCameraRequest request);

    /// <summary>
    /// 取消发布摄像机视频流
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(MediaPublisherServiceFault))]
    UnpublishCameraResponse UnpublishCamera(UnpublishCameraRequest request);
  }
}
