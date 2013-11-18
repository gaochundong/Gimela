using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using Gimela.Crust.Tectosphere;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Common.ExceptionHandling;
using Gimela.Rukbat.DVC.Contracts.CallbackContracts;
using Gimela.Rukbat.DVC.Contracts.MessageContracts;
using Gimela.Rukbat.DVC.Contracts.ServiceContracts;
using Gimela.Rukbat.Communication;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.DVC.Contracts.DataContracts;
using Gimela.Rukbat.DomainModels.MediaSource;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Models
{
  public class CameraModel : ModelBase
  {
    public void GetCameras(EventHandler<AsyncWorkerCallbackEventArgs<IList<Camera>>> callback)
    {
      try
      {
        AsyncWorkerHandle<IList<Camera>> handle = AsyncWorkerHelper.DoWork<IList<Camera>>(
          delegate(object sender, DoWorkEventArgs e)
          {
            List<Camera> cameras = new List<Camera>();

            GetCamerasRequest request = new GetCamerasRequest();
            GetCamerasResponse response =
              ServiceProvider.GetService<IDeviceConnectorService, IDeviceConnectorCallbackService>(
              ViewModelLocator.ServiceClient,
              ViewModelLocator.SelectedService.HostName,
              ViewModelLocator.SelectedService.Uri.ToString()
              ).GetCameras(request);

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

                switch (item.Profile.FilterType)
                {
                  case FilterTypeData.LocalCamera:
                    camera.VideoSourceDescription.SourceType = VideoSourceType.LocalCamera;
                    break;
                  case FilterTypeData.LocalDesktop:
                    camera.VideoSourceDescription.SourceType = VideoSourceType.LocalDesktop;
                    break;
                  case FilterTypeData.LocalAVIFile:
                    camera.VideoSourceDescription.SourceType = VideoSourceType.LocalAVIFile;
                    break;
                  case FilterTypeData.NetworkJPEG:
                    camera.VideoSourceDescription.SourceType = VideoSourceType.NetworkJPEG;
                    break;
                  case FilterTypeData.NetworkMJPEG:
                    camera.VideoSourceDescription.SourceType = VideoSourceType.NetworkMJPEG;
                    break;
                  default:
                    break;
                }

                camera.HostName = ViewModelLocator.SelectedService.HostName;
                camera.HostUri = ViewModelLocator.SelectedService.Uri.ToString();

                cameras.Add(camera);
              }
            }

            e.Result = cameras;
          },
          null, callback);
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    public void CreateCamera(Camera camera, EventHandler<AsyncWorkerCallbackEventArgs<Camera>> callback)
    {
      if (camera == null)
        throw new ArgumentNullException("camera");

      try
      {
        AsyncWorkerHandle<Camera> handle = AsyncWorkerHelper.DoWork<Camera>(
          delegate(object sender, DoWorkEventArgs e)
          {
            CreateCameraRequest request = new CreateCameraRequest();
            request.Camera = new CameraData();

            request.Camera.Profile = new CameraProfileData();
            request.Camera.Profile.Id = camera.Id;
            request.Camera.Profile.Name = camera.Name;
            request.Camera.Profile.Description = camera.Description;
            request.Camera.Profile.Tags = camera.Tags;

            request.Camera.Profile.FilterType = FilterTypeData.None;
            switch (camera.VideoSourceDescription.SourceType)
            {
              case VideoSourceType.Mock:
                break;
              case VideoSourceType.LocalAVIFile:
                request.Camera.Profile.FilterType = FilterTypeData.LocalAVIFile;
                request.Camera.Profile.FilterId = camera.VideoSourceDescription.OriginalSourceString;
                break;
              case VideoSourceType.LocalCamera:
                request.Camera.Profile.FilterType = FilterTypeData.LocalCamera;
                request.Camera.Profile.FilterId = camera.VideoSourceDescription.OriginalSourceString;
                break;
              case VideoSourceType.LocalDesktop:
                request.Camera.Profile.FilterType = FilterTypeData.LocalDesktop;
                request.Camera.Profile.FilterId = camera.VideoSourceDescription.OriginalSourceString;
                break;
              case VideoSourceType.NetworkJPEG:
                request.Camera.Profile.FilterType = FilterTypeData.NetworkJPEG;
                request.Camera.Profile.FilterId = camera.VideoSourceDescription.OriginalSourceString;
                break;
              case VideoSourceType.NetworkMJPEG:
                request.Camera.Profile.FilterType = FilterTypeData.NetworkMJPEG;
                request.Camera.Profile.FilterId = camera.VideoSourceDescription.OriginalSourceString;
                break;
              default:
                break;
            }

            request.Camera.Config = new CameraConfigData()
            {
              FriendlyName = camera.VideoSourceDescription.FriendlyName,
              OriginalSourceString = camera.VideoSourceDescription.OriginalSourceString,
              SourceString = camera.VideoSourceDescription.SourceString,
              FrameInterval = camera.VideoSourceDescription.FrameInterval,
              FrameRate = camera.VideoSourceDescription.FrameRate,
              UserName = camera.VideoSourceDescription.UserName,
              Password = camera.VideoSourceDescription.Password,
              UserAgent = camera.VideoSourceDescription.UserAgent,
            };

            if (camera.VideoSourceDescription.Resolution != null)
            {
              request.Camera.Config.Resolution = new ResolutionData()
              {
                Width = camera.VideoSourceDescription.Resolution.Width,
                Height = camera.VideoSourceDescription.Resolution.Height
              };
            }

            // 默认请求超时时间为1分钟
            CreateCameraResponse response =
              ServiceProvider.GetService<IDeviceConnectorService, IDeviceConnectorCallbackService>(
              ViewModelLocator.ServiceClient,
              ViewModelLocator.SelectedService.HostName,
              ViewModelLocator.SelectedService.Uri.ToString()
              ).CreateCamera(request);
            if (response.Camera != null)
            {
              e.Result = camera;
            }
          },
          null, callback);
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    public void UpdateCamera(Camera camera, EventHandler<AsyncWorkerCallbackEventArgs<Camera>> callback)
    {
      if (camera == null)
        throw new ArgumentNullException("camera");

      try
      {
        AsyncWorkerHandle<Camera> handle = AsyncWorkerHelper.DoWork<Camera>(
          delegate(object sender, DoWorkEventArgs e)
          {
            UpdateCameraRequest request = new UpdateCameraRequest();
            request.CameraId = camera.Id;
            request.CameraName = camera.Name;
            request.Description = camera.Description;
            request.Tags = camera.Tags;

            UpdateCameraResponse response =
              ServiceProvider.GetService<IDeviceConnectorService, IDeviceConnectorCallbackService>(
              ViewModelLocator.ServiceClient,
              ViewModelLocator.SelectedService.HostName,
              ViewModelLocator.SelectedService.Uri.ToString()
              ).UpdateCamera(request);
            if (response.Camera != null)
            {
              e.Result = camera;
            }
          },
          null, callback);
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    public void DeleteCamera(Camera camera, EventHandler<AsyncWorkerCallbackEventArgs<bool>> callback)
    {
      if (camera == null)
        throw new ArgumentNullException("camera");

      try
      {
        AsyncWorkerHandle<bool> handle = AsyncWorkerHelper.DoWork<bool>(
          delegate(object sender, DoWorkEventArgs e)
          {
            DeleteCameraRequest request = new DeleteCameraRequest();
            request.CameraId = camera.Id;

            DeleteCameraResponse response =
              ServiceProvider.GetService<IDeviceConnectorService, IDeviceConnectorCallbackService>(
              ViewModelLocator.ServiceClient,
              ViewModelLocator.SelectedService.HostName,
              ViewModelLocator.SelectedService.Uri.ToString()
              ).DeleteCamera(request);

            e.Result = true;
          },
          null, callback);
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    public void CheckCameraName(Camera camera, EventHandler<AsyncWorkerCallbackEventArgs<bool>> callback)
    {
      if (camera == null)
        throw new ArgumentNullException("camera");

      try
      {
        AsyncWorkerHandle<bool> handle = AsyncWorkerHelper.DoWork<bool>(
          delegate(object sender, DoWorkEventArgs e)
          {
            e.Result = true; // check here, true is available, false is in using

            GetCamerasRequest request = new GetCamerasRequest();
            GetCamerasResponse response =
              ServiceProvider.GetService<IDeviceConnectorService, IDeviceConnectorCallbackService>(
              ViewModelLocator.ServiceClient,
              ViewModelLocator.SelectedService.HostName,
              ViewModelLocator.SelectedService.Uri.ToString()
              ).GetCameras(request);
            if (response.Cameras != null)
            {
              foreach (var item in response.Cameras)
              {
                if (item.Profile.Name == camera.Name)
                {
                  e.Result = false;
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
