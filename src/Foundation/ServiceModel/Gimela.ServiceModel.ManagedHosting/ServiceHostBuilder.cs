using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Gimela.ServiceModel.ManagedHosting
{
  /// <summary>
  /// 服务托管宿主构建器
  /// </summary>
  public static class ServiceHostBuilder
  {
    /// <summary>
    /// 构建服务托管宿主
    /// </summary>
    /// <typeparam name="TServiceContract">服务契约类型</typeparam>
    /// <param name="serviceType">服务实例类型</param>
    /// <param name="serviceBinding">服务绑定类型</param>
    /// <param name="serviceName">服务名称</param>
    /// <param name="servicePort">服务绑定端口</param>
    /// <returns>服务托管宿主</returns>
    public static ServiceHost GetServiceHost<TServiceContract>(Type serviceType, Binding serviceBinding, string serviceName, int servicePort)
    {
      return GetServiceHost<TServiceContract>(serviceType, serviceBinding, serviceName, servicePort, @"localhost");
    }

    /// <summary>
    /// 构建服务托管宿主
    /// </summary>
    /// <typeparam name="TServiceContract">服务契约类型</typeparam>
    /// <param name="serviceInstance">服务实例类型</param>
    /// <param name="serviceBinding">服务绑定类型</param>
    /// <param name="serviceName">服务名称</param>
    /// <param name="servicePort">服务绑定端口</param>
    /// <returns>服务托管宿主</returns>
    public static ServiceHost GetServiceHost<TServiceContract>(object serviceInstance, Binding serviceBinding, string serviceName, int servicePort)
    {
      return GetServiceHost<TServiceContract>(serviceInstance, serviceBinding, serviceName, servicePort, @"localhost");
    }

    /// <summary>
    /// 构建服务托管宿主
    /// </summary>
    /// <typeparam name="TServiceContract">服务契约类型</typeparam>
    /// <param name="serviceType">服务实例类型</param>
    /// <param name="serviceBinding">服务绑定类型</param>
    /// <param name="serviceName">服务名称</param>
    /// <param name="servicePort">服务绑定端口</param>
    /// <param name="serviceAddress">服务地址</param>
    /// <returns>服务托管宿主</returns>
    public static ServiceHost GetServiceHost<TServiceContract>(Type serviceType, Binding serviceBinding, string serviceName, int servicePort, string serviceAddress)
    {
      return GetServiceHost<TServiceContract>(Activator.CreateInstance(serviceType), serviceBinding, serviceName, servicePort, serviceAddress);
    }

    /// <summary>
    /// 构建服务托管宿主
    /// </summary>
    /// <typeparam name="TServiceContract">服务契约类型</typeparam>
    /// <param name="serviceInstance">服务实例</param>
    /// <param name="serviceBinding">服务绑定类型</param>
    /// <param name="serviceName">服务名称</param>
    /// <param name="servicePort">服务绑定端口</param>
    /// <param name="serviceAddress">服务地址</param>
    /// <returns>服务托管宿主</returns>
    public static ServiceHost GetServiceHost<TServiceContract>(object serviceInstance, Binding serviceBinding, string serviceName, int servicePort, string serviceAddress)
    {
      string address = string.Format(CultureInfo.InvariantCulture, @"{0}://{1}:{2}/{3}", serviceBinding.Scheme, serviceAddress, servicePort, serviceName);

      ServiceHost serviceHost = new ServiceHost(serviceInstance);
      serviceHost.AddServiceEndpoint(typeof(TServiceContract), serviceBinding, address);

      return serviceHost;
    }
  }
}
