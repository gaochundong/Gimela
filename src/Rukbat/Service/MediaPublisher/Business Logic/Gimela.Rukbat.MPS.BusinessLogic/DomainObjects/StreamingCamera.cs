using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using Gimela.Common.ExceptionHandling;
using Gimela.Management;
using Gimela.Media.Video;
using Gimela.Rukbat.Communication;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.DomainModels.MediaSource;
using Gimela.Rukbat.DVC.Contracts.DataContracts;
using Gimela.Rukbat.DVC.Contracts.MessageContracts;
using Gimela.Rukbat.DVC.Contracts.ServiceContracts;
using Gimela.Rukbat.MPS.BusinessEntities;
using Gimela.Rukbat.MPS.BusinessLogic.ServiceAgents;
using Gimela.Streaming.MJpegStreaming;

namespace Gimela.Rukbat.MPS.BusinessLogic.DomainObjects
{
  public class StreamingCamera
  {
    private readonly object _syncRoot = new object();
    private MJpegStreamingServer _streamingServer = null;
    private readonly object _syncSteamingFrame = new object();
    private Camera _device = null;
    private System.Threading.Timer _keepCameraAliveTimer;

    public StreamingCamera(PublishedCamera camera)
    {
      if (camera == null)
        throw new ArgumentNullException("camera");

      Camera = camera;

      _keepCameraAliveTimer = new System.Threading.Timer(KeepCameraAlive, null,
        TimeSpan.FromSeconds(0),
        TimeSpan.FromSeconds(4));
    }

    public string Id { get { return Camera.Id; } }

    public PublishedCamera Camera { get; private set; }

    public int Port { get { return Camera.Destination.Port; } }

    public bool IsStreaming { get; private set; }

    public void Start()
    {
      lock (_syncRoot)
      {
        if (IsStreaming) return;

        _streamingServer = new MJpegStreamingServer(Port);
        _streamingServer.Start();

        _device = GetCameraDevice();
        if (_device != null)
        {
          _device.NewFrameEvent += OnNewFrame;
          _device.Start();

          string address = LocalMachine.HostName;
          int port = int.Parse(_device.VideoSource.Source);
          StartStream(_device, address, port);
        }

        IsStreaming = true;
      }
    }

    public void Stop()
    {
      lock (_syncRoot)
      {
        if (!IsStreaming) return;

        if (_device != null)
        {
          try
          {
            string address = LocalMachine.HostName;
            int port = int.Parse(_device.VideoSource.Source);
            StopStream(_device, address, port);

            _device.NewFrameEvent -= OnNewFrame;
            _device.Stop();
          }
          catch { }
          _device = null;
        }

        if (_streamingServer != null)
        {
          try
          {
            _streamingServer.Stop();
          }
          catch { }
          _streamingServer = null;
        }

        IsStreaming = false;
      }
    }

    private void OnNewFrame(object sender, EventArgs e)
    {
      MJpegStreamingServer server = _streamingServer;
      if (server != null)
      {
        lock (_syncSteamingFrame)
        {
          if (_device != null)
          {
            Bitmap origin = _device.LastFrame;

            if (origin != null)
            {
              Bitmap frame = origin.Clone() as Bitmap;

              if (frame != null)
              {
                server.Write(frame);
              }
            }
          }
        }
      }
    }

    private void StartStream(Camera device, string address, int port)
    {
      PublishCameraSync(device, address, port);
    }

    private void StopStream(Camera device, string address, int port)
    {
      UnpublishCameraSync(device, address, port);
    }

    private void KeepCameraAlive(object state)
    {
      try
      {
        if (_device != null)
        {
          // 发送保活消息
          string address = LocalMachine.HostName;
          int port = int.Parse(_device.VideoSource.Source);
          KeepPublishedCameraAliveSync(_device, address, port);
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    private static Camera KeepPublishedCameraAliveSync(Camera camera, string address, int port)
    {
      MediaService service = GetDeviceConnectorServiceSync(camera.HostUri);

      if (service != null)
      {
        KeepPublishedCameraAliveRequest request = new KeepPublishedCameraAliveRequest()
        {
          CameraId = camera.Id,
          Destination = new PublishDestinationData()
          {
            Address = address,
            Port = port,
          },
        };
        KeepPublishedCameraAliveResponse response =
          ServiceProvider
            .GetService<IDeviceConnectorService, IDeviceConnectorCallbackService>(
              DeviceConnectorServiceClient.ServiceClient, service.HostName, service.Uri.ToString())
            .KeepPublishedCameraAlive(request);
      }

      return camera;
    }

    private static Camera PublishCameraSync(Camera camera, string address, int port)
    {
      MediaService service = GetDeviceConnectorServiceSync(camera.HostUri);

      if (service != null)
      {
        PublishCameraRequest request = new PublishCameraRequest()
        {
          CameraId = camera.Id,
          Destination = new PublishDestinationData()
          {
            Address = address,
            Port = port,
          },
        };
        PublishCameraResponse response =
          ServiceProvider
            .GetService<IDeviceConnectorService, IDeviceConnectorCallbackService>(
              DeviceConnectorServiceClient.ServiceClient, service.HostName, service.Uri.ToString())
            .PublishCamera(request);
      }

      return camera;
    }

    private static Camera UnpublishCameraSync(Camera camera, string address, int port)
    {
      MediaService service = GetDeviceConnectorServiceSync(camera.HostUri);

      if (service != null)
      {
        UnpublishCameraRequest request = new UnpublishCameraRequest()
        {
          CameraId = camera.Id,
          Destination = new PublishDestinationData()
          {
            Address = address,
            Port = port,
          },
        };
        UnpublishCameraResponse response =
          ServiceProvider
            .GetService<IDeviceConnectorService, IDeviceConnectorCallbackService>(
              DeviceConnectorServiceClient.ServiceClient, service.HostName, service.Uri.ToString())
            .UnpublishCamera(request);
      }

      return camera;
    }

    private Camera GetCameraDevice()
    {
      MediaService service = GetDeviceConnectorServiceSync(Camera.Profile.DeviceServiceUri);

      GetCameraRequest request = new GetCameraRequest()
      {
        CameraId = Camera.Profile.CameraId,
      };
      GetCameraResponse response =
        ServiceProvider
          .GetService<IDeviceConnectorService, IDeviceConnectorCallbackService>(
            DeviceConnectorServiceClient.ServiceClient, service.HostName, service.Uri.ToString())
          .GetCamera(request);

      Camera camera = null;

      if (response.Camera != null)
      {
        camera = new Camera()
        {
          Id = response.Camera.Profile.Id,
          Name = response.Camera.Profile.Name,
          Description = response.Camera.Profile.Description,
          Tags = response.Camera.Profile.Tags,
          Thumbnail = response.Camera.Thumbnail
        };

        // camera video source
        camera.VideoSourceDescription = new VideoSourceDescription()
        {
          FriendlyName = response.Camera.Config.FriendlyName,
          OriginalSourceString = response.Camera.Config.OriginalSourceString,
          SourceString = response.Camera.Config.SourceString,
          FrameInterval = response.Camera.Config.FrameInterval,
          FrameRate = response.Camera.Config.FrameRate,
          UserName = response.Camera.Config.UserName,
          Password = response.Camera.Config.Password,
          UserAgent = response.Camera.Config.UserAgent,
        };

        if (response.Camera.Config.Resolution != null)
        {
          camera.VideoSourceDescription.Resolution = new Resolution() { Width = response.Camera.Config.Resolution.Width, Height = response.Camera.Config.Resolution.Height };
        }

        // translate cameras came from remote server
        switch (response.Camera.Profile.FilterType)
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
      }

      return camera;
    }

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
  }
}
