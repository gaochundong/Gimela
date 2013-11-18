using System.ServiceModel;
using Gimela.Rukbat.DVC.Contracts.CallbackContracts;

namespace Gimela.Rukbat.DVC.Contracts.ServiceContracts
{
  public interface IDeviceConnectorCallbackService
  {
    /// <summary>
    /// 通知摄像机已创建
    /// </summary>
    /// <param name="request"></param>
    [OperationContract(IsOneWay = true)]
    void NotifyCameraCreated(NotifyCameraCreatedRequest request);

    /// <summary>
    /// 通知摄像机已删除
    /// </summary>
    /// <param name="request"></param>
    [OperationContract(IsOneWay = true)]
    void NotifyCameraDeleted(NotifyCameraDeletedRequest request);

    /// <summary>
    /// 通知摄像机已发布
    /// </summary>
    /// <param name="request"></param>
    [OperationContract(IsOneWay = true)]
    void NotifyCameraPublished(NotifyCameraPublishedRequest request);

    /// <summary>
    /// 通知摄像机已取消发布
    /// </summary>
    /// <param name="request"></param>
    [OperationContract(IsOneWay = true)]
    void NotifyCameraUnpublished(NotifyCameraUnpublishedRequest request);
  }
}
