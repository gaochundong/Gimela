using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Gimela.ServiceModel.ManagedService;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Rukbat.DPS.BusinessLogic;

namespace Gimela.Rukbat.DPS.ServiceImplementation
{
  [ServiceBehavior(
    InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple,
    IncludeExceptionDetailInFaults = true)]
  public partial class DeviceProfileService : ManagedServiceBase
  {
    #region Override Methods

    protected override void OnStart()
    {
      Locator.Add<ICameraManager>(new CameraManager());
    }

    protected override void OnStop()
    {
      Locator.Clear();
    }

    #endregion
  }
}
