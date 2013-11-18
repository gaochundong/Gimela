using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Rukbat.ServiceDiscovery.Contracts.MessageContracts;
using Gimela.Rukbat.ServiceDiscovery.Contracts.ServiceContracts;
using Gimela.ServiceModel.ChannelManagement;
using Gimela.ServiceModel.ManagedDiscovery;

namespace Gimela.Rukbat.Communication
{
  public static class ServiceProvider
  {
    static ServiceProvider()
    {
      // 服务缓存
      Locator.Add<EndpointDiscoveryMetadataCollection>(new EndpointDiscoveryMetadataCollection());
      // 主动搜索
      Locator.Add<IAdHocDiscoveryClient<IServiceDiscoveryService>>(new AdHocDiscoveryClient<IServiceDiscoveryService>(Locator.Get<EndpointDiscoveryMetadataCollection>()).Start());
      // 被动通知
      Locator.Add<IAnnouncementMonitor<IServiceDiscoveryService>>(new AnnouncementMonitor<IServiceDiscoveryService>(Locator.Get<EndpointDiscoveryMetadataCollection>()).Start());
    }

    public static void Bootstrap()
    {
      // only boot
    }

    public static T GetService<T>() where T : class
    {
      GetServicesRequest request = new GetServicesRequest();
      request.ServiceName = typeof(T).FullName;

      GetServicesResponse response = MessageSender.Send<IServiceDiscoveryService, GetServicesResponse, GetServicesRequest>(
        delegate(IServiceDiscoveryService s, GetServicesRequest r)
        {
          return s.GetServices(r);
        },
        request);

      if (response.Services != null && response.Services.Count > 0)
      {
        ChannelFactory<T> channelFactory = new ChannelFactory<T>(new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName));
        T client = channelFactory.CreateChannel(new EndpointAddress(response.Services[0].Uri));
        return client;
      }

      throw new ContractNotFoundException(string.Format(CultureInfo.InvariantCulture, "Cannot find service type [{0}].", typeof(T).FullName));
    }

    public static T GetService<T>(string hostName) where T : class
    {
      GetServicesRequest request = new GetServicesRequest();
      request.ServiceName = typeof(T).FullName;

      GetServicesResponse response = MessageSender.Send<IServiceDiscoveryService, GetServicesResponse, GetServicesRequest>(
        delegate(IServiceDiscoveryService s, GetServicesRequest r)
        {
          return s.GetServices(r);
        },
        request);

      if (response.Services != null && response.Services.Count > 0)
      {
        ChannelFactory<T> channelFactory = new ChannelFactory<T>(new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName));
        T client = channelFactory.CreateChannel(new EndpointAddress(response.Services.Find(s => s.Uri.Host == hostName).Uri));
        return client;
      }

      throw new ContractNotFoundException(string.Format(CultureInfo.InvariantCulture, "Cannot find service type [{0}] by hostname [{1}].", typeof(T).FullName, hostName));
    }

    public static T GetService<T>(string hostName, string uri) where T : class
    {
      GetServicesRequest request = new GetServicesRequest();
      request.ServiceName = typeof(T).FullName;

      GetServicesResponse response = MessageSender.Send<IServiceDiscoveryService, GetServicesResponse, GetServicesRequest>(
        delegate(IServiceDiscoveryService s, GetServicesRequest r)
        {
          return s.GetServices(r);
        },
        request);

      if (response.Services != null && response.Services.Count > 0)
      {
        ChannelFactory<T> channelFactory = new ChannelFactory<T>(new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName));
        T client = channelFactory.CreateChannel(new EndpointAddress(response.Services.Find(s => s.Uri.Host == hostName && s.Uri.ToString() == uri).Uri));
        return client;
      }

      throw new ContractNotFoundException(string.Format(CultureInfo.InvariantCulture, "Cannot find service type [{0}] by hostname [{1}].", typeof(T).FullName, hostName));
    }

    public static T GetService<T, C>(C callback) where T : class
    {
      GetServicesRequest request = new GetServicesRequest();
      request.ServiceName = typeof(T).FullName;

      GetServicesResponse response = MessageSender.Send<IServiceDiscoveryService, GetServicesResponse, GetServicesRequest>(
        delegate(IServiceDiscoveryService s, GetServicesRequest r)
        {
          return s.GetServices(r);
        },
        request);

      if (response.Services != null && response.Services.Count > 0)
      {
        Locator.Remove<T>();

        DuplexChannelFactory<T> channelFactory = new DuplexChannelFactory<T>(new InstanceContext(callback), new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName));
        T client = channelFactory.CreateChannel(new EndpointAddress(response.Services[0].Uri));
        return client;
      }

      throw new ContractNotFoundException(string.Format(CultureInfo.InvariantCulture, "Cannot find service type [{0}].", typeof(T).FullName));
    }

    public static T GetService<T, C>(C callback, string hostName) where T : class
    {
      GetServicesRequest request = new GetServicesRequest();
      request.ServiceName = typeof(T).FullName;

      GetServicesResponse response = MessageSender.Send<IServiceDiscoveryService, GetServicesResponse, GetServicesRequest>(
        delegate(IServiceDiscoveryService s, GetServicesRequest r)
        {
          return s.GetServices(r);
        },
        request);

      if (response.Services != null && response.Services.Count > 0)
      {
        DuplexChannelFactory<T> channelFactory = new DuplexChannelFactory<T>(new InstanceContext(callback), new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName));
        T client = channelFactory.CreateChannel(new EndpointAddress(response.Services.Find(s => s.Uri.Host == hostName).Uri));
        return client;
      }

      throw new ContractNotFoundException(string.Format(CultureInfo.InvariantCulture, "Cannot find service type [{0}] by hostname [{1}].", typeof(T).FullName, hostName));
    }

    public static T GetService<T, C>(C callback, string hostName, string uri) where T : class
    {
      GetServicesRequest request = new GetServicesRequest();
      request.ServiceName = typeof(T).FullName;

      GetServicesResponse response = MessageSender.Send<IServiceDiscoveryService, GetServicesResponse, GetServicesRequest>(
        delegate(IServiceDiscoveryService s, GetServicesRequest r)
        {
          return s.GetServices(r);
        },
        request);

      if (response.Services != null && response.Services.Count > 0)
      {
        DuplexChannelFactory<T> channelFactory = new DuplexChannelFactory<T>(new InstanceContext(callback), new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName));
        T client = channelFactory.CreateChannel(new EndpointAddress(response.Services.Find(s => s.Uri.Host == hostName && s.Uri.ToString() == uri).Uri));
        return client;
      }

      throw new ContractNotFoundException(string.Format(CultureInfo.InvariantCulture, "Cannot find service type [{0}] by hostname [{1}].", typeof(T).FullName, hostName));
    }

    public static Dictionary<Uri, ServiceProfile> GetServices<T>()
    {
      Dictionary<Uri, ServiceProfile> services = new Dictionary<Uri, ServiceProfile>();

      GetServicesRequest request = new GetServicesRequest();
      request.ServiceName = typeof(T).FullName;

      GetServicesResponse response = MessageSender.Send<IServiceDiscoveryService, GetServicesResponse, GetServicesRequest>(
        delegate(IServiceDiscoveryService s, GetServicesRequest r)
        {
          return s.GetServices(r);
        },
        request);

      if (response.Services != null)
      {
        foreach (var service in response.Services)
        {
          ServiceProfile profile = new ServiceProfile()
          {
            Name = service.Name,
            ContractName = service.ContractName,
            Binding = service.Binding,
            Address = service.Address,
            HostName = service.HostName,
            Uri = service.Uri,
          };
          services.Add(profile.Uri, profile);
        }
      }

      return services;
    }
  }
}
