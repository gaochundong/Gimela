using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Rukbat.DPS.BusinessEntities;

namespace Gimela.Rukbat.DPS.BusinessLogic
{
  public interface ICameraManager
  {
    IList<Camera> GetCameras();
    Camera GetCamera(string cameraId);
    byte[] GetCameraThumbnail(string cameraId);
  }
}
