using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using Gimela.Crust.Tectosphere;
using Gimela.Common.ExceptionHandling;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Rukbat.Communication;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.DomainModels.MediaSource;
using Gimela.Rukbat.GUI.Modules.PublishMedia.Entities;
using Gimela.Rukbat.MPS.Contracts.DataContracts;
using Gimela.Rukbat.MPS.Contracts.MessageContracts;
using Gimela.Rukbat.MPS.Contracts.ServiceContracts;

namespace Gimela.Rukbat.GUI.Modules.PublishMedia.Models
{
  public class PublishServiceModel : ModelBase
  {
    public void GetServices(EventHandler<AsyncWorkerCallbackEventArgs<IList<MediaService>>> callback)
    {
      try
      {
        AsyncWorkerHandle<IList<MediaService>> handle = AsyncWorkerHelper.DoWork<IList<MediaService>>(
          delegate(object sender, DoWorkEventArgs e)
          {
            List<MediaService> services = new List<MediaService>();

            foreach (var item in ServiceProvider.GetServices<IMediaPublisherService>())
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
