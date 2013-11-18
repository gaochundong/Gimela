using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Gimela.ServiceModel.ManagedService;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Rukbat.MPS.BusinessLogic;
using Gimela.Rukbat.MPS.DataAccess;

namespace Gimela.Rukbat.MPS.ServiceImplementation
{
  [ServiceBehavior(
    InstanceContextMode = InstanceContextMode.Single, 
    ConcurrencyMode = ConcurrencyMode.Multiple, 
    IncludeExceptionDetailInFaults = true)]
  public partial class MediaPublisherService : ManagedServiceBase
  {
    #region Override Methods

    protected override void OnStart()
    {
      Locator.Add<IDatabase>(new Database(DatabaseSettings.DatabasePath, DatabaseSettings.DatabaseName));
      Locator.Add<IPublishedCameraRepository>(new PublishedCameraRepository());
      Locator.Add<IStreamingManager>(new StreamingManager());
      Locator.Add<IPublishedCameraManager>(new PublishedCameraManager().Restore());
    }

    protected override void OnStop()
    {
      Locator.Get<IDatabase>().Shutdown();

      Locator.Clear();
    }

    #endregion
  }
}
