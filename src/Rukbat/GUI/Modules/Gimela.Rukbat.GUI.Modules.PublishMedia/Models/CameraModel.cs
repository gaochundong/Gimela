using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using Gimela.Crust.Tectosphere;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Common.ExceptionHandling;
using Gimela.Common.Logging;
using Gimela.Rukbat.DVC.Contracts.DataContracts;
using Gimela.Rukbat.DVC.Contracts.MessageContracts;
using Gimela.Rukbat.DVC.Contracts.ServiceContracts;
using Gimela.Rukbat.Communication;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.DomainModels.MediaSource;

namespace Gimela.Rukbat.GUI.Modules.PublishMedia.Models
{
  public class CameraModel : ModelBase
  {
    private static List<MediaService> GetDeviceConnectorServicesSync()
    {
      List<MediaService> services = new List<MediaService>();

      foreach (var item in ServiceProvider.GetServices<IDeviceConnectorService>())
      {
        services.Add(new MediaService()
        {
          Id = item.Value.Uri.ToString(),
          Name = item.Value.Name,
          ContractName = item.Value.Name,
          HostName = item.Value.HostName,
          Uri = item.Value.Uri,
        });
      }

      return services;
    }

    private static MediaService GetDeviceConnectorServiceSync(string hostUri)
    {
      MediaService service = null;

      foreach (var item in ServiceProvider.GetServices<IDeviceConnectorService>())
      {
        if (item.Value.Uri.ToString() == hostUri)
        {
          service = new MediaService()
          {
            Id = item.Value.Uri.ToString(),
            Name = item.Value.Name,
            ContractName = item.Value.Name,
            HostName = item.Value.HostName,
            Uri = item.Value.Uri,
          };
        }
      }

      return service;
    }

    public void GetCameras(EventHandler<AsyncWorkerCallbackEventArgs<IList<Camera>>> callback)
    {
      try
      {
        AsyncWorkerHandle<IList<Camera>> handle = AsyncWorkerHelper.DoWork<IList<Camera>>(
          delegate(object sender, DoWorkEventArgs e)
          {
            e.Result = GetCamerasSync();
          },
          null, callback);
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    public void SearchCameras(string searchText, EventHandler<AsyncWorkerCallbackEventArgs<IList<Camera>>> callback)
    {
      try
      {
        AsyncWorkerHandle<IList<Camera>> handle = AsyncWorkerHelper.DoWork<IList<Camera>>(
          delegate(object sender, DoWorkEventArgs e)
          {
            List<Camera> matches = new List<Camera>();

            foreach (var item in GetCamerasSync())
            {
              if (string.IsNullOrEmpty(searchText) || item.Name.Contains(searchText))
              {
                matches.Add(item as Camera);
              }
            }

            e.Result = matches;
          },
          null, callback);
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    private static List<Camera> GetCamerasSync()
    {
      List<MediaService> services = GetDeviceConnectorServicesSync();

      List<Camera> cameras = new List<Camera>();

      foreach (var service in services)
      {
        GetCamerasRequest request = new GetCamerasRequest();
        GetCamerasResponse response =
          ServiceProvider
            .GetService<IDeviceConnectorService, IDeviceConnectorCallbackService>(
              ViewModelLocator.ServiceClient, service.HostName, service.Uri.ToString())
            .GetCameras(request);

        if (response.Cameras != null)
        {
          foreach (var item in response.Cameras)
          {
            Camera camera = new Camera()
            {
              Id = item.Profile.Id,
              Name = item.Profile.Name,
              Description = item.Profile.Description,
              Tags = item.Profile.Tags,
              Thumbnail = item.Thumbnail
            };

            // camera video source
            camera.VideoSourceDescription = new VideoSourceDescription()
            {
              FriendlyName = item.Config.FriendlyName,
              OriginalSourceString = item.Config.OriginalSourceString,
              SourceString = item.Config.SourceString,
              FrameInterval = item.Config.FrameInterval,
              FrameRate = item.Config.FrameRate,
              UserName = item.Config.UserName,
              Password = item.Config.Password,
              UserAgent = item.Config.UserAgent,
            };

            if (item.Config.Resolution != null)
            {
              camera.VideoSourceDescription.Resolution = new Resolution() { Width = item.Config.Resolution.Width, Height = item.Config.Resolution.Height };
            }

            // translate cameras came from remote server
            switch (item.Profile.FilterType)
            {
              case FilterTypeData.LocalCamera:
              case FilterTypeData.LocalDesktop:
              case FilterTypeData.LocalAVIFile:
              case FilterTypeData.NetworkJPEG:
              case FilterTypeData.NetworkMJPEG:
                camera.VideoSourceDescription.SourceType = VideoSourceType.NetworkRtpStream;
                break;
              default:
                break;
            }

            // where is this camera
            camera.HostName = service.HostName;
            camera.HostUri = service.Uri.ToString();

            cameras.Add(camera);
          }
        }
      }

      return cameras;
    }
  }
}
