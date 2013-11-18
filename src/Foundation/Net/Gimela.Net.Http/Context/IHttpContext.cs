using System.IO;
using System.Net;
using System.Net.Security;

namespace Gimela.Net.Http
{
  /// <summary>
  /// HTTP请求的上下文接口，由HTTP监听器在接受Socket时创建
  /// </summary>
  public interface IHttpContext
  {
    /// <summary>
    /// 获取是否当前上下文使用的是安全连接
    /// </summary>
    bool IsSecure { get; }

    /// <summary>
    /// 获取远端终结点
    /// </summary>
    IPEndPoint RemoteEndPoint { get; }

    /// <summary>
    /// 获取用于从远端终结点发送和接收数据的流
    /// </summary>
    /// <remarks>
    /// <para>
    /// The stream can be any type of stream, do not assume that it's a network
    /// stream. For instance, it can be a <see cref="SslStream"/> or a ZipStream.
    /// </para>
    /// </remarks>
    Stream Stream { get; }

    /// <summary>
    /// 获取当前被处理的请求
    /// </summary>
    /// <value>The request.</value>
    IRequest Request { get; }

    /// <summary>
    /// 获取即将发回客户端的响应
    /// </summary>
    /// <value>The response.</value>
    IResponse Response { get; }

    /// <summary>
    /// 断开上下文连接
    /// </summary>
    void Disconnect();
  }
}