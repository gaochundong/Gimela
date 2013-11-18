using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Infrastructure.Patterns;
using Gimela.Rukbat.DVC.Contracts.CallbackContracts;
using Gimela.Rukbat.DVC.Contracts.ServiceContracts;

namespace Gimela.Rukbat.MPS.BusinessLogic.ServiceAgents
{
  internal class DeviceConnectorServiceClient : IDeviceConnectorCallbackService
  {
    internal static DeviceConnectorServiceClient ServiceClient
    {
      get
      {
        return Singleton<DeviceConnectorServiceClient>.Instance;
      }
    }

    #region IDeviceConnectorCallbackService Members

    public void NotifyCameraCreated(NotifyCameraCreatedRequest request)
    {

    }

    public void NotifyCameraDeleted(NotifyCameraDeletedRequest request)
    {

    }

    public void NotifyCameraPublished(NotifyCameraPublishedRequest request)
    {

    }

    public void NotifyCameraUnpublished(NotifyCameraUnpublishedRequest request)
    {

    }

    #endregion
  }
}
