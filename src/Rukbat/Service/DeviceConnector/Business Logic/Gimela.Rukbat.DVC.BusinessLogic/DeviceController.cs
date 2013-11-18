using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Media.Video;
using Gimela.Rukbat.DVC.BusinessEntities;
using Gimela.Rukbat.DVC.DataAccess;
using CameraData = Gimela.Rukbat.DVC.DataAccess.Models.Camera;
using PublishedCameraData = Gimela.Rukbat.DVC.DataAccess.Models.PublishedCamera;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public class DeviceController : IDeviceController
  {
    #region IDeviceController Members

    public void Start()
    {
      LoadCamera();

      Locator.Get<IFilterManager>().FilterRemoved += new EventHandler<FilterRemovedEventArgs>(OnFilterRemoved);
    }

    public void Stop()
    {
      Locator.Get<IFilterManager>().FilterRemoved -= new EventHandler<FilterRemovedEventArgs>(OnFilterRemoved);
    }

    #endregion

    #region Private Methods

    private static void LoadCamera()
    {
      // get local filters
      List<DesktopFilter> desktopFilters = Locator.Get<IFilterManager>().GetDesktopFilters().ToList();
      List<CameraFilter> cameraFilters = Locator.Get<IFilterManager>().GetCameraFilters().ToList();

      // get cameras from db and published status
      List<CameraData> cameraDatas = Locator.Get<ICameraRepository>().FindAll().ToList();
      List<PublishedCameraData> publishedCameraDatas = Locator.Get<IPublishedCameraRepository>().FindAll().ToList();

      // matching local filters and db cameras
      foreach (var camera in cameraDatas)
      {
        if ((FilterType)camera.Profile.FilterType == FilterType.LocalDesktop
          || (FilterType)camera.Profile.FilterType == FilterType.LocalCamera)
        {
          IFilter filter = null;
          if ((FilterType)camera.Profile.FilterType == FilterType.LocalDesktop)
          {
            filter = desktopFilters.FirstOrDefault(d => d.Id == camera.Profile.FilterId);
          }
          else if ((FilterType)camera.Profile.FilterType == FilterType.LocalCamera)
          {
            filter = cameraFilters.FirstOrDefault(d => d.Id == camera.Profile.FilterId);
          }

          if (filter != null)
          {
            CameraProfile profile = CameraBuilder.FromFilter(camera.Id, filter);
            profile.Name = camera.Profile.Name;
            profile.Description = camera.Profile.Description;
            profile.Tags = camera.Profile.Tags;

            Locator.Get<ICameraManager>().BuildCamera(
              profile, 
              CameraBuilder.Translate(camera.Config), 
              camera.Thumbnail);
          }
          else
          {
            // camera filter is not found, maybe it has been removed, this camera is invalid
            Locator.Get<ICameraRepository>().Remove(camera.Id);
          }
        }
        else
        {
          // AVIFile, JPEG, MJPEG
          Locator.Get<ICameraManager>().BuildCamera(
            CameraBuilder.Translate(camera.Profile), 
            CameraBuilder.Translate(camera.Config), 
            camera.Thumbnail);
        }
      }

      foreach (var item in publishedCameraDatas)
      {
        Camera camera = Locator.Get<ICameraManager>().GetCamera(item.Id);
        if (camera != null)
        {
          if (item.Destinations != null && item.Destinations.Count > 0)
          {
            Locator.Get<IStreamingManager>().PublishCamera(camera.Id,
              (from d in item.Destinations select new PublishDestination(d.Address, d.Port)).ToList());
          }
        }
        else
        {
          // camera filter is not found, maybe it has been removed, this camera is invalid
          Locator.Get<IPublishedCameraRepository>().Remove(item.Id);
        }
      }
    }

    private void OnFilterRemoved(object sender, FilterRemovedEventArgs e)
    {
      var query = from c in Locator.Get<ICameraManager>().GetCameras()
                  where c.Profile.FilterType == e.FilterType && c.Profile.FilterId == e.FilterId
                  select c;
      foreach (var camera in query)
      {
        Locator.Get<ICameraManager>().DeleteCamera(camera.Id);
      }
    }

    #endregion
  }
}
