using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Gimela.ServiceModel.ManagedService;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Rukbat.DVC.BusinessLogic;
using Gimela.Rukbat.DVC.DataAccess;

namespace Gimela.Rukbat.DVC.ServiceImplementation
{
  [ServiceBehavior(
    InstanceContextMode = InstanceContextMode.Single, 
    ConcurrencyMode = ConcurrencyMode.Multiple, 
    IncludeExceptionDetailInFaults = true)]
  public partial class DeviceConnectorService : ManagedServiceBase
  {
    #region Override Methods

    protected override void OnStart()
    {
      Locator.Add<IDatabase>(new Database(DatabaseSettings.DatabasePath, DatabaseSettings.DatabaseName));

      Locator.Add<ICameraRepository>(new CameraRepository());
      Locator.Add<IPublishedCameraRepository>(new PublishedCameraRepository());

      Locator.Add<IFilterManager>(new FilterManager());
      Locator.Add<ICameraManager>(new CameraManager());
      Locator.Add<IStreamingManager>(new StreamingManager());
      Locator.Add<IDeviceController>(new DeviceController());

      Locator.Get<IDeviceController>().Start();
    }

    protected override void OnStop()
    {
      Locator.Get<IDeviceController>().Stop();
      Locator.Get<IFilterManager>().Dispose();

      Locator.Get<IDatabase>().Shutdown();

      Locator.Clear();
    }

    #endregion
  }
}
