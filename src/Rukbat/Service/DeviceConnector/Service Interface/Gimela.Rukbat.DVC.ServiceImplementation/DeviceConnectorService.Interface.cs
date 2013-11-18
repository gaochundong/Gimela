using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Gimela.Common.ExceptionHandling;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Rukbat.DVC.BusinessEntities;
using Gimela.Rukbat.DVC.BusinessLogic;
using Gimela.Rukbat.DVC.Contracts.DataContracts;
using Gimela.Rukbat.DVC.Contracts.FaultContracts;
using Gimela.Rukbat.DVC.Contracts.MessageContracts;
using Gimela.Rukbat.DVC.Contracts.ServiceContracts;
using Gimela.Rukbat.DVC.ServiceImplementation.Translators;

namespace Gimela.Rukbat.DVC.ServiceImplementation
{
  public partial class DeviceConnectorService : IDeviceConnectorService
  {
    #region IDeviceConnectorService Members

    public GetCameraFiltersResponse GetCameraFilters(GetCameraFiltersRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        GetCameraFiltersResponse response = new GetCameraFiltersResponse();
        response.Filters = new CameraFilterDataCollection();

        IEnumerable<CameraFilter> filters = Locator.Get<IFilterManager>().GetCameraFilters();
        response.Filters.AddRange((from f in filters select CameraFilterTranslator.Translate(f)).ToList());

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public GetDesktopFiltersResponse GetDesktopFilters(GetDesktopFiltersRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        GetDesktopFiltersResponse response = new GetDesktopFiltersResponse();
        response.Filters = new DesktopFilterDataCollection();

        IEnumerable<DesktopFilter> filters = Locator.Get<IFilterManager>().GetDesktopFilters();
        response.Filters.AddRange((from f in filters select DesktopFilterTranslator.Translate(f)).ToList());

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public GetCameraResponse GetCamera(GetCameraRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        GetCameraResponse response = new GetCameraResponse();

        response.Camera = CameraTranslator.Translate(Locator.Get<ICameraManager>().GetCamera(request.CameraId));

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public GetCamerasResponse GetCameras(GetCamerasRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        GetCamerasResponse response = new GetCamerasResponse();
        response.Cameras = new CameraDataCollection();

        IList<Camera> list = Locator.Get<ICameraManager>().GetCameras();
        response.Cameras.AddRange((from c in list select CameraTranslator.Translate(c)).ToList());

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public CreateCameraResponse CreateCamera(CreateCameraRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        CreateCameraResponse response = new CreateCameraResponse();

        response.Camera = CameraTranslator.Translate(
          Locator.Get<ICameraManager>().CreateCamera(
            CameraTranslator.Translate(request.Camera.Profile),
            CameraTranslator.Translate(request.Camera.Config)));

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public UpdateCameraResponse UpdateCamera(UpdateCameraRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        UpdateCameraResponse response = new UpdateCameraResponse();

        response.Camera = CameraTranslator.Translate(
          Locator.Get<ICameraManager>().UpdateCamera(
          request.CameraId, request.CameraName, request.Description, request.Tags));

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public DeleteCameraResponse DeleteCamera(DeleteCameraRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        DeleteCameraResponse response = new DeleteCameraResponse();

        Locator.Get<ICameraManager>().DeleteCamera(request.CameraId);

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public PingCameraResponse PingCamera(PingCameraRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        PingCameraResponse response = new PingCameraResponse();

        response.Status = CameraTranslator.Translate(Locator.Get<ICameraManager>().PingCamera(request.CameraId));

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public GetCameraSnapshotResponse GetCameraSnapshot(GetCameraSnapshotRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        GetCameraSnapshotResponse response = new GetCameraSnapshotResponse();

        response.Snapshot = Locator.Get<IStreamingManager>().GetCameraSnapshot(request.CameraId);

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public GetPublishedCamerasResponse GetPublishedCameras(GetPublishedCamerasRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        GetPublishedCamerasResponse response = new GetPublishedCamerasResponse();
        response.PublishedCameras = new PublishedCameraDataCollection();

        IList<PublishedCamera> list = Locator.Get<IStreamingManager>().GetPublishedCameras();
        response.PublishedCameras.AddRange((from c in list select PublishedCameraTranslator.Translate(c)).ToList());

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public PublishCameraResponse PublishCamera(PublishCameraRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        PublishCameraResponse response = new PublishCameraResponse();

        Locator.Get<IStreamingManager>().PublishCamera(request.CameraId, PublishedCameraTranslator.Translate(request.Destination));

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public UnpublishCameraResponse UnpublishCamera(UnpublishCameraRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        UnpublishCameraResponse response = new UnpublishCameraResponse();

        Locator.Get<IStreamingManager>().UnpublishCamera(request.CameraId, PublishedCameraTranslator.Translate(request.Destination));

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public KeepPublishedCameraAliveResponse KeepPublishedCameraAlive(KeepPublishedCameraAliveRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        KeepPublishedCameraAliveResponse response = new KeepPublishedCameraAliveResponse();

        Locator.Get<IStreamingManager>().KeepAlive(request.CameraId, PublishedCameraTranslator.Translate(request.Destination));

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceConnectorServiceFault>(new DeviceConnectorServiceFault(ex.Message, ex), ex.Message);
      }
    }

    #endregion
  }
}
