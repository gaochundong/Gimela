using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using Gimela.Common.ExceptionHandling;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 服务主动发现器
  /// </summary>
  /// <typeparam name="T">发现指定的服务类型</typeparam>
  public class AdHocDiscoveryClient<T> : IAdHocDiscoveryClient<T> where T : class
  {
    private DiscoveryClient discoveryClient = null;
    private EndpointDiscoveryMetadataCollection cache;

    /// <summary>
    /// 服务主动发现器
    /// </summary>
    /// <param name="cache">服务缓存</param>
    public AdHocDiscoveryClient(EndpointDiscoveryMetadataCollection cache)
    {
      if (cache == null)
        throw new ArgumentNullException("cache");
      this.cache = cache;
    }

    /// <summary>
    /// 启动服务发现
    /// </summary>
    /// <returns>点对点服务发现客户端接口</returns>
    public IAdHocDiscoveryClient<T> Start()
    {
      if (discoveryClient != null && this.discoveryClient.InnerChannel.State != CommunicationState.Closed)
        throw new InvalidProgramException("This ad hoc discovery client has been started.");

      this.discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());

      this.discoveryClient.FindProgressChanged += new EventHandler<FindProgressChangedEventArgs>(this.OnFindProgressChanged);
      this.discoveryClient.FindCompleted += new EventHandler<FindCompletedEventArgs>(this.OnFindCompleted);

      // 开始搜索指定类型的服务
      this.discoveryClient.FindAsync(new FindCriteria(typeof(T)), discoveryClient);

      return this;
    }

    /// <summary>
    /// 终止服务发现
    /// </summary>
    /// <returns>点对点服务发现客户端接口</returns>
    public IAdHocDiscoveryClient<T> Close()
    {
      if (this.discoveryClient != null)
      {
        this.discoveryClient.FindProgressChanged -= new EventHandler<FindProgressChangedEventArgs>(this.OnFindProgressChanged);
        this.discoveryClient.FindCompleted -= new EventHandler<FindCompletedEventArgs>(this.OnFindCompleted);

        this.discoveryClient.CancelAsync(discoveryClient);
        this.discoveryClient.Close();
      }

      return this;
    }

    private void OnFindProgressChanged(object sender, FindProgressChangedEventArgs e)
    {
      if (!cache.Contains(e.EndpointDiscoveryMetadata.Address.Uri))
      {
        cache.Add(e.EndpointDiscoveryMetadata);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "AdHoc discovery find endpoint : {0} - {1}", e.EndpointDiscoveryMetadata.GetSpecifiedName(), e.EndpointDiscoveryMetadata.Address.Uri));
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
