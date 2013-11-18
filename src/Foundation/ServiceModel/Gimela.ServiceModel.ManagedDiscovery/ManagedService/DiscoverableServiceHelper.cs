using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Xml.Linq;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 可发现服务帮助类
  /// </summary>
  public static class DiscoverableServiceHelper
  {
    /// <summary>
    /// 向服务宿主中添加服务发现终结点
    /// </summary>
    /// <param name="host">被添加终结点的服务宿主</param>
    public static void AddDiscoveryEndpointToServiceHost(ServiceHost host)
    {
      if (host == null)
        throw new ArgumentNullException("host");

      ServiceDiscoveryBehavior behavior = new ServiceDiscoveryBehavior();
      behavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
      host.Description.Behaviors.Add(behavior);
      host.AddServiceEndpoint(new UdpDiscoveryEndpoint());
    }

    /// <summary>
    /// 向服务终结点中添加服务发现行为
    /// </summary>
    /// <param name="endpoint">被添加服务发现行为的终结点</param>
    /// <param name="contractType">被添加终结点的服务契约类型</param>
    public static void AddDiscoveryBehaviorToServiceEndpoint(ServiceEndpoint endpoint, Type contractType)
    {
      if (endpoint == null)
        throw new ArgumentNullException("endpoint");

      EndpointDiscoveryBehavior endpointDiscoveryBehavior = new EndpointDiscoveryBehavior();
      endpointDiscoveryBehavior.Extensions.Add(
          new XElement(
              "root",
              new XElement("SpecifiedName",
                string.Format(CultureInfo.InvariantCulture, @"{0}#{1}", contractType.FullName, endpoint.ListenUri.Host))));

      endpoint.Behaviors.Add(endpointDiscoveryBehavior);

      Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Add discovery behavior to {0}.", endpoint.Address.Uri));
    }
  }
}
