using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Gimela.Rukbat.Communication;
using Gimela.Rukbat.DPS.BusinessEntities;
using Gimela.Rukbat.MPS.Contracts.MessageContracts;
using Gimela.Rukbat.MPS.Contracts.ServiceContracts;

namespace Gimela.Rukbat.DPS.BusinessLogic
{
  public class CameraManager : ICameraManager
  {
    public IList<Camera> GetCameras()
    {
      List<Camera> cameras = new List<Camera>();
      Dictionary<Uri, ServiceProfile> services = ServiceProvider.GetServices<IMediaPublisherService>();

      foreach (var item in services)
      {
        IMediaPublisherService service = ServiceProvider.GetService<IMediaPublisherService>(item.Value.HostName, item.Value.Uri.ToString());

        GetPublishedCamerasRequest request = new GetPublishedCamerasRequest();
        GetPublishedCamerasResponse response = service.GetPublishedCameras(request);

        foreach (var publishedCamera in response.PublishedCameras)
        {
          Camera camera = new Camera()
          {
            Id = publishedCamera.Profile.CameraId,
            Name = publishedCamera.Profile.CameraName,
            Url = string.Format(CultureInfo.InvariantCulture, @"http://{0}:{1}", item.Value.HostName, publishedCamera.Destination.Port),
            Port = publishedCamera.Destination.Port,
          };
          cameras.Add(camera);
        }
      }

      return cameras;
    }

    public Camera GetCamera(string cameraId)
    {
      Dictionary<Uri, ServiceProfile> services = ServiceProvider.GetServices<IMediaPublisherService>();

      foreach (var item in services)
      {
        IMediaPublisherService service = ServiceProvider.GetService<IMediaPublisherService>(item.Value.HostName, item.Value.Uri.ToString());

        GetPublishedCamerasRequest request = new GetPublishedCamerasRequest();
        GetPublishedCamerasResponse response = service.GetPublishedCameras(request);

        var publishedCamera = response.PublishedCameras.FirstOrDefault(c => c.Profile.CameraId == cameraId);
        if (publishedCamera != null)
        {
          Camera camera = new Camera()
          {
            Id = publishedCamera.Profile.CameraId,
            Name = publishedCamera.Profile.CameraName,
            Url = string.Format(CultureInfo.InvariantCulture, @"http://{0}:{1}", item.Value.HostName, publishedCamera.Destination.Port),
            Port = publishedCamera.Destination.Port,
          };
          return camera;
        }
      }

      return null;
    }

    public byte[] GetCameraThumbnail(string cameraId)
    {
      Dictionary<Uri, ServiceProfile> services = ServiceProvider.GetServices<IMediaPublisherService>();

      foreach (var item in services)
      {
        IMediaPublisherService service = ServiceProvider.GetService<IMediaPublisherService>(item.Value.HostName, item.Value.Uri.ToString());

        GetPublishedCamerasRequest request = new GetPublishedCamerasRequest();
        GetPublishedCamerasResponse response = service.GetPublishedCameras(request);

        var publishedCamera = response.PublishedCameras.FirstOrDefault(c => c.Profile.CameraId == cameraId);
        if (publishedCamera != null)
        {
          return publishedCamera.Profile.CameraThumbnail;
        }
      }

      return null;
    }
  }
}
