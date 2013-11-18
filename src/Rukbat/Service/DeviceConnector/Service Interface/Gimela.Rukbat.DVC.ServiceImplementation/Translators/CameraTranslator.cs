using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Rukbat.DVC.BusinessEntities;
using Gimela.Rukbat.DVC.BusinessLogic;
using Gimela.Rukbat.DVC.Contracts.DataContracts;
using BEFilterType = Gimela.Rukbat.DVC.BusinessEntities.FilterType;
using DCFilterType = Gimela.Rukbat.DVC.Contracts.DataContracts.FilterTypeData;

namespace Gimela.Rukbat.DVC.ServiceImplementation.Translators
{
  internal static class CameraTranslator
  {
    internal static CameraData Translate(Camera camera)
    {
      CameraData data = new CameraData()
      {
        Profile = Translate(camera.Profile),
        Config = Translate(camera.Config),
        Thumbnail = camera.Thumbnail
      };

      return data;
    }

    internal static CameraProfile Translate(CameraProfileData data)
    {
      CameraProfile profile = new CameraProfile(data.Id, Translate(data.FilterType), data.FilterId)
      {
        Name = data.Name,
        Description = data.Description,
        Tags = data.Tags
      };
      return profile;
    }

    internal static CameraProfileData Translate(CameraProfile profile)
    {
      CameraProfileData data = new CameraProfileData()
      {
        Id = profile.Id,
        Name = profile.Name,
        FilterType = Translate(profile.FilterType),
        FilterId = profile.FilterId,
        Description = profile.Description,
        Tags = profile.Tags
      };
      return data;
    }

    internal static CameraConfig Translate(CameraConfigData data)
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

    internal static CameraConfigData Translate(CameraConfig config)
    {
      CameraConfigData data = new CameraConfigData()
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
        data.Resolution = new ResolutionData() { Width = config.Resolution.Width, Height = config.Resolution.Height };
      }

      return data;
    }

    internal static CameraStatusData Translate(CameraStatus status)
    {
      CameraStatusData cameraStatus = CameraStatusData.Unknown;

      switch (status)
      {
        case CameraStatus.Online:
          cameraStatus = CameraStatusData.Online;
          break;
        case CameraStatus.Offline:
          cameraStatus = CameraStatusData.Offline;
          break;
        case CameraStatus.Unknown:
        default:
          cameraStatus = CameraStatusData.Unknown;
          break;
      }

      return cameraStatus;
    }

    internal static BEFilterType Translate(DCFilterType filterType)
    {
      return (BEFilterType)((int)filterType);
    }

    internal static DCFilterType Translate(BEFilterType filterType)
    {
      return (DCFilterType)((int)filterType);
    }
  }
}
