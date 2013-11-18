using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Gimela.Rukbat.DVC.BusinessEntities;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public interface ICameraManager
  {
    bool IsCameraExist(string cameraId);
    Camera GetCamera(string cameraId);
    ReadOnlyCollection<Camera> GetCameras();
    Camera BuildCamera(CameraProfile profile, CameraConfig config, byte[] thumbnail);
    Camera CreateCamera(CameraProfile profile, CameraConfig config);
    Camera UpdateCamera(string cameraId, string cameraName, string description, string tags);
    void DeleteCamera(string cameraId);
    CameraStatus PingCamera(string cameraId);
  }
}
