using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 服务发现代理托管器
  /// </summary>
  /// <typeparam name="T">服务发现代理</typeparam>
  public class ServiceDiscoveryProxyHost<T> where T : DiscoveryProxy, new()
  {
    private ServiceHost proxyHost;
    private string proxyAddress;

    /// <summary>
    /// 服务发现代理托管器
    /// </summary>
    /// <param name="proxyAddress">服务发现代理地址</param>
    public ServiceDiscoveryProxyHost(string proxyAddress)
    {
      if (string.IsNullOrEmpty(proxyAddress))
        throw new ArgumentNullException("proxyAddress");

      // proxyAddress like : net.tcp://localhost:8001/discoveryproxy
      this.proxyAddress = proxyAddress;
    }

    /// <summary>
    /// 启动服务发现代理托管器
    /// </summary>
    /// <returns>服务宿主</returns>
    public ServiceHost Open()
    {
      if (proxyHost != null && proxyHost.State != CommunicationState.Closed)
        throw new InvalidProgramException("This proxy host has been opened.");

      // create a new service host with a singleton proxy
      proxyHost = new ServiceHost(new T());

      // create the discovery endpoint
      DiscoveryEndpoint discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName), new EndpointAddress(proxyAddress));

      discoveryEndpoint.IsSystemEndpoint = false;

      // add UDP Annoucement endpoint
      proxyHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());

      // add the discovery endpoint
      proxyHost.AddServiceEndpoint(discoveryEndpoint);

      proxyHost.Open();
      Console.WriteLine("Discovery Proxy {0}", proxyAddress);

      return proxyHost;
    }

    /// <summary>
    /// 关闭服务发现代理托管器
    /// </summary>
    public void Close()
    {
      if (this.proxyHost != null)
      {
        this.proxyHost.BeginClose(
            (result) => { this.proxyHost.EndClose(result); },
            null);
      }
    }
  }
}
