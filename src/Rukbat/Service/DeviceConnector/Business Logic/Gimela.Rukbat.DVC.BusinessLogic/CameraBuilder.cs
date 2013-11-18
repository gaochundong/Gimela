using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Gimela.Data.Json;
using Gimela.Media.Video;
using Gimela.Media.Video.DirectShow;
using Gimela.Rukbat.DVC.BusinessEntities;
using DACameraConfig = Gimela.Rukbat.DVC.DataAccess.Models.CameraConfig;
using DACameraProfile = Gimela.Rukbat.DVC.DataAccess.Models.CameraProfile;
using DACamera = Gimela.Rukbat.DVC.DataAccess.Models.Camera;
using DAResolution = Gimela.Rukbat.DVC.DataAccess.Models.Resolution;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  internal static class CameraBuilder
  {
    internal static CameraProfile FromFilter(string cameraId, IFilter filter)
    {
      return new CameraProfile(cameraId, filter.Type, filter.Id);
    }

    internal static Camera Create(CameraProfile profile, CameraConfig config, IVideoSource videoSource)
    {
      return new Camera(profile, config, videoSource);
    }

    internal static Camera Create(CameraProfile profile, CameraConfig config)
    {
      switch (profile.FilterType)
      {
        case FilterType.LocalCamera:
          return Create(profile, config,
            new VideoCaptureDeviceVideoSource(config.OriginalSourceString)
            {
              DesiredFrameRate = 10000000 / config.FrameRate, // 1 fps, 30 fps = 10000000 / 30 = 333333
              DesiredFrameSize = new System.Drawing.Size(config.Resolution.Width, config.Resolution.Height),
              DesiredSnapshotSize = new System.Drawing.Size(config.Resolution.Width, config.Resolution.Height),
            });
        case FilterType.LocalDesktop:
          return Create(profile, config,
            new DesktopVideoSource(int.Parse(config.OriginalSourceString, CultureInfo.InvariantCulture))
            {
              FrameInterval = config.FrameInterval, // default 1000 == 1 fps, 30 fps = 1000 / 30 = 33
              IsResized = true,
              ResizeWidth = config.Resolution.Width,
              ResizeHeight = config.Resolution.Height,
            });
        case FilterType.LocalAVIFile:
          return Create(profile, config,
            new FileVideoSource(config.OriginalSourceString));
        case FilterType.NetworkJPEG:
          return Create(profile, config,
            new JpegVideoSource(config.OriginalSourceString)
            {
              FrameInterval = config.FrameInterval,
              HttpUserAgent = config.UserAgent,
              Login = config.UserName,
              Password = config.Password,
            });
        case FilterType.NetworkMJPEG:
          return Create(profile, config,
            new MJpegVideoSource(config.OriginalSourceString)
            {
              HttpUserAgent = config.UserAgent,
              Login = config.UserName,
              Password = config.Password,
            });
        default:
          throw new NotSupportedException("Do not support the given filter type " + profile.FilterType.ToString());
      }
    }

    internal static CameraProfile Translate(DACameraProfile data)
    {
      CameraProfile profile = new CameraProfile(data.Id, (FilterType)data.FilterType, data.FilterId)
      {
        Name = data.Name,
        Description = data.Description,
        Tags = data.Tags
      };
      return profile;
    }

    internal static DACameraProfile Translate(CameraProfile profile)
    {
      DACameraProfile data = new DACameraProfile()
      {
        Id = profile.Id,
        Name = profile.Name,
        FilterType = (int)profile.FilterType,
        FilterId = profile.FilterId,
        Description = profile.Description,
        Tags = profile.Tags
      };
      return data;
    }

    internal static CameraConfig Translate(DACameraConfig data)
    {
      CameraConfig config = new CameraConfig()
      {
        FriendlyName = data.FriendlyName,
        OriginalSourceString = data.OriginalSourceString,
        SourceString = data.SourceString,
        FrameInterval = data.FrameInterval,
        FrameRate = data.FrameRate,
        UserName = data.UserName,
        Password = data.Password,
        UserAgent = data.UserAgent,
      };

      if (data.Resolution != null)
      {
        config.Resolution = new Resolution() { Width = data.Resolution.Width, Height = data.Resolution.Height };
      }

      return config;
    }

    internal static DACameraConfig Translate(CameraConfig config)
    {
      DACameraConfig data = new DACameraConfig()
      {
        FriendlyName = config.FriendlyName,
        OriginalSourceString = config.OriginalSourceString,
        SourceString = config.SourceString,
        FrameInterval = config.FrameInterval,
        FrameRate = config.FrameRate,
        UserName = config.UserName,
        Password = config.Password,
        UserAgent = config.UserAgent,
      };

      if (config.Resolution != null)
      {
        data.Resolution = new DAResolution() { Width = config.Resolution.Width, Height = config.Resolution.Height };
      }

      return data;
    }

    internal static DACamera Translate(Camera camera)
    {
      DACamera target = new DACamera();
      target.Id = camera.Id;
      target.Profile = Translate(camera.Profile);
      target.Config = Translate(camera.Config);
      target.Thumbnail = camera.Thumbnail;

      return target;
    }
  }
}
