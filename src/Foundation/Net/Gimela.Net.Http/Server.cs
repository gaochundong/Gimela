using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Gimela.Common.ExceptionHandling;
using Gimela.Common.Logging;
using Gimela.Net.Http.Authentication;
using Gimela.Net.Http.BodyDecoders;
using Gimela.Net.Http.Headers;
using Gimela.Net.Http.Messages;
using Gimela.Net.Http.Modules;
using Gimela.Net.Http.Routing;

namespace Gimela.Net.Http
{
  /// <summary>
  /// HTTP服务器
  /// </summary>
  public class Server
  {
    #region Fields
    
    [ThreadStatic]
    private static Server _server; // 每线程唯一

    private readonly BodyDecoderCollection _bodyDecoders = new BodyDecoderCollection();
    private readonly List<IHttpListener> _listeners = new List<IHttpListener>();
    private readonly List<IModule> _modules = new List<IModule>();
    private readonly List<IRouter> _routers = new List<IRouter>();

    private HttpFactory _factory;
    private bool _isStarted;

    #endregion

    #region Ctors

    /// <summary>
    /// 初始化HTTP服务器的实例
    /// </summary>
    /// <param name="serverName">Name of the server.</param>
    public Server(string serverName)
      : this(serverName, new HttpFactory())
    {
    }

    /// <summary>
    /// 初始化HTTP服务器的实例
    /// </summary>
    /// <param name="serverName">Name of the server.</param>
    /// <param name="factory">Factory used to create objects used in this library.</param>
    public Server(string serverName, HttpFactory factory)
    {
      _server = this;
      _factory = factory;

      ServerName = serverName;

      AuthenticationProvider = new AuthenticationProvider();
      MaxContentSize = 1000000;
    }

    #endregion

    #region Properties

    /// <summary>
    /// 获取当前Http服务器
    /// </summary>
    /// <remarks>
    /// Only valid when a request have been received and is being processed.
    /// </remarks>
    public static Server Current
    {
      get { return _server; }
    }

    /// <summary>
    /// 服务器名称，用于填充请求头部Server字段
    /// </summary>
    /// <remarks>
    /// Used in the "Server" header when serving requests.
    /// </remarks>
    public string ServerName { get; private set; }

    /// <summary>
    /// 鉴权提供器
    /// </summary>
    /// <remarks>
    /// A authentication provider is used to keep track of all authentication types
    /// that can be used.
    /// </remarks>
    public AuthenticationProvider AuthenticationProvider { get; private set; }

    /// <summary>
    /// Body内容长度限制
    /// </summary>
    /// <remarks>
    /// <para>
    /// Used to determine the answer to a 100-continue request.
    /// </para>
    /// <para>
    ///  0 = turned off.
    /// </para>
    /// </remarks>
    public int ContentLengthLimit { get; set; }

    /// <summary>
    /// 请求消息体的最大Size (in bytes)
    /// </summary>
    public long MaxContentSize { get; set; }

    #endregion

    #region Server

    /// <summary>
    /// 启动HTTP服务器
    /// </summary>
    /// <param name="backLog">允许的等待连接数</param>
    public void Start(int backLog)
    {
      if (_isStarted)
        return;

      // 添加默认的消息体解析器
      if (_bodyDecoders.Count == 0)
      {
        _bodyDecoders.Add(new MultiPartDecoder());
        _bodyDecoders.Add(new UrlDecoder());
      }

      // 消息监听器由外部应用在启动前添加
      foreach (IHttpListener listener in _listeners)
      {
        listener.ErrorPageRequested += OnListenerErrorPageRequested;
        listener.RequestReceived += OnListenerRequestReceived;
        listener.ContentLengthLimit = this.ContentLengthLimit;

        // 监听器中包含被监听的地址和端口
        listener.Start(backLog);
      }

      _isStarted = true;
    }

    /// <summary>
    /// 停止HTTP服务器
    /// </summary>
    /// <param name="removeModules">是否移除所有模块</param>
    public void Stop(bool removeModules)
    {
      foreach (IHttpListener listener in _listeners)
      {
        listener.Stop();
      }

      if (removeModules)
        _modules.Clear();
    }

    #endregion

    #region Listener Handlers

    private void OnListenerErrorPageRequested(object sender, ErrorPageEventArgs e)
    {
      _server = this;

      DisplayErrorPage(e.Context, e.Exception);
      e.IsHandled = true;
    }

    private void OnListenerRequestReceived(object sender, RequestEventArgs e)
    {
      _server = this;

      Exception exception;
      try
      {
        ProcessingResult result = HandleRequest(e);
        if (result != ProcessingResult.Continue)
          return;

        exception = null;
      }
      catch (HttpException err)
      {
        Logger.Error("Got an HTTP exception.");
        ExceptionHandler.Handle(err);
        e.Response.Status = err.Code;
        e.Response.Reason = err.Message;
        exception = err;
      }
      catch (Exception err)
      {
        Logger.Error("Got an unhandled exception.");
        ExceptionHandler.Handle(err);
        exception = err;
        e.Response.Status = HttpStatusCode.InternalServerError;
        e.Response.Reason = "Failed to process request.";
      }

      if (exception == null)
      {
        e.Response.Status = HttpStatusCode.NotFound;
        e.Response.Reason = "Requested resource is not found.";
        exception = new HttpException(HttpStatusCode.NotFound, "Failed to find uri " + e.Request.Uri);
      }
      DisplayErrorPage(e.Context, exception);
      e.IsHandled = true;
    }

    #endregion

    #region Public Methods
    
    /// <summary>
    /// 增加新的消息解析器
    /// </summary>
    /// <param name="decoder">decoder to add</param>
    /// <remarks>
    /// Adding zero decoders will make the server add the 
    /// default ones which is <see cref="MultiPartDecoder"/> and <see cref="UrlDecoder"/>.
    /// </remarks>
    public void Add(IBodyDecoder decoder)
    {
      _bodyDecoders.Add(decoder);
    }

    /// <summary>
    /// 增加新的路由器
    /// </summary>
    /// <param name="router">Router to add</param>
    /// <exception cref="InvalidOperationException">Server have been started.</exception>
    public void Add(IRouter router)
    {
      if (_isStarted)
        throw new InvalidOperationException("Server have been started.");

      _routers.Add(router);
    }

    /// <summary>
    /// 增加新的模块
    /// </summary>
    /// <param name="module">Module to add</param>
    /// <exception cref="ArgumentNullException"><c>module</c> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Cannot add modules when server have been started.</exception>
    public void Add(IModule module)
    {
      if (module == null)
        throw new ArgumentNullException("module");
      if (_isStarted)
        throw new InvalidOperationException("Cannot add modules when server have been started.");

      _modules.Add(module);
    }

    /// <summary>
    /// 增加新的Http监听器
    /// </summary>
    /// <param name="listener"></param>
    /// <exception cref="InvalidOperationException">Listener have been started.</exception>
    public void Add(IHttpListener listener)
    {
      if (listener.IsStarted)
        throw new InvalidOperationException("Listener have been started.");

      _listeners.Add(listener);
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// 有错误发生，发送错误显示页面至客户端
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="exception">The exception.</param>
    /// <remarks>
    /// Invoke base class (<see cref="Server"/>) to send the contents
    /// of <see cref="IHttpContext.Response"/>.
    /// </remarks>
    protected virtual void DisplayErrorPage(IHttpContext context, Exception exception)
    {
      var httpException = exception as HttpException;
      if (httpException != null)
      {
        context.Response.Reason = httpException.Code.ToString();
        context.Response.Status = httpException.Code;
      }
      else
      {
        context.Response.Reason = "Internal Server Error";
        context.Response.Status = HttpStatusCode.InternalServerError;
      }

      var args = new ErrorPageEventArgs(context) { Exception = exception };
      ErrorPageRequested(this, args);

      if (args.IsHandled)
      {
        // 外部已处理
        var writer = HttpFactory.Current.Get<ResponseWriter>();
        writer.Send(context, context.Response);
      }
      else
      {
        // 外部未处理，发送异常内容
        var writer = HttpFactory.Current.Get<ResponseWriter>();
        writer.SendErrorPage(context, context.Response, exception);
        args.IsHandled = true;
      }
    }

    /// <summary>
    /// 处理Http请求
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private ProcessingResult HandleRequest(RequestEventArgs e)
    {
      var context = new RequestContext
                        {
                          HttpContext = e.Context,
                          Request = e.Request,
                          Response = e.Response
                        };

      // 鉴权
      AuthenticateRequest(context);

      // 前期处理
      OnBeforeProcessRequest(context);

      // 通知外部请求到达
      BeforeProcessRequest(this, e);

      // 解析消息体
      if (e.Request.ContentLength.Value > 0)
      {
        DecodeBody(e.Request);
      }

      ProcessingResult result = ProcessingResult.Continue;

      // 由路由器处理消息
      result = ProcessRouters(context);
      if (ProcessResult(result, e))
      {
        Logger.Debug("Routers processed the request.");
      }

      // 有模块处理消息
      result = ProcessModules(context);
      if (ProcessResult(result, e))
      {
        return result;
      }

      // 通知请求已收到，由外部模块处理请求
      RequestReceived(this, e);

      return ProcessingResult.Continue;
    }

    /// <summary>
    /// 查看消息头鉴权内容
    /// </summary>
    /// <param name="context">The context.</param>
    /// <remarks>
    /// Looks after a <see cref="AuthorizationHeader"/> in the request and will
    /// use the <see cref="AuthenticationProvider"/> if found.
    /// </remarks>
    protected virtual void AuthenticateRequest(RequestContext context)
    {
      var authHeader = (AuthorizationHeader)context.Request.Headers[AuthorizationHeader.AuthorizationName];
      if (authHeader != null)
      {
        AuthenticationProvider.Authenticate(context.Request);
      }
    }

    /// <summary>
    /// 在请求消息已到达，准备处理之前的准备工作
    /// </summary>
    /// <param name="context">The context.</param>
    /// <remarks>
    /// Default implementation adds a <c>Date</c> header and <c>Server</c> header.
    /// </remarks>
    protected virtual void OnBeforeProcessRequest(RequestContext context)
    {
      if (context.Request.ContentLength.Value > MaxContentSize)
        throw new HttpException(HttpStatusCode.RequestEntityTooLarge, "Too large body, limit is " + MaxContentSize);

      context.Response.Add(new DateHeader("Date", DateTime.UtcNow));
      context.Response.Add(new StringHeader("Server", ServerName));
    }

    /// <summary>
    /// 解析消息体
    /// </summary>
    /// <param name="request">请求消息</param>
    protected virtual void DecodeBody(IRequest request)
    {
      // 消息编码格式
      Encoding encoding = null;
      if (request.ContentType != null)
      {
        string encodingStr = request.ContentType.Parameters["Encoding"];
        if (!string.IsNullOrEmpty(encodingStr))
          encoding = Encoding.GetEncoding(encodingStr);
      }

      // 默认UTF8编码
      if (encoding == null)
        encoding = Encoding.UTF8;

      // 由指定的消息体解析器解析消息体
      DecodedData data = _bodyDecoders.Decode(request.Body, request.ContentType, encoding);
      if (data == null)
        return;

      // 违反了里氏替换原则
      if (!(request is Request))
        throw new InternalServerException("Request object has to derive from Request (sorry for breaking LSP).");

      var r = (Request)request;
      r.Form = data.Parameters;
      r.Files = data.Files;      
    }

    /// <summary>
    /// 由路由器处理请求消息
    /// </summary>
    /// <param name="context">Request context.</param>
    /// <returns>Processing result.</returns>
    private ProcessingResult ProcessRouters(RequestContext context)
    {
      foreach (IRouter router in _routers)
      {
        if (router.Process(context) != ProcessingResult.SendResponse)
          continue;

        Logger.Debug(router.GetType().Name + " sends the response.");
        return ProcessingResult.SendResponse;
      }

      return ProcessingResult.Continue;
    }

    /// <summary>
    /// 由模块处理请求消息
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private ProcessingResult ProcessModules(RequestContext context)
    {
      foreach (IModule module in _modules)
      {
        ProcessingResult result = module.Process(context);
        if (result != ProcessingResult.Continue)
        {
          Logger.Debug(module.GetType().Name + ": " + result);
          return result;
        }
      }

      return ProcessingResult.Continue;
    }

    /// <summary>
    /// 检查处理结果，是否需要发送响应消息
    /// Process result (check if it should be sent back or not)
    /// </summary>
    /// <param name="result"></param>
    /// <param name="e"></param>
    /// <returns><c>true</c> if request was processed properly.; otherwise <c>false</c>.</returns>
    protected virtual bool ProcessResult(ProcessingResult result, RequestEventArgs e)
    {
      if (result == ProcessingResult.Abort)
      {
        e.IsHandled = true;
        return true;
      }

      if (result == ProcessingResult.SendResponse)
      {
        // 发送响应消息
        SendResponse(e.Context, e.Request, e.Response);

        e.IsHandled = true;
        return true;
      }

      return false;
    }

    /// <summary>
    /// 发送响应消息
    /// </summary>
    /// <param name="context"></param>
    /// <param name="request"></param>
    /// <param name="response"></param>
    protected void SendResponse(IHttpContext context, IRequest request, IResponse response)
    {
      BeforeSendResponse(this, new RequestEventArgs(context, request, response));

      var generator = HttpFactory.Current.Get<ResponseWriter>();
      generator.Send(context, response);
      if (request.Connection != null && request.Connection.Type == ConnectionType.Close)
      {
        context.Stream.Close();
        Logger.Debug("Closing connection.");
      }
    }

    #endregion

    #region Events

    /// <summary>
    /// 在请求被尝试处理前发生
    /// </summary>
    /// <remarks>
    /// Event can be used to load a session from a cookie or to force
    /// authentication or anything other you might need t do before a request
    /// is handled.
    /// </remarks>
    public event EventHandler<RequestEventArgs> BeforeProcessRequest = delegate { };

    /// <summary>
    /// 在请求已被所有路由器和模块尝试处理后发生
    /// </summary>
    /// <remarks>
    /// The event can be used to handle the request after all routes and modules
    /// have tried to process the request.
    /// </remarks>
    public event EventHandler<RequestEventArgs> RequestReceived = delegate { };

    /// <summary>
    /// 在响应发回至客户端前发生
    /// </summary>
    public event EventHandler<RequestEventArgs> BeforeSendResponse = delegate { };

    /// <summary>
    /// 在请求错误页发送至客户端时发生，可通过可接口定制错误页面，否则发送默认内容
    /// </summary>
    public event EventHandler<ErrorPageEventArgs> ErrorPageRequested = delegate { };

    #endregion
  }
}