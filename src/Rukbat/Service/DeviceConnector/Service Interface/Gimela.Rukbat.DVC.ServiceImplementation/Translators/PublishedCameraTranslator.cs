using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Rukbat.DVC.BusinessEntities;
using Gimela.Rukbat.DVC.BusinessLogic;
using Gimela.Rukbat.DVC.Contracts.DataContracts;

namespace Gimela.Rukbat.DVC.ServiceImplementation.Translators
{
  internal static class PublishedCameraTranslator
  {
    internal static PublishedCameraData Translate(PublishedCamera camera)
    {
      PublishedCameraData data = new PublishedCameraData()
      {
        CameraId = camera.Id,
        Destinations = new List<PublishDestinationData>()
      };
      foreach (var item in camera.Destinations)
      {
        data.Destinations.Add(new PublishDestinationData()
        {
          Address = item.Address,
          Port = item.Port
        });
      }
      return data;
    }

    internal static PublishDestination Translate(PublishDestinationData data)
    {
      PublishDestination dest = new PublishDestination(data.Address, data.Port);
      return dest;
    }
  }
}
