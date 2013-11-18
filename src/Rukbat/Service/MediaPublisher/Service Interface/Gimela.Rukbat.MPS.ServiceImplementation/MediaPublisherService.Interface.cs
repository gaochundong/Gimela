using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Gimela.Common.ExceptionHandling;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Rukbat.MPS.BusinessEntities;
using Gimela.Rukbat.MPS.BusinessLogic;
using Gimela.Rukbat.MPS.Contracts.DataContracts;
using Gimela.Rukbat.MPS.Contracts.FaultContracts;
using Gimela.Rukbat.MPS.Contracts.MessageContracts;
using Gimela.Rukbat.MPS.Contracts.ServiceContracts;

namespace Gimela.Rukbat.MPS.ServiceImplementation
{
  public partial class MediaPublisherService : IMediaPublisherService
  {
    #region IMediaPublisherService Members

    public GetPublishedCamerasResponse GetPublishedCameras(GetPublishedCamerasRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        GetPublishedCamerasResponse response = new GetPublishedCamerasResponse();

        IList<PublishedCamera> list = Locator.Get<IPublishedCameraManager>().GetPublishedCameras();
        foreach (var item in list)
        {
          PublishedCameraData camera = new PublishedCameraData()
          {
            Profile = new PublishedCameraProfileData()
            {
              CameraId = item.Profile.CameraId,
              CameraName = item.Profile.CameraName,
              CameraThumbnail = item.Profile.CameraThumbnail,
              DeviceServiceHostName = item.Profile.DeviceServiceHostName,
              DeviceServiceUri = item.Profile.DeviceServiceUri,
            },
            Destination = new PublishedDestinationData()
            {
              Port = item.Destination.Port,
            },
          };
          response.PublishedCameras.Add(camera);
        }

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<MediaPublisherServiceFault>(new MediaPublisherServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public PublishCameraResponse PublishCamera(PublishCameraRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        PublishCameraResponse response = new PublishCameraResponse();

        Locator.Get<IPublishedCameraManager>().PublishCamera(
          new PublishedCameraProfile(request.Profile.CameraId, request.Profile.CameraName)
          {
            CameraThumbnail = request.Profile.CameraThumbnail,
            DeviceServiceHostName = request.Profile.DeviceServiceHostName,
            DeviceServiceUri = request.Profile.DeviceServiceUri,
          },
          new PublishedDestination(request.Destination.Port));

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<MediaPublisherServiceFault>(new MediaPublisherServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public UnpublishCameraResponse UnpublishCamera(UnpublishCameraRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        UnpublishCameraResponse response = new UnpublishCameraResponse();

        Locator.Get<IPublishedCameraManager>().UnpublishCamera(
          request.CameraId, 
          new PublishedDestination(request.Destination.Port));

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<MediaPublisherServiceFault>(new MediaPublisherServiceFault(ex.Message, ex), ex.Message);
      }
    }

    #endregion
  }
}
