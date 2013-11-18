using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;
using Gimela.Crust.Tectosphere;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Common.ExceptionHandling;
using Gimela.Common.Logging;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Rukbat.DVC.Contracts.MessageContracts;
using Gimela.Rukbat.DVC.Contracts.ServiceContracts;
using Gimela.Rukbat.Communication;
using Gimela.Rukbat.DomainModels.MediaSource.VideoFilters;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Models;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Rukbat.GUI.ValidationRules.Enumerations;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Models
{
  public class CameraFilterModel : ModelBase
  {
    public void GetCameraFilters(EventHandler<AsyncWorkerCallbackEventArgs<IList<CameraFilter>>> callback)
    {
      try
      {
        AsyncWorkerHandle<IList<CameraFilter>> handle = AsyncWorkerHelper.DoWork<IList<CameraFilter>>(
          delegate(object sender, DoWorkEventArgs e)
          {
            List<CameraFilter> filters = new List<CameraFilter>();

            GetCameraFiltersRequest request = new GetCameraFiltersRequest();
            GetCameraFiltersResponse response =
              ServiceProvider.GetService<IDeviceConnectorService, IDeviceConnectorCallbackService>(
              ViewModelLocator.ServiceClient, 
              ViewModelLocator.SelectedService.HostName,
              ViewModelLocator.SelectedService.Uri.ToString()
              ).GetCameraFilters(request);

            if (response.Filters != null)
            {
              foreach (var item in response.Filters)
              {
                CameraFilter filter = new CameraFilter(item.Name, item.Uri);
                filters.Add(filter);
              }
            }

            e.Result = filters;
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
