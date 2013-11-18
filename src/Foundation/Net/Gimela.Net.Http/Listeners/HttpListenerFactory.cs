using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Gimela.Net.Http
{
  /// <summary>
  /// HTTP监听器创建工厂
  /// </summary>
  public static class HttpListenerFactory
  {
    /// <summary>
    /// 使用默认的HTTP工厂创建监听器新实例
    /// </summary>
    /// <param name="address">监听器监听的地址</param>
    /// <param name="port">监听器监听的端口</param>
    /// <returns>Created HTTP listener.</returns>
    public static HttpListener Create(IPAddress address, int port)
    {
      return new HttpListener(address, port);
    }

    /// <summary>
    /// 使用指定的HTTP工厂创建监听器新实例
    /// </summary>
    /// <param name="address">监听器监听的地址</param>
    /// <param name="port">监听器监听的端口</param>
    /// <param name="factory">Factory used to create different types in the framework.</param>
    /// <returns>Created HTTP listener.</returns>
    public static HttpListener Create(IPAddress address, int port, HttpFactory factory)
    {
      return new HttpListener(address, port);
    }

    /// <summary>
    /// 使用默认的HTTP工厂创建监听器新实例
    /// </summary>
    /// <param name="address">监听器监听的地址</param>
    /// <param name="port">监听器监听的端口</param>
    /// <param name="certificate">使用证书</param>
    /// <returns>Created HTTP listener.</returns>
    public static HttpListener Create(IPAddress address, int port, X509Certificate certificate)
    {
      return new SecureHttpListener(address, port, certificate);
    }
  }
}
