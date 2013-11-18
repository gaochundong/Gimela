using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Gimela.Rukbat.MPS.BusinessEntities;

namespace Gimela.Rukbat.MPS.BusinessLogic
{
  /// <summary>
  /// 发布摄像机管理器
  /// </summary>
  public interface IPublishedCameraManager
  {
    /// <summary>
    /// 获取已发布的摄像机集合
    /// </summary>
    /// <returns>已发布的摄像机集合</returns>
    ReadOnlyCollection<PublishedCamera> GetPublishedCameras();
    /// <summary>
    /// 根据摄像机ID获取已发布的摄像机集合
    /// </summary>
    /// <param name="cameraId">摄像机ID</param>
    /// <returns>已发布的摄像机集合</returns>
    ReadOnlyCollection<PublishedCamera> GetPublishedCamerasById(string cameraId);
    /// <summary>
    /// 发布摄像机
    /// </summary>
    /// <param name="profile">被发布的摄像机的信息</param>
    /// <param name="destination">摄像机发布的目标地址</param>
    /// <returns>已发布摄像机</returns>
    PublishedCamera PublishCamera(PublishedCameraProfile profile, PublishedDestination destination);
    /// <summary>
    /// 取消发布摄像机
    /// </summary>
    /// <param name="cameraId">摄像机ID</param>
    /// <param name="destination">摄像机发布的目标地址</param>
    void UnpublishCamera(string cameraId, PublishedDestination destination);
    /// <summary>
    /// 取消发布摄像机
    /// </summary>
    /// <param name="cameraId">摄像机ID</param>
    void UnpublishCamera(string cameraId);
  }
}
