
namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 点对点服务发现客户端接口
  /// </summary>
  /// <typeparam name="T">服务类型</typeparam>
  public interface IAdHocDiscoveryClient<T> where T : class
  {
    /// <summary>
    /// 启动服务发现
    /// </summary>
    /// <returns>点对点服务发现客户端接口</returns>
    IAdHocDiscoveryClient<T> Start();
    /// <summary>
    /// 终止服务发现
    /// </summary>
    /// <returns>点对点服务发现客户端接口</returns>
    IAdHocDiscoveryClient<T> Close();
  }
}
