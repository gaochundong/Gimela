using System;
using System.Net;
using System.Net.Sockets;

namespace Gimela.Net.Http
{
  /// <summary>
  /// HTTP消息监听器接口
  /// </summary>
  public interface IHttpListener
  {
    /// <summary>
    /// 获取监听器监听的地址
    /// </summary>
    IPAddress Address { get; }

    /// <summary>
    /// 获取监听器监听的端口
    /// </summary>
    int Port { get; }

    /// <summary>
    /// 监听器是否是基于安全策略的
    /// </summary>
    bool IsSecure { get; }

    /// <summary>
    /// 监听器是否已启动监听
    /// </summary>
    bool IsStarted { get; }

    /// <summary>
    /// 获取或设置监听器允许的消息内容最大长度
    /// </summary>
    /// <value>The content length limit.</value>
    /// <remarks>
    /// Used when responding to 100-continue.
    /// </remarks>
    int ContentLengthLimit { get; set; }

    /// <summary>
    /// 启动监听器
    /// </summary>
    /// <param name="backLog">挂起的请求数量</param>
    /// <remarks>
    /// Make sure that you are subscribing on <see cref="RequestReceived"/> first.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Listener have already been started.</exception>
    /// <exception cref="SocketException">Failed to start socket.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Invalid port number.</exception>
    void Start(int backLog);

    /// <summary>
    /// 停止监听器
    /// </summary>
    void Stop();

    /// <summary>
    /// 当接收到一个请求时发生
    /// </summary>
    event EventHandler<RequestEventArgs> RequestReceived;

    /// <summary>
    /// 当连接被接受时发生，可用于拒绝一定的请求客户端
    /// </summary>
    event EventHandler<SocketFilterEventArgs> SocketAccepted;

    /// <summary>
    /// 当消息处理出现异常时发生，请求显示错误页面
    /// </summary>
    /// <remarks>
    /// Fill the body with a user friendly error page, or redirect to somewhere else.
    /// </remarks>
    event EventHandler<ErrorPageEventArgs> ErrorPageRequested;
  }
}