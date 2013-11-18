using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using Gimela.Common.ExceptionHandling;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 在指定的服务发现代理服务器中查找指定的服务
  /// </summary>
  /// <typeparam name="T">指定类型的服务</typeparam>
  public class ServiceDiscoveryProxyClient<T> where T : class
  {
    private DiscoveryClient discoveryClient = null;
    private string proxyAddress;
    private EndpointDiscoveryMetadataCollection cache;

    /// <summary>
    /// 在指定的服务发现代理服务器中查找指定的服务
    /// </summary>
    /// <param name="proxyAddress">发现代理服务器地址</param>
    /// <param name="cache">缓存</param>
    public ServiceDiscoveryProxyClient(string proxyAddress, EndpointDiscoveryMetadataCollection cache)
    {
      if (string.IsNullOrEmpty(proxyAddress))
        throw new ArgumentNullException("proxyAddress");

      // 指定的代理服务器
      this.proxyAddress = proxyAddress;

      if (cache == null)
        throw new ArgumentNullException("cache");
      this.cache = cache;
    }

    /// <summary>
    /// 查找指定的服务
    /// </summary>
    public void Start()
    {
      if (discoveryClient != null && this.discoveryClient.InnerChannel.State != CommunicationState.Closed)
        throw new InvalidProgramException("This discovery proxy client has been started.");

      // create an endpoint for the proxy
      DiscoveryEndpoint proxyEndpoint = new DiscoveryEndpoint(new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName), new EndpointAddress(proxyAddress));

      // create the DiscoveryClient with a proxy endpoint for managed discovery
      this.discoveryClient = new DiscoveryClient(proxyEndpoint);

      // same handlers as ad hoc discovery
      this.discoveryClient.FindCompleted += new EventHandler<FindCompletedEventArgs>(this.OnFindCompleted);
      this.discoveryClient.FindProgressChanged += new EventHandler<FindProgressChangedEventArgs>(this.OnFindProgressChanged);

      this.discoveryClient.FindAsync(new FindCriteria(typeof(T)));
    }

    /// <summary>
    /// 终止服务发现
    /// </summary>
    public void Close()
    {
      if (this.discoveryClient != null)
      {
        this.discoveryClient.FindCompleted -= new EventHandler<FindCompletedEventArgs>(this.OnFindCompleted);
        this.discoveryClient.FindProgressChanged -= new EventHandler<FindProgressChangedEventArgs>(this.OnFindProgressChanged);

        this.discoveryClient.CancelAsync(discoveryClient);
        this.discoveryClient.Close();
      }
    }

    private void OnFindProgressChanged(object sender, FindProgressChangedEventArgs e)
    {
      if (!cache.Contains(e.EndpointDiscoveryMetadata.Address.Uri))
      {
        cache.Add(e.EndpointDiscoveryMetadata);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Discovery proxy client find endpoint : {0} - {1}", e.EndpointDiscoveryMetadata.GetSpecifiedName(), e.EndpointDiscoveryMetadata.Address.Uri));
      }
    }

    private void OnFindCompleted(object sender, FindCompletedEventArgs e)
    {
      if (e.Cancelled)
      {
        // 搜索被取消了
      }
      else if (e.Error != null)
      {
        // 搜索过程出现错误
        this.discoveryClient.Close();
        ExceptionHandler.Handle(e.Error);
      }
      else
      {
        if (this.discoveryClient.InnerChannel.State == CommunicationState.Opened)
        {
          this.discoveryClient.Close();
        }
      }

      this.discoveryClient = null;
    }
  }
}
