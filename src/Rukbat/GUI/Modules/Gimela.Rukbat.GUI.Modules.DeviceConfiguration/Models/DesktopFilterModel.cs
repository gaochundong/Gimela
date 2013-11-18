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
  public class DesktopFilterModel : ModelBase
  {
    public void GetDesktopFilters(EventHandler<AsyncWorkerCallbackEventArgs<IList<DesktopFilter>>> callback)
    {
      try
      {
        AsyncWorkerHandle<IList<DesktopFilter>> handle = AsyncWorkerHelper.DoWork<IList<DesktopFilter>>(
          delegate(object sender, DoWorkEventArgs e)
          {
            List<DesktopFilter> filters = new List<DesktopFilter>();

            GetDesktopFiltersRequest request = new GetDesktopFiltersRequest();
            GetDesktopFiltersResponse response =
              ServiceProvider.GetService<IDeviceConnectorService, IDeviceConnectorCallbackService>(
              ViewModelLocator.ServiceClient,
              ViewModelLocator.SelectedService.HostName,
              ViewModelLocator.SelectedService.Uri.ToString()
              ).GetDesktopFilters(request);

            if (response.Filters != null)
            {
              foreach (var item in response.Filters)
              {
                DesktopFilter filter = new DesktopFilter(item.Name, item.Index)
                {
                  Primary = item.IsPrimary,
                  Bounds = item.Bounds
                };
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
