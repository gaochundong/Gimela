using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Rukbat.DVC.BusinessEntities;
using Gimela.Rukbat.DVC.DataAccess;
using DACamera = Gimela.Rukbat.DVC.DataAccess.Models.Camera;
using DAPublishedCamera = Gimela.Rukbat.DVC.DataAccess.Models.PublishedCamera;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public class CameraManager : ICameraManager
  {
    private List<Camera> _cameras = new List<Camera>();
    private readonly object _accessLock = new object();

    #region ICameraManager Members

    public bool IsCameraExist(string cameraId)
    {
      bool isExist = false;
      Camera camera = _cameras.Find(c => c.Id == cameraId);
      if (camera != null)
        isExist = true;
      return isExist;
    }

    public Camera GetCamera(string cameraId)
    {
      Camera camera = _cameras.Find(c => c.Id == cameraId);
      if (camera == null)
      {
        throw new CameraNotFoundException(string.Format(CultureInfo.InvariantCulture, "Cannot find camera {0}.", cameraId));
      }
      return camera;
    }

    public ReadOnlyCollection<Camera> GetCameras()
    {
      return new ReadOnlyCollection<Camera>(_cameras);
    }

    public Camera BuildCamera(CameraProfile profile, CameraConfig config, byte[] thumbnail)
    {
      lock (_accessLock)
      {
        // 检查给定的源是否存在
        if (profile.FilterType == FilterType.LocalCamera
          || profile.FilterType == FilterType.LocalDesktop)
        {
          if (!Locator.Get<IFilterManager>().IsFilterExist(profile.FilterType, profile.FilterId))
          {
            throw new FilterNotFoundException(string.Format(CultureInfo.InvariantCulture, 
              "Cannot find filter by type [{0}] and id [{1}].", 
              profile.FilterType, profile.FilterId));
          }
        }

        Camera camera = _cameras.Find(c => c.Id == profile.Id);
        if (camera == null)
        {
          camera = CameraBuilder.Create(profile, config);
          _cameras.Add(camera);

          camera.Thumbnail = thumbnail;
        }

        return camera;
      }
    }

    public Camera CreateCamera(CameraProfile profile, CameraConfig config)
    {
      lock (_accessLock)
      {
        // 检查给定的源是否存在
        if (profile.FilterType == FilterType.LocalCamera
          || profile.FilterType == FilterType.LocalDesktop)
        {
          if (!Locator.Get<IFilterManager>().IsFilterExist(profile.FilterType, profile.FilterId))
          {
            throw new FilterNotFoundException(string.Format(CultureInfo.InvariantCulture,
              "Cannot find filter by type [{0}] and id [{1}].",
              profile.FilterType, profile.FilterId));
          }
        }
        else if (profile.FilterType == FilterType.LocalAVIFile)
        {
          if (!File.Exists(profile.FilterId))
          {
            throw new FilterNotFoundException(string.Format(CultureInfo.InvariantCulture,
              "Cannot find filter by type [{0}] and id [{1}].",
              profile.FilterType, profile.FilterId));
          }
        }

        // 构造摄像机
        Camera camera = _cameras.Find(c => c.Id == profile.Id);
        if (camera == null)
        {
          camera = CameraBuilder.Create(profile, config);
          _cameras.Add(camera);

          camera.Thumbnail = Locator.Get<IStreamingManager>().GetCameraSnapshot(camera.Id);

          Locator.Get<ICameraRepository>().Save(CameraBuilder.Translate(camera));
        }

        return camera;
      }
    }

    public Camera UpdateCamera(string cameraId, string cameraName, string description, string tags)
    {
      lock (_accessLock)
      {
        Camera camera = _cameras.Find(c => c.Id == cameraId);
        if (camera != null)
        {
          camera.Profile.Name = cameraName;
          camera.Profile.Description = description;
          camera.Profile.Tags = tags;
          Locator.Get<ICameraRepository>().Save(CameraBuilder.Translate(camera));
        }

        return camera;
      }
    }

    public void DeleteCamera(string cameraId)
    {
      lock (_accessLock)
      {
        if (Locator.Get<IStreamingManager>().IsCameraPublished(cameraId))
        {
          throw new CameraBusyException(string.Format(CultureInfo.InvariantCulture, "Cannot delete camera {0}, this camera has been published.", cameraId));
        }
        _cameras.RemoveAll(c => c.Id == cameraId);
        Locator.Get<ICameraRepository>().Remove(cameraId);
      }
    }

    public CameraStatus PingCamera(string cameraId)
    {
      CameraStatus status = CameraStatus.Offline;

      Camera camera = _cameras.Find(c => c.Id == cameraId);
      if (camera != null)
        status = CameraStatus.Online;

      return status;
    }

    #endregion
  }
}
