using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using Gimela.ServiceModel.ManagedDiscovery;
using Gimela.ServiceModel.ManagedService;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Rukbat.SVD.BusinessEntities;
using Gimela.Rukbat.SVD.BusinessLogic;

namespace Gimela.Rukbat.SVD.ServiceImplementation
{
  [ServiceBehavior(
    InstanceContextMode = InstanceContextMode.Single, 
    ConcurrencyMode = ConcurrencyMode.Multiple, 
    IncludeExceptionDetailInFaults = true)]
  public partial class ServiceDiscoveryService : ManagedServiceBase
  {
    #region Fields

    private ReadOnlyCollection<ContractInfo> contracts;

    #endregion

    #region Override Methods

    protected override void OnStart()
    {
      Locator.Add<IContractFinder>(new ContractFinder());
      Locator.Add<EndpointDiscoveryMetadataCollection>(new EndpointDiscoveryMetadataCollection());

      contracts = Locator.Get<IContractFinder>().Find();
      foreach (var contract in contracts)
      {
        StartDiscoverServiceByType(contract.ContractType);
        Console.WriteLine(string.Format("Resolve - {0}", contract.ContractType.FullName));
      }
    }

    protected override void OnStop()
    {
      foreach (var contract in contracts)
      {
        StopDiscoverServiceByType(contract.ContractType);
      }

      Locator.Clear();
    }

    #endregion

    #region Discovery Methods

    private void StartDiscoverServiceByType(Type type)
    {
      MethodInfo method = this.GetType().GetMethod("StartDiscoverService", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
      if (method == null)
        throw new InvalidOperationException("Method missing.");
      MethodInfo genericMethod = method.MakeGenericMethod(new Type[] { type });
      if (genericMethod == null)
        throw new InvalidOperationException("Method missing.");
      genericMethod.Invoke(null, null);
    }

    private void StopDiscoverServiceByType(Type type)
    {
      MethodInfo method = this.GetType().GetMethod("StopDiscoverService", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
      if (method == null)
        throw new InvalidOperationException("Method missing.");
      MethodInfo genericMethod = method.MakeGenericMethod(new Type[] { type });
      if (genericMethod == null)
        throw new InvalidOperationException("Method missing.");
      genericMethod.Invoke(null, null);
    }

    private static void StartDiscoverService<T>() where T : class
    {
      // 主动搜索
      Locator.Add<IAdHocDiscoveryClient<T>>(new AdHocDiscoveryClient<T>(Locator.Get<EndpointDiscoveryMetadataCollection>()).Start());
      // 被动通知
      Locator.Add<IAnnouncementMonitor<T>>(new AnnouncementMonitor<T>(Locator.Get<EndpointDiscoveryMetadataCollection>()).Start());
    }

    private static void StopDiscoverService<T>() where T : class
    {
      // 主动搜索
      Locator.Get<IAdHocDiscoveryClient<T>>().Close();
      Locator.Remove<IAdHocDiscoveryClient<T>>();
      // 被动通知
      Locator.Get<IAnnouncementMonitor<T>>().Close();
      Locator.Remove<IAnnouncementMonitor<T>>();
    }

    #endregion
  }
}
