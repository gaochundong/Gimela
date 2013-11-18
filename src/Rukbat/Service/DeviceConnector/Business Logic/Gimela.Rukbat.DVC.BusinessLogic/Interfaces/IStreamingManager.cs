using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Gimela.Rukbat.DVC.BusinessEntities;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public interface IStreamingManager
  {
    byte[] GetCameraSnapshot(string cameraId);
    bool IsCameraPublished(string cameraId);
    PublishedCamera GetPublishedCamera(string cameraId);
    ReadOnlyCollection<PublishedCamera> GetPublishedCameras();
    PublishedCamera PublishCamera(string cameraId, PublishDestination destination);
    PublishedCamera PublishCamera(string cameraId, ICollection<PublishDestination> destinations);
    void UnpublishCamera(string cameraId, PublishDestination destination);
    void UnpublishCamera(string cameraId, ICollection<PublishDestination> destinations);
    void UnpublishCamera(string cameraId);
    void KeepAlive(string cameraId, PublishDestination destination);
  }
}
