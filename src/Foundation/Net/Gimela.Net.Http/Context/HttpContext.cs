using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Gimela.Common.ExceptionHandling;
using Gimela.Net.Http.Headers;
using Gimela.Net.Http.Messages;
using Gimela.Net.Http.Messages.Parsers;
using Gimela.Net.Http.Transports;
using Gimela.Common.Logging;
using Gimela.Infrastructure.Patterns;

namespace Gimela.Net.Http
{
  /// <summary>
  /// HTTP请求的上下文，由HTTP监听器在接受Socket时创建，用于封装Socket并创建流，从流中接收和发送消息。
  /// </summary>
  public class HttpContext : IHttpContext, IDisposable
  {
    #region Fields

    private static readonly FlyweightObjectPool<byte[]> Buffers = new FlyweightObjectPool<byte[]>(() => new byte[65535]);
    /// <summary>
    /// HTTP请求的上下文
    /// </summary>
    [ThreadStatic]
    private static IHttpContext _context;
    /// <summary>
    /// 流中读取的字节
    /// </summary>
    private readonly byte[] _buffer;
    /// <summary>
    /// HTTP保活计时器
    /// </summary>
    private Timer _keepAlive;
    /// <summary>
    /// HTTP保活超时时间
    /// </summary>
    private int _keepAliveTimeout = 100000; // 100 seconds.

    #endregion

    #region Ctors
    
    /// <summary>
    /// HTTP请求的上下文接口，由HTTP监听器在接受Socket时创建
    /// </summary>
    /// <param name="socket">HTTP监听器接受的Socket</param>
    /// <param name="context">用于解析消息内容的消息工厂上下文</param>
    public HttpContext(Socket socket, MessageFactoryContext context)
    {
      Socket = socket;

      MessageFactoryContext = context;
      MessageFactoryContext.RequestCompleted += OnRequest;
      MessageFactoryContext.ContinueResponseRequested += On100Continue;

      _buffer = Buffers.Dequeue();
    }

    #endregion

    #region Properties
    
    /// <summary>
    /// 获取当前正在执行的HTTP上下文
    /// </summary>
    public static IHttpContext Current
    {
      get { return _context; }
      internal set { _context = value; }
    }

    /// <summary>
    /// 获取用于该HTTP协议栈的依赖注入器
    /// </summary>
    internal HttpFactory HttpFactory { get; set; }

    /// <summary>
    /// 获取用于解析消息内容的消息工厂上下文
    /// </summary>
    internal MessageFactoryContext MessageFactoryContext { get; private set; }

    /// <summary>
    /// 获取HTTP监听器接受的Socket
    /// </summary>
    internal Socket Socket { get; private set; }

    #endregion

    #region IHttpContext Members

    /// <summary>
    /// 获取是否当前上下文使用的是安全连接
    /// </summary>
    public virtual bool IsSecure
    {
      get { return false; }
    }

    /// <summary>
    /// 获取远端终结点
    /// </summary>
    public IPEndPoint RemoteEndPoint
    {
      get { return (IPEndPoint)Socket.RemoteEndPoint; }
    }

    /// <summary>
    /// 获取用于从远端终结点发送和接收数据的流
    /// </summary>
    /// <remarks>
    /// <para>
    /// The stream can be any type of stream, do not assume that it's a network
    /// stream. For instance, it can be a SslStream or a ZipStream.
    /// </para>
    /// </remarks>
    public Stream Stream { get; private set; }

    /// <summary>
    /// 获取当前被处理的请求
    /// </summary>
    /// <value>The request.</value>
    public IRequest Request { get; internal set; }

    /// <summary>
    /// 获取即将发回客户端的响应
    /// </summary>
    /// <value>The response.</value>
    public IResponse Response { get; internal set; }

    /// <summary>
    /// 断开上下文连接
    /// </summary>
    public void Disconnect()
    {
      Close();
    }

    #endregion

    #region Context Control

    /// <summary>
    /// 启动上下文的处理，创建和读取流
    /// </summary>
    /// <exception cref="SocketException">A socket operation failed.</exception>
    /// <exception cref="IOException">Reading from stream failed.</exception>
    internal void Start()
    {
      Stream = CreateStream(Socket);
      Stream.BeginRead(_buffer, 0, _buffer.Length, OnReceive, null);
    }

    /// <summary>
    /// 关闭和断开上下文连接Socket
    /// </summary>
    private void Close()
    {
      lock (this)
      {
        if (Socket == null)
          return;

        try
        {
          if (_keepAlive != null)
          {
            _keepAlive.Dispose();
            _keepAlive = null;
          }

          Socket.Disconnect(true);
          Socket.Close();
          Socket = null;

          Stream.Dispose();
          Stream = null;

          MessageFactoryContext.RequestCompleted -= OnRequest;
          MessageFactoryContext.ContinueResponseRequested -= On100Continue;
          MessageFactoryContext.Reset();
        }
        catch (Exception err)
        {
          Logger.Warning("Failed to close context properly.");
          ExceptionHandler.Handle(err);
        }
      }

      // 当与客户端的连接已经断开时发生
      Disconnected(this, EventArgs.Empty);
    }

    #endregion

    #region Stream

    /// <summary>
    /// 基于Socket创建用于发送和接收数据的流
    /// </summary>
    /// <param name="socket">Socket to wrap</param>
    /// <returns>Stream</returns>
    /// <exception cref="InvalidOperationException">Stream could not be created.</exception>
    protected virtual Stream CreateStream(Socket socket)
    {
      return new ReusableSocketNetworkStream(socket, true);
    }

    /// <summary>
    /// 从Socket连接流中接收到数据
    /// </summary>
    /// <param name="ar"></param>
    private void OnReceive(IAsyncResult ar)
    {
      // 流已经被关闭
      if (Stream == null)
        return;

      _context = this;
      HttpFactory.Current = HttpFactory;

      try
      {
        int bytesLeft = Stream.EndRead(ar);
        if (bytesLeft == 0)
        {
          Logger.Trace("Client disconnected.");
          Close();
          return;
        }

        Logger.Debug(Socket.RemoteEndPoint + " received " + bytesLeft + " bytes.");

        if (bytesLeft < 5000)
        {
          string temp = Encoding.Default.GetString(_buffer, 0, bytesLeft);
          Logger.Trace(temp);
        }

        // 解析流内容
        int offset = ParseBuffer(bytesLeft);
        bytesLeft -= offset;

        // 流中的长度大于一个消息的长度，偏移至流结尾处供下一个消息处理用
        if (bytesLeft > 0)
        {
          Logger.Warning("Moving " + bytesLeft + " from " + offset + " to beginning of array.");
          Buffer.BlockCopy(_buffer, offset, _buffer, 0, bytesLeft);
        }

        // 继续读取流
        Stream.BeginRead(_buffer, 0, _buffer.Length - offset, OnReceive, null);
      }
      catch (ParserException err)
      {
        Logger.Warning(err.ToString());
        var response = new Response("HTTP/1.0", HttpStatusCode.BadRequest, err.Message);
        var generator = HttpFactory.Current.Get<ResponseWriter>();
        generator.SendErrorPage(this, response, err);
        Close();
      }
      catch (Exception err)
      {
        if (!(err is IOException))
        {
          Logger.Error("Failed to read from stream: " + err);
          var responseWriter = HttpFactory.Current.Get<ResponseWriter>();
          var response = new Response("HTTP/1.0", HttpStatusCode.InternalServerError, err.Message);
          responseWriter.SendErrorPage(this, response, err);
        }

        Close();
      }
    }

    #endregion

    #region Message Parser

    /// <summary>
    /// 解析缓存中的消息
    /// </summary>
    /// <param name="bytesLeft"></param>
    /// <returns>offset in buffer where parsing stopped.</returns>
    /// <exception cref="InvalidOperationException">Parsing failed.</exception>
    private int ParseBuffer(int bytesLeft)
    {
      int offset = MessageFactoryContext.Parse(_buffer, 0, bytesLeft);
      bytesLeft -= offset;

      // try another pass if we got bytes left.
      if (bytesLeft <= 0)
        return offset;

      // Continue until offset is not changed.
      int oldOffset = 0;
      while (offset != oldOffset)
      {
        oldOffset = offset;
        Logger.Trace("Parsing from index " + offset + ", " + bytesLeft + " bytes.");
        offset = MessageFactoryContext.Parse(_buffer, offset, bytesLeft);
        bytesLeft -= offset;
      }
      return offset;
    }

    /// <summary>
    /// 消息解析器解析出一个请求消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnRequest(object sender, FactoryRequestEventArgs e)
    {
      _context = this;

      // 根据请求消息内容生成响应消息
      Response = HttpFactory.Current.Get<IResponse>(this, e.Request);
      Logger.Debug("Received '" + e.Request.Method + " " + e.Request.Uri.PathAndQuery + "' from " + Socket.RemoteEndPoint);

      // 如果请求连接中设置了保活
      if (e.Request.Connection != null && e.Request.Connection.Type == ConnectionType.KeepAlive)
      {
        Response.Add(new StringHeader("Keep-Alive", "timeout=5, max=100"));

        // 刷新计时器
        if (_keepAlive != null)
          _keepAlive.Change(_keepAliveTimeout, _keepAliveTimeout);
      }

      // 记录请求消息
      Request = e.Request;

      // 通知处理请求
      CurrentRequestReceived(this, new RequestEventArgs(this, e.Request, Response));
      RequestReceived(this, new RequestEventArgs(this, e.Request, Response));

      // 如果请求连接中设置了保活，记录请求处理的超时时间
      if (Response.Connection.Type == ConnectionType.KeepAlive)
      {
        if (_keepAlive == null)
          _keepAlive = new Timer(OnKeepAliveTimeout, null, _keepAliveTimeout, _keepAliveTimeout);
      }

      // 通知请求处理完毕
      RequestCompleted(this, new RequestEventArgs(this, e.Request, Response));
      CurrentRequestCompleted(this, new RequestEventArgs(this, e.Request, Response));
    }

    /// <summary>
    /// 消息解析器解析出一个100Continue消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void On100Continue(object sender, ContinueEventArgs e)
    {
      ContinueResponseRequested(this, e);
    }

    /// <summary>
    /// 保活超时
    /// </summary>
    /// <param name="state"></param>
    private void OnKeepAliveTimeout(object state)
    {
      if (_keepAlive != null)
        _keepAlive.Dispose();

      Logger.Info("Keep-Alive timeout");

      // 保活超时断开连接
      Disconnect();
    }

    #endregion

    #region Events

    /// <summary>
    /// 当与客户端的连接已经断开时发生
    /// </summary>
    public event EventHandler Disconnected = delegate { };

    /// <summary>
    /// 客户端询问是否需要继续保持连接
    /// </summary>
    /// <remarks>
    /// If the body is too large or anything like that you should respond <see cref="HttpStatusCode.ExpectationFailed"/>.
    /// </remarks>
    public event EventHandler<ContinueEventArgs> ContinueResponseRequested = delegate { };

    /// <summary>
    /// 当接收到一个新的请求消息时发生
    /// </summary>
    public event EventHandler<RequestEventArgs> RequestReceived = delegate { };

    /// <summary>
    /// 当接收到一个新的请求消息时发生
    /// </summary>
    public static event EventHandler<RequestEventArgs> CurrentRequestReceived = delegate { };

    /// <summary>
    /// 当请求消息被处理并且响应消息被发送后发生
    /// </summary>
    public event EventHandler<RequestEventArgs> RequestCompleted = delegate { };

    /// <summary>
    /// 当请求消息被处理并且响应消息被发送后发生
    /// </summary>
    public static event EventHandler<RequestEventArgs> CurrentRequestCompleted = delegate { };

    #endregion

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    public void Dispose()
    {
      Buffers.Enqueue(_buffer);
      Close();
    }

    #endregion
  }
}