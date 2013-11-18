using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Gimela.Common.ExceptionHandling;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Media.Video;
using Gimela.Rukbat.MPS.BusinessEntities;
using Gimela.Rukbat.MPS.DataAccess;
using DA = Gimela.Rukbat.MPS.DataAccess.Models;

namespace Gimela.Rukbat.MPS.BusinessLogic
{
  public class PublishedCameraManager : IPublishedCameraManager
  {
    private readonly object _syncRoot = new object();
    private List<PublishedCamera> _cameras = new List<PublishedCamera>();

    public PublishedCameraManager()
    {
    }

    #region IPublishedCameraManager Members

    public ReadOnlyCollection<PublishedCamera> GetPublishedCameras()
    {
      lock (_syncRoot)
      {
        return new ReadOnlyCollection<PublishedCamera>(_cameras);
      }
    }

    public ReadOnlyCollection<PublishedCamera> GetPublishedCamerasById(string cameraId)
    {
      lock (_syncRoot)
      {
        return new ReadOnlyCollection<PublishedCamera>(_cameras.Where(c => c.Profile.CameraId == cameraId).ToList());
      }
    }

    public PublishedCamera PublishCamera(PublishedCameraProfile profile, PublishedDestination destination)
    {
      lock (_syncRoot)
      {
        if (profile == null)
          throw new ArgumentNullException("profile");
        if (destination == null)
          throw new ArgumentNullException("destination");

        PublishedCamera camera = _cameras.Find(c => c.Profile.CameraId == profile.CameraId && c.Destination == destination);
        if (camera == null)
        {
          camera = new PublishedCamera(profile, destination);
          Locator.Get<IStreamingManager>().StartCameraStreaming(camera);
          _cameras.Add(camera);

          DA::PublishedCamera entity = new DA::PublishedCamera()
          {
            Id = camera.Id,
            Profile = new DA::PublishedCameraProfile()
            {
              CameraId = profile.CameraId,
              CameraName = profile.CameraName,
              CameraThumbnail = profile.CameraThumbnail,
              DeviceServiceHostName = profile.DeviceServiceHostName,
              DeviceServiceUri = profile.DeviceServiceUri,
            },
            Destination = new DA::Destination()
            {
              Port = camera.Destination.Port,
            },
          };
          Locator.Get<IPublishedCameraRepository>().Save(entity);
        }

        return camera;
      }
    }

    public void UnpublishCamera(string cameraId, PublishedDestination destination)
    {
      lock (_syncRoot)
      {
        if (destination == null)
          throw new ArgumentNullException("destination");

        PublishedCamera camera = _cameras.Find(c => c.Profile.CameraId == cameraId && c.Destination.Port == destination.Port);
        if (camera != null)
        {
          Locator.Get<IStreamingManager>().StopCameraStreaming(camera);
          Locator.Get<IPublishedCameraRepository>().Remove(camera.Id);
          _cameras.Remove(camera);
        }
      }
    }

    public void UnpublishCamera(string cameraId)
    {
      lock (_syncRoot)
      {
        foreach (var camera in _cameras.Where(c => c.Profile.CameraId == cameraId).ToList())
        {
          Locator.Get<IStreamingManager>().StopCameraStreaming(camera);
          Locator.Get<IPublishedCameraRepository>().Remove(camera.Id);
        }
        _cameras.RemoveAll(c => c.Profile.CameraId == cameraId);
      }
    }

    #endregion

    public PublishedCameraManager Restore()
    {
      lock (_syncRoot)
      {
        List<DA::PublishedCamera> entities = Locator.Get<IPublishedCameraRepository>().FindAll().ToList();
        foreach (var entity in entities)
        {
          PublishedCamera camera = new PublishedCamera(
            new PublishedCameraProfile(entity.Profile.CameraId, entity.Profile.CameraName)
            {
              CameraThumbnail = entity.Profile.CameraThumbnail,
              DeviceServiceHostName = entity.Profile.DeviceServiceHostName,
              DeviceServiceUri = entity.Profile.DeviceServiceUri,
            },
            new PublishedDestination(entity.Destination.Port));
          _cameras.Add(camera);
          Locator.Get<IStreamingManager>().StartCameraStreaming(camera);
        }

        return this;
      }
    }
  }
}
