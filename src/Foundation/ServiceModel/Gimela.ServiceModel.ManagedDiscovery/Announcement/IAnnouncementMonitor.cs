
namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 公告监听器接口, 监听指定类型服务的上下线通知的服务
  /// </summary>
  /// <typeparam name="T">指定类型服务</typeparam>
  public interface IAnnouncementMonitor<T> where T : class
  {
    /// <summary>
    /// 启动监听
    /// </summary>
    /// <returns>公告监听器接口</returns>
    IAnnouncementMonitor<T> Start();
    /// <summary>
    /// 停止监听
    /// </summary>
    /// <returns>公告监听器接口</returns>
    IAnnouncementMonitor<T> Close();
  }
}
