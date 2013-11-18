using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using Gimela.Crust.Tectosphere;
using Gimela.Common.ExceptionHandling;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Rukbat.Communication;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.DomainModels.MediaSource;
using Gimela.Rukbat.GUI.Modules.PublishMedia.Entities;
using Gimela.Rukbat.MPS.Contracts.DataContracts;
using Gimela.Rukbat.MPS.Contracts.MessageContracts;
using Gimela.Rukbat.MPS.Contracts.ServiceContracts;

namespace Gimela.Rukbat.GUI.Modules.PublishMedia.Models
{
  public class PublishedCameraModel : ModelBase
  {
    private static List<MediaService> GetMediaPublisherServicesSync()
    {
      List<MediaService> services = new List<MediaService>();

      foreach (var item in ServiceProvider.GetServices<IMediaPublisherService>())
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

    private static MediaService GetMediaPublisherServiceSync(string hostUri)
    {
      MediaService service = null;

      foreach (var item in ServiceProvider.GetServices<IMediaPublisherService>())
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

    public void GetPublishedCameras(EventHandler<AsyncWorkerCallbackEventArgs<IList<PublishedCamera>>> callback)
    {
      try
      {
        AsyncWorkerHandle<IList<PublishedCamera>> handle = AsyncWorkerHelper.DoWork<IList<PublishedCamera>>(
          delegate(object sender, DoWorkEventArgs e)
          {
            e.Result = GetPublishedCamerasSync();
          },
          null, callback);
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    private static List<PublishedCamera> GetPublishedCamerasSync()
    {
      List<MediaService> services = GetMediaPublisherServicesSync();

      List<PublishedCamera> cameras = new List<PublishedCamera>();

      foreach (var service in services)
      {
        GetPublishedCamerasRequest request = new GetPublishedCamerasRequest();
        GetPublishedCamerasResponse response =
          ServiceProvider
            .GetService<IMediaPublisherService>(service.HostName, service.Uri.ToString())
            .GetPublishedCameras(request);

        if (response.PublishedCameras != null)
        {
          foreach (var item in response.PublishedCameras)
          {
            PublishedCamera camera = new PublishedCamera(
              new PublishedCameraProfile(item.Profile.CameraId, item.Profile.CameraName)
              {
                CameraThumbnail = item.Profile.CameraThumbnail,
                DeviceServiceHostName = item.Profile.DeviceServiceHostName,
                DeviceServiceUri = item.Profile.DeviceServiceUri,
              },
              new PublishedDestination(item.Destination.Port))
            {
              PublishServiceHostName = service.HostName,
              PublishServiceUri = service.Uri.ToString(),
            };

            cameras.Add(camera);
          }
        }
      }

      return cameras;
    }

    public void PublishCamera(string hostUri, PublishedCameraProfile profile, PublishedDestination destination, EventHandler<AsyncWorkerCallbackEventArgs<bool>> callback)
    {
      try
      {
        AsyncWorkerHandle<bool> handle = AsyncWorkerHelper.DoWork<bool>(
          delegate(object sender, DoWorkEventArgs e)
          {
            e.Result = PublishCameraSync(hostUri, profile, destination);
          },
          null, callback);
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    private static bool PublishCameraSync(string hostUri, PublishedCameraProfile profile, PublishedDestination destination)
    {
      MediaService service = GetMediaPublisherServiceSync(hostUri);

      if (service != null)
      {
        PublishCameraRequest request = new PublishCameraRequest()
        {
          Profile = new PublishedCameraProfileData()
          {
            CameraId = profile.CameraId,
            CameraName = profile.CameraName,
            CameraThumbnail = profile.CameraThumbnail,
            DeviceServiceHostName = profile.DeviceServiceHostName,
            DeviceServiceUri = profile.DeviceServiceUri,
          },
          Destination = new PublishedDestinationData()
          {
            Port = destination.Port,
          },
        };
        PublishCameraResponse response =
          ServiceProvider
            .GetService<IMediaPublisherService>(service.HostName, service.Uri.ToString())
            .PublishCamera(request);
      }

      return true;
    }

    public void UnpublishCamera(string hostUri, PublishedCamera camera, EventHandler<AsyncWorkerCallbackEventArgs<bool>> callback)
    {
      try
      {
        AsyncWorkerHandle<bool> handle = AsyncWorkerHelper.DoWork<bool>(
          delegate(object sender, DoWorkEventArgs e)
          {
            e.Result = UnpublishCameraSync(hostUri, camera);
          },
          null, callback);
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    private static bool UnpublishCameraSync(string hostUri, PublishedCamera camera)
    {
      MediaService service = GetMediaPublisherServiceSync(hostUri);

      if (service != null)
      {
        UnpublishCameraRequest request = new UnpublishCameraRequest()
        {
          CameraId = camera.Profile.CameraId,
          Destination = new PublishedDestinationData()
          {
            Port = camera.Destination.Port,
          },
        };
        UnpublishCameraResponse response =
          ServiceProvider
            .GetService<IMediaPublisherService>(service.HostName, service.Uri.ToString())
            .UnpublishCamera(request);
      }

      return true;
    }

    public void CheckPortAvailable(string hostUri, int port, EventHandler<AsyncWorkerCallbackEventArgs<bool>> callback)
    {
      if (port <= 0)
        throw new ArgumentNullException("port");

      try
      {
        AsyncWorkerHandle<bool> handle = AsyncWorkerHelper.DoWork<bool>(
          delegate(object sender, DoWorkEventArgs e)
          {
            e.Result = true; // check here, true is available, false is not available

            MediaService service = GetMediaPublisherServiceSync(hostUri);

            GetPublishedCamerasRequest request = new GetPublishedCamerasRequest();
            GetPublishedCamerasResponse response =
              ServiceProvider
                .GetService<IMediaPublisherService>(service.HostName, service.Uri.ToString())
                .GetPublishedCameras(request);

            if (response.PublishedCameras != null)
            {
              foreach (var item in response.PublishedCameras)
              {
                if (item.Destination.Port == port)
                {
                  e.Result = false; // find this port is in using, so it's not available
                  break;
                }
              }
            }
          },
          null, callback);
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }
  }
}
