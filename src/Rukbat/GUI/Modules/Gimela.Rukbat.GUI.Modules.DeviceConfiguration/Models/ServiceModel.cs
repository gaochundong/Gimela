using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading;
using Gimela.Crust.Tectosphere;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Common.ExceptionHandling;
using Gimela.Rukbat.DVC.Contracts.ServiceContracts;
using Gimela.Rukbat.Communication;
using Gimela.Rukbat.DomainModels;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Models
{
  public class ServiceModel : ModelBase
  {
    public void GetServices(EventHandler<AsyncWorkerCallbackEventArgs<IList<MediaService>>> callback)
    {
      try
      {
        AsyncWorkerHandle<IList<MediaService>> handle = AsyncWorkerHelper.DoWork<IList<MediaService>>(
          delegate(object sender, DoWorkEventArgs e)
          {
            List<MediaService> services = new List<MediaService>();

            foreach (var item in ServiceProvider.GetServices<IDeviceConnectorService>())
            {
              services.Add(new MediaService()
              {
                Id = item.Value.Uri.ToString(),
                Name = item.Value.Name,
                ContractName = item.Value.Name,
                HostName = item.Value.HostName,
                Uri = item.Value.Uri,
              });
            }

            e.Result = services;
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
