using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Rukbat.DPS.BusinessLogic;
using Gimela.Rukbat.DPS.Contracts.FaultContracts;
using Gimela.Rukbat.DPS.Contracts.MessageContracts;
using Gimela.Rukbat.DPS.Contracts.ServiceContracts;
using BE = Gimela.Rukbat.DPS.BusinessEntities;
using DC = Gimela.Rukbat.DPS.Contracts.DataContracts;

namespace Gimela.Rukbat.DPS.ServiceImplementation
{
  public partial class DeviceProfileService : IDeviceProfileService
  {
    public GetCamerasResponse GetCameras(GetCamerasRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        GetCamerasResponse response = new GetCamerasResponse();

        IList<BE::Camera> list = Locator.Get<ICameraManager>().GetCameras();
        foreach (var item in list)
        {
          DC::Camera camera = new DC::Camera()
          {
            Id = item.Id,
            Name = item.Name,
            Url = item.Url,
            Port = item.Port,
          };
          response.Cameras.Add(camera);
        }

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceProfileServiceFault>(new DeviceProfileServiceFault(ex.Message, ex), ex.Message);
      }
    }

    public GetCameraResponse GetCamera(GetCameraRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        GetCameraResponse response = new GetCameraResponse();

        BE::Camera c = Locator.Get<ICameraManager>().GetCamera(request.CameraId);
        if (c != null)
        {
          DC::Camera camera = new DC::Camera()
          {
            Id = c.Id,
            Name = c.Name,
            Url = c.Url,
            Port = c.Port,
          };
          response.Camera = camera;
        }

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<DeviceProfileServiceFault>(new DeviceProfileServiceFault(ex.Message, ex), ex.Message);
      }
    }
  }
}
