using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Gimela.Net.Http
{
  /// <summary>
  /// 基于安全连接的HTTP监听器
  /// </summary>
  public class SecureHttpListener : HttpListener
  {
    /// <summary>
    /// 安全连接的证书
    /// </summary>
    private readonly X509Certificate _certificate;

    /// <summary>
    /// 基于安全连接的HTTP监听器
    /// </summary>
    /// <param name="address">监听器监听的地址</param>
    /// <param name="port">监听器监听的端口</param>
    /// <param name="certificate">安全连接的证书</param>
    internal SecureHttpListener(IPAddress address, int port, X509Certificate certificate)
      : base(address, port)
    {
      Protocol = SslProtocols.Tls;

      _certificate = certificate;
    }

    /// <summary>
    /// 监听器是否是基于安全策略的
    /// </summary>
    public override bool IsSecure
    {
      get { return true; }
    }

    /// <summary>
    /// 获取或设置SSL协议
    /// </summary>
    public SslProtocols Protocol { get; set; }

    /// <summary>
    /// 获取或设置是否使用客户端证书
    /// </summary>
    public bool UseClientCertificate { get; set; }

    /// <summary>
    /// 创建连接上下文
    /// </summary>
    /// <param name="socket">被接受的连接</param>
    /// <returns>A new context.</returns>
    /// <remarks>
    /// Factory is assigned by the <see cref="HttpListener"/> on each incoming request.
    /// </remarks>
    protected override HttpContext CreateContext(Socket socket)
    {
      return Factory.Get<SecureHttpContext>(_certificate, Protocol, socket);
    }
  }
}