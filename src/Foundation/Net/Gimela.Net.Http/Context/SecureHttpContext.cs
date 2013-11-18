using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Gimela.Net.Http.Messages;
using Gimela.Net.Http.Transports;
using Gimela.Common.Logging;

namespace Gimela.Net.Http
{
  /// <summary>
  /// HTTP请求的上下文接口，由HTTP监听器在接受Socket时创建
  /// </summary>
  internal class SecureHttpContext : HttpContext
  {
    #region Fields
    
    /// <summary>
    /// 服务器端使用的证书
    /// </summary>
    private readonly X509Certificate _certificate;

    #endregion

    #region Ctors

    /// <summary>
    /// HTTP请求的上下文接口，由HTTP监听器在接受Socket时创建
    /// </summary>
    /// <param name="certificate">服务器端使用的证书</param>
    /// <param name="protocols">SSL协议</param>
    /// <param name="socket">The socket.</param>
    /// <param name="context">The context.</param>
    internal SecureHttpContext(X509Certificate certificate, SslProtocols protocols, Socket socket, MessageFactoryContext context)
      : base(socket, context)
    {
      _certificate = certificate;
      Protocol = protocols;
    }

    #endregion

    #region IHttpContext Members

    /// <summary>
    /// 获取是否当前上下文使用的是安全连接
    /// </summary>
    public override bool IsSecure
    {
      get { return true; }
    }

    #endregion

    #region Properties
    
    /// <summary>
    /// 获取或设置客户端使用的证书
    /// </summary>
    public ClientCertificate ClientCertificate { get; private set; }

    /// <summary>
    /// 获取或设置是否使用客户端的证书代替服务端的证书
    /// </summary>
    public bool UseClientCertificate { get; set; }

    /// <summary>
    /// 获取使用的SSL协议
    /// </summary>
    protected SslProtocols Protocol { get; private set; }

    #endregion

    #region Stream

    /// <summary>
    /// 基于Socket创建用于发送和接收数据的流
    /// </summary>
    /// <param name="socket">Socket to wrap</param>
    /// <returns>Stream</returns>
    /// <exception cref="InvalidOperationException">Stream could not be created.</exception>
    protected override Stream CreateStream(Socket socket)
    {
      Stream stream = base.CreateStream(socket);

      var sslStream = new SslStream(stream, false, OnRemoteCertificateValidation);
      try
      {
        sslStream.AuthenticateAsServer(_certificate, UseClientCertificate, Protocol, false);
      }
      catch (IOException err)
      {
        Logger.Trace(err.Message);
        throw new InvalidOperationException("Failed to authenticate", err);
      }
      catch (ObjectDisposedException err)
      {
        Logger.Trace(err.Message);
        throw new InvalidOperationException("Failed to create stream.", err);
      }
      catch (AuthenticationException err)
      {
        Logger.Trace((err.InnerException != null) ? err.InnerException.Message : err.Message);
        throw new InvalidOperationException("Failed to authenticate.", err);
      }

      return sslStream;
    }

    private bool OnRemoteCertificateValidation(object sender, X509Certificate receivedCertificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
      ClientCertificate = new ClientCertificate(receivedCertificate, chain, sslPolicyErrors);
      return !(UseClientCertificate && receivedCertificate == null);
    }

    #endregion
  }
}