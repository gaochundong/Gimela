using System;
using System.ServiceModel;
using Gimela.ServiceModel.ManagedDiscovery;

namespace Gimela.ServiceModel.ManagedHosting
{
  /// <summary>
  /// 服务寄宿激活器
  /// </summary>
  /// <typeparam name="TServiceContract">服务契约类型</typeparam>
  public class ServiceHostActivator<TServiceContract> where TServiceContract : class
  {
    /// <summary>
    /// 服务寄宿激活器
    /// </summary>
    /// <param name="info">服务寄宿信息</param>
    public ServiceHostActivator(ServiceHostInfo info)
    {
      if (info == null)
        throw new ArgumentNullException("info");
      if (info.Service.GetInterface(info.Contract.ToString()) == null)
        throw new ArgumentException("The service does not implement the target interface.");

      ServiceInfo = info;
    }

    /// <summary>
    /// 服务寄宿信息
    /// </summary>
    public ServiceHostInfo ServiceInfo { get; private set; }

    /// <summary>
    /// 服务实例
    /// </summary>
    public TServiceContract ServiceInstance { get; private set; }

    /// <summary>
    /// 服务寄宿
    /// </summary>
    public ServiceHost ServiceHost { get; private set; }

    /// <summary>
    /// 启动服务
    /// </summary>
    public virtual void Start()
    {
      if (ServiceInstance == null)
      {
        Start(Activator.CreateInstance(ServiceInfo.Service) as TServiceContract);
      }
      else
      {
        Start(ServiceInstance);
      }
    }

    /// <summary>
    /// 启动服务
    /// </summary>
    /// <param name="singletonInstance">承载的服务的实例</param>
    public virtual void Start(TServiceContract singletonInstance)
    {
      if (singletonInstance == null)
        throw new ArgumentNullException("singletonInstance");

      if (ServiceHost != null && ServiceHost.State != CommunicationState.Closed)
        throw new InvalidOperationException("The service host has been started.");

      ServiceInstance = singletonInstance;

      ServiceHost = ServiceHostBuilder.GetServiceHost<TServiceContract>(ServiceInstance, ServiceInfo.Binding, ServiceInfo.Name, ServiceInfo.Port, ServiceInfo.Address);

      // add discovery endpoint and behavior
      IDiscoverableService discoverableService = ServiceInstance as IDiscoverableService;
      if (discoverableService != null)
      {
        DiscoverableServiceHelper.AddDiscoveryEndpointToServiceHost(ServiceHost);
        var endpoint = ServiceHost.Description.Endpoints.Find(typeof(TServiceContract));
        DiscoverableServiceHelper.AddDiscoveryBehaviorToServiceEndpoint(endpoint, typeof(TServiceContract));        
      }

      ServiceHost.Open();
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    public virtual void Stop()
    {
      if (ServiceHost != null)
      {
        ServiceHost.Abort();
        ServiceHost = null;
      }
    }
  }
}
