using System;
using System.IO;
using System.Net;
using Gimela.Net.Http.Headers;
using Gimela.Net.Http.Messages.Parsers;
using Gimela.Common.Logging;

namespace Gimela.Net.Http.Messages
{
  /// <summary>
  /// Creates a single message for one of the end points.
  /// </summary>
  public class MessageFactoryContext : IDisposable
  {
    #region Fields
    
    private readonly HeaderFactory _headerFactory;
    private readonly MessageFactory _messageFactory;
    private readonly MessageParser _httpParser;

    /// <summary>
    /// 当前正在解析的消息
    /// </summary>
    private IMessage _message;

    #endregion

    #region Ctors

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageFactoryContext"/> class.
    /// </summary>
    /// <param name="messageFactory">消息工厂</param>
    /// <param name="headerFactory">头域工厂</param>
    /// <param name="httpParser">指定的HTTP消息解析器</param>
    public MessageFactoryContext(MessageFactory messageFactory, HeaderFactory headerFactory, MessageParser httpParser)
    {
      _messageFactory = messageFactory;
      _headerFactory = headerFactory;
      _httpParser = httpParser;

      _httpParser.HeaderParsed += OnHeader;
      _httpParser.MessageComplete += OnMessageComplete;
      _httpParser.RequestLineParsed += OnRequestLine;
      _httpParser.ResponseLineParsed += OnResponseLine;
      _httpParser.BodyBytesReceived += OnBody;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 解析缓存数据直至无更多内容可供解析
    /// </summary>
    /// <param name="buffer">将被解析的缓存</param>
    /// <param name="offset">在缓存中开始解析的位置</param>
    /// <param name="length">要处理的Byte数量</param>
    /// <returns>解析结束的位置</returns>
    /// <exception cref="ParserException">解析失败时会抛出异常</exception>
    public int Parse(byte[] buffer, int offset, int length)
    {
      return _httpParser.Parse(buffer, offset, length);
    }

    /// <summary>
    /// 重置解析器
    /// </summary>
    /// <remarks>
    /// Something failed, reset parser so it can start on a new request.
    /// </remarks>
    public void Reset()
    {
      _httpParser.Reset();
    }

    #endregion

    #region Public Events

    /// <summary>
    /// 当一个请求消息已经被成功解析时发生
    /// </summary>
    public event EventHandler<FactoryRequestEventArgs> RequestCompleted = delegate { };

    /// <summary>
    /// 当一个响应消息已经被成功解析时发生
    /// </summary>
    public event EventHandler<FactoryResponseEventArgs> ResponseCompleted = delegate { };

    /// <summary>
    /// 客户端询问是否需要继续保持连接
    /// </summary>
    /// <remarks>
    /// If the body is too large or anything like that you should respond <see cref="HttpStatusCode.ExpectationFailed"/>.
    /// </remarks>
    public event EventHandler<ContinueEventArgs> ContinueResponseRequested = delegate { };

    #endregion

    #region Private Methods

    private void OnBody(object sender, BodyEventArgs e)
    {
      _message.Body.Write(e.Buffer, e.Offset, e.Count);
    }

    /// <summary>
    /// Received a header from parser
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnHeader(object sender, HeaderEventArgs e)
    {
      Logger.Trace(e.Name + ": " + e.Value);
      IHeader header = _headerFactory.Parse(e.Name, e.Value);
      _message.Add(header.Name.ToLower(), header);
      if (header.Name.ToLower() == "expect" && e.Value.ToLower().Contains("100-continue"))
      {
        Console.WriteLine("Got 100 continue request.");
        ContinueResponseRequested(this, new ContinueEventArgs((IRequest)_message));
      }
    }

    private void OnMessageComplete(object sender, EventArgs e)
    {
      _message.Body.Seek(0, SeekOrigin.Begin);
      if (_message is IRequest)
        RequestCompleted(this, new FactoryRequestEventArgs((IRequest)_message));
      else
        ResponseCompleted(this, new FactoryResponseEventArgs((IResponse)_message));
    }

    private void OnRequestLine(object sender, RequestLineEventArgs e)
    {
      Logger.Trace(e.Method + ": " + e.UriPath);
      _message = _messageFactory.CreateRequest(e.Method, e.UriPath, e.Version);
    }

    private void OnResponseLine(object sender, ResponseLineEventArgs e)
    {
      Logger.Trace(e.StatusCode + ": " + e.ReasonPhrase);
      _message = _messageFactory.CreateResponse(e.Version, e.StatusCode, e.ReasonPhrase);
    }

    #endregion

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    public void Dispose()
    {
    }

    #endregion
  }
}