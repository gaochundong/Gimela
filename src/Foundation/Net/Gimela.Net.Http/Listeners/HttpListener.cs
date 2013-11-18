using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Gimela.Common.ExceptionHandling;
using Gimela.Net.Http.Headers;
using Gimela.Net.Http.Messages;
using Gimela.Common.Logging;

namespace Gimela.Net.Http
{
  /// <summary>
  /// HTTP消息监听器
  /// </summary>
  public class HttpListener : IHttpListener
  {
    #region Fields

    /// <summary>
    /// 用于该HTTP协议栈的依赖注入器
    /// </summary>
    private readonly HttpFactory _factory;
    /// <summary>
    /// 监听器的TCP连接
    /// </summary>
    private TcpListener _listener;
    /// <summary>
    /// 挂起队列中的请求数量
    /// </summary>
    private int _pendingAccepts;
    /// <summary>
    /// 是否监听器正在停止
    /// </summary>
    private bool _shuttingDown;
    /// <summary>
    /// 监听器停止信号量
    /// </summary>
    private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);

    #endregion

    #region Ctors

    /// <summary>
    /// HTTP消息监听器
    /// </summary>
    /// <param name="address">监听器监听的地址</param>
    /// <param name="port">监听器监听的端口</param>
    internal HttpListener(IPAddress address, int port)
      : this(address, port, new HttpFactory())
    {
    }

    /// <summary>
    /// HTTP消息监听器
    /// </summary>
    /// <param name="address">监听器监听的地址</param>
    /// <param name="port">监听器监听的端口</param>
    /// <param name="httpFactory">用于该HTTP协议栈的依赖注入器</param>
    internal HttpListener(IPAddress address, int port, HttpFactory httpFactory)
    {
      Address = address;
      Port = port;

      _factory = httpFactory;
    }

    #endregion

    #region IHttpListener Members

    /// <summary>
    /// 获取监听器监听的地址
    /// </summary>
    public IPAddress Address { get; private set; }

    /// <summary>
    /// 获取监听器监听的端口
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// 监听器是否是基于安全策略的
    /// </summary>
    public virtual bool IsSecure
    {
      get { return false; }
    }

    /// <summary>
    /// 监听器是否已启动监听
    /// </summary>
    public bool IsStarted { get; private set; }

    /// <summary>
    /// 获取或设置监听器允许的消息内容最大长度
    /// </summary>
    /// <value>The content length limit.</value>
    /// <remarks>
    /// <para>
    /// Used when responding to 100-continue.
    /// </para>
    /// <para>
    /// 0 = turned off.
    /// </para>
    /// </remarks>
    public int ContentLengthLimit { get; set; }

    /// <summary>
    /// 启动监听器
    /// </summary>
    /// <param name="backLog">挂起队列中的请求数量</param>
    /// <remarks>
    /// Make sure that you are subscribing on <see cref="RequestReceived"/> first.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Listener have already been started.</exception>
    /// <exception cref="SocketException">Failed to start socket.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Invalid port number.</exception>
    public void Start(int backLog)
    {
      if (_listener != null)
        throw new InvalidOperationException("Listener have already been started.");

      IsStarted = true;
      _listener = new TcpListener(Address, Port);
      _listener.Start(backLog);

      if (Port == 0 && _listener.LocalEndpoint is IPEndPoint)
        Port = ((IPEndPoint)_listener.LocalEndpoint).Port;

      // do not use beginaccept. Let exceptions be thrown.
      Interlocked.Increment(ref _pendingAccepts);
      _listener.BeginAcceptSocket(OnSocketAccepted, null);
    }

    /// <summary>
    /// 停止监听器
    /// </summary>
    public void Stop()
    {
      _shuttingDown = true;
      _listener.Stop();
    }

    /// <summary>
    /// 当接收到一个请求时发生
    /// </summary>
    public event EventHandler<RequestEventArgs> RequestReceived = delegate { };

    /// <summary>
    /// 当连接被接受时发生，可用于拒绝一定的请求客户端
    /// </summary>
    public event EventHandler<SocketFilterEventArgs> SocketAccepted = delegate { };

    /// <summary>
    /// 当消息处理出现异常时发生，请求显示错误页面
    /// </summary>
    /// <remarks>
    /// Fill the body with a user friendly error page, or redirect to somewhere else.
    /// </remarks>
    public event EventHandler<ErrorPageEventArgs> ErrorPageRequested = delegate { };

    #endregion

    #region Events

    /// <summary>
    /// 客户端询问是否需要继续保持连接
    /// </summary>
    /// <remarks>
    /// If the body is too large or anything like that you should respond <see cref="HttpStatusCode.ExpectationFailed"/>.
    /// </remarks>
    public event EventHandler<RequestEventArgs> ContinueResponseRequested = delegate { };

    #endregion

    #region Protected Methods

    /// <summary>
    /// 获取用于该HTTP协议栈的依赖注入器
    /// </summary>
    protected IHttpFactory Factory
    {
      get { return _factory; }
    }

    /// <summary>
    /// 创建连接上下文
    /// </summary>
    /// <param name="socket">被接受的连接</param>
    /// <returns>A new context.</returns>
    /// <remarks>
    /// Factory is assigned by the <see cref="HttpListener"/> on each incoming request.
    /// </remarks>
    protected virtual HttpContext CreateContext(Socket socket)
    {
      return Factory.Get<HttpContext>(socket);
    }

    #endregion

    #region Socket Accepted

    /// <summary>
    /// 监听Socket连接
    /// </summary>
    /// <param name="ar">异步处理结果</param>
    private void OnSocketAccepted(IAsyncResult ar)
    {
      HttpFactory.Current = Factory;

      Socket socket = null;
      try
      {
        socket = _listener.EndAcceptSocket(ar);
        Interlocked.Decrement(ref _pendingAccepts);

        // 是否正在停止监听
        if (_shuttingDown && _pendingAccepts == 0)
          _shutdownEvent.Set();

        // 该Socket连接是否能被接受
        if (!CanAcceptSocket(socket))
        {
          Logger.Debug("Socket was rejected: " + socket.RemoteEndPoint);
          socket.Disconnect(true);
          BeginAccept(); // 接受新的连接
          return;
        }
      }
      catch (Exception err)
      {
        Logger.Warning("Failed to end accept: " + err.Message);
        BeginAccept(); // 接受新的连接
        if (socket != null)
          socket.Disconnect(true);
        return;
      }

      if (!_shuttingDown)
        BeginAccept(); // 接受新的连接

      Logger.Trace("Accepted connection from: " + socket.RemoteEndPoint);

      // 已接受新的连接，生成连接上下文
      try
      {
        HttpContext context = CreateContext(socket);
        HttpContext.Current = context;
        context.HttpFactory = _factory;
        context.RequestReceived += OnContextRequest;
        context.Disconnected += OnContextDisconnected;
        context.ContinueResponseRequested += OnContext100Continue;
        context.Start(); // 开始上下文
      }
      catch (Exception err)
      {
        Logger.Error("ContextReceived raised an exception: " + err.Message);
        socket.Disconnect(true);
      }
    }

    /// <summary>
    /// 接受新的连接
    /// </summary>
    private void BeginAccept()
    {
      if (_shuttingDown)
        return;

      Interlocked.Increment(ref _pendingAccepts);
      try
      {
        _listener.BeginAcceptSocket(OnSocketAccepted, null);
      }
      catch (Exception err)
      {
        Logger.Error("Unhandled exception in BeginAccept.");
        ExceptionHandler.Handle(err);
      }
    }

    /// <summary>
    /// 该Socket连接是否能被接受
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    private bool CanAcceptSocket(Socket socket)
    {
      try
      {
        var args = new SocketFilterEventArgs(socket);
        SocketAccepted(this, args); // 触发Socket连接是否能被接受事件通知
        return args.IsSocketOk;
      }
      catch (Exception err)
      {
        Logger.Error("SocketAccepted trigger exception: " + err);
        return true;
      }
    }

    #endregion

    #region Context Handler

    /// <summary>
    /// 响应上下文中接收请求消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnContextRequest(object sender, RequestEventArgs e)
    {
      var context = (HttpContext)sender;
      HttpFactory.Current = Factory;
      HttpContext.Current = context;

      try
      {
        var args = new RequestEventArgs(context, e.Request, e.Response);
        // 触发请求收到事件通知
        RequestReceived(this, args);
        // 判断外部程序是否已处理该请求
        if (!args.IsHandled)
        {
          // 如果外部程序未处理该请求，则发送请求的响应消息
          var generator = new ResponseWriter();
          generator.Send(context, args.Response);
        }

        // 请求被处理后断开连接
        if (e.Response.HttpVersion == "HTTP/1.0" || e.Response.Connection.Type == ConnectionType.Close)
          context.Disconnect();
      }
      catch (Exception err)
      {
        if (err is HttpException)
        {
          var exception = (HttpException)err;
          SendErrorPage(exception);
        }
        else
        {
          Logger.Debug("Request failed.");
          ExceptionHandler.Handle(err);
          SendErrorPage(err);
        }
        e.IsHandled = true;
      }
    }

    /// <summary>
    /// 响应上下文中接收100Continue请求消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnContext100Continue(object sender, ContinueEventArgs e)
    {
      // 100状态码的目的在于允许客户端判定服务器是否愿意接受客户端发来的消息主体（基于请求头域）在客户端发送此请求消息主体前。
      var response = new Response(e.Request.HttpVersion, HttpStatusCode.Continue, "Please continue.");

      // 如果服务器认为客户端准备发送的消息体长度超过限制范围，则回复417状态码拒绝接收
      if (ContentLengthLimit != 0 && e.Request.ContentLength.Value > ContentLengthLimit)
      {
        Logger.Warning("Requested to send " + e.Request.ContentLength.Value + " bytes, but we only allow " + ContentLengthLimit);
        response.Status = HttpStatusCode.ExpectationFailed;
        response.Reason = "Too large content length";
      }

      string responseString = string.Format("{0} {1} {2}\r\n\r\n",
        e.Request.HttpVersion, (int)response.Status, response.Reason);
      byte[] buffer = e.Request.Encoding.GetBytes(responseString);
      HttpContext.Current.Stream.Write(buffer, 0, buffer.Length);
      HttpContext.Current.Stream.Flush();
      Logger.Info(responseString);
    }

    /// <summary>
    /// 响应上下文中连接断开事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnContextDisconnected(object sender, EventArgs e)
    {
      HttpFactory.Current = Factory;
      var context = (HttpContext)sender;
      context.Disconnected -= OnContextDisconnected;
      context.RequestReceived -= OnContextRequest;
      context.ContinueResponseRequested -= OnContext100Continue;
    }

    /// <summary>
    /// 发送异常页面通知
    /// </summary>
    /// <param name="exception">异常</param>
    private void SendErrorPage(Exception exception)
    {
      var httpException = exception as HttpException;
      var response = HttpContext.Current.Response;
      response.Status = httpException != null ? httpException.Code : HttpStatusCode.InternalServerError;
      response.Reason = exception.Message;

      if (response.Body.CanWrite)
        response.Body.SetLength(0);

      var args = new ErrorPageEventArgs(HttpContext.Current) { Exception = exception };
      // 发送异常错误页面请求事件通知
      ErrorPageRequested(this, args);

      try
      {
        var generator = new ResponseWriter();
        if (args.IsHandled)
          generator.Send(HttpContext.Current, response);
        else
          generator.SendErrorPage(HttpContext.Current, response, exception);
      }
      catch (Exception err)
      {
        Logger.Error("Failed to display error page");
        ExceptionHandler.Handle(err);
      }
    }

    #endregion
  }
}