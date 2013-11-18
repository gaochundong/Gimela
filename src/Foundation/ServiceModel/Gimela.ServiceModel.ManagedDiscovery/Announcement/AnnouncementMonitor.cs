using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 监听指定类型服务的上下线通知的服务
  /// </summary>
  /// <typeparam name="T">指定类型服务</typeparam>
  public class AnnouncementMonitor<T> : IAnnouncementMonitor<T> where T : class
  {
    private AnnouncementService announcementService;
    private ServiceHost announcementServiceHost;
    private EndpointDiscoveryMetadataCollection cache;

    /// <summary>
    /// 监听指定类型服务的上下线通知的服务
    /// </summary>
    /// <param name="cache">服务缓存</param>
    public AnnouncementMonitor(EndpointDiscoveryMetadataCollection cache)
    {
      if (cache == null)
        throw new ArgumentNullException("cache");
      this.cache = cache;
    }

    /// <summary>
    /// 启动监听
    /// </summary>
    /// <returns>公告监听器接口</returns>
    public IAnnouncementMonitor<T> Start()
    {
      if (announcementService != null && announcementServiceHost.State != CommunicationState.Closed)
        throw new InvalidProgramException("This announcement service has been opened.");

      this.announcementService = new AnnouncementService();

      // add event handlers
      this.announcementService.OnlineAnnouncementReceived += new EventHandler<AnnouncementEventArgs>(this.OnOnlineAnnouncement);
      this.announcementService.OfflineAnnouncementReceived += new EventHandler<AnnouncementEventArgs>(this.OnOfflineAnnouncement);

      // create the service host with a singleton
      this.announcementServiceHost = new ServiceHost(this.announcementService);

      // add the announcement endpoint
      this.announcementServiceHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());

      // open the host async
      this.announcementServiceHost.BeginOpen(
          (result) => { announcementServiceHost.EndOpen(result); },
          null);

      return this;
    }

    /// <summary>
    /// 停止监听
    /// </summary>
    /// <returns>公告监听器接口</returns>
    public IAnnouncementMonitor<T> Close()
    {
      if (this.announcementServiceHost != null)
      {
        this.announcementService.OnlineAnnouncementReceived -= new EventHandler<AnnouncementEventArgs>(this.OnOnlineAnnouncement);
        this.announcementService.OfflineAnnouncementReceived -= new EventHandler<AnnouncementEventArgs>(this.OnOfflineAnnouncement);

        this.announcementServiceHost.BeginClose(
            (result) => { this.announcementServiceHost.EndClose(result); },
            null);
      }

      return this;
    }

    private void OnOnlineAnnouncement(object sender, AnnouncementEventArgs e)
    {
      EndpointDiscoveryMetadata metadata = e.EndpointDiscoveryMetadata;

      FindCriteria criteria = new FindCriteria(typeof(T));

      if (criteria.IsMatch(metadata))
      {
        // 指定类型的服务上线
        if (!cache.Contains(metadata.Address.Uri))
        {
          cache.Add(metadata);
          Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Announcement endpoint online : {0} - {1}", metadata.GetSpecifiedName(), metadata.Address.Uri));
        }
      }
    }

    private void OnOfflineAnnouncement(object sender, AnnouncementEventArgs e)
    {
      EndpointDiscoveryMetadata metadata = e.EndpointDiscoveryMetadata;

      FindCriteria criteria = new FindCriteria(typeof(T));

      if (criteria.IsMatch(metadata))
      {
        // 指定类型的服务下线
        if (cache.Contains(metadata.Address.Uri))
        {
          cache.Remove(metadata);
          Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Announcement endpoint offline : {0} - {1}", metadata.GetSpecifiedName(), metadata.Address.Uri));
        }
      }
    }
  }
}
