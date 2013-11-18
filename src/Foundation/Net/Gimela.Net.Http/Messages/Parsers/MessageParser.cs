using System;
using System.Net;
using Gimela.Common.Logging;
using Gimela.Text;

namespace Gimela.Net.Http.Messages.Parsers
{
  /// <summary>
  /// HTTP消息解析器，用于解析请求或响应消息
  /// </summary>
  public class MessageParser
  {
    #region Fields
    
    private readonly BodyEventArgs _bodyEventArgs = new BodyEventArgs();
    private readonly HeaderEventArgs _headerEventArgs = new HeaderEventArgs();
    private readonly BufferReader _reader = new BufferReader();
    private readonly RequestLineEventArgs _requestEventArgs = new RequestLineEventArgs();
    private readonly ResponseLineEventArgs _responseEventArgs = new ResponseLineEventArgs();
    
    /// <summary>
    /// 当前正在被解析的缓存
    /// </summary>
    private byte[] _buffer;
    private int _bodyBytesLeft;
    private string _headerName;
    private string _headerValue;

    /// <summary>
    /// 当前使用的解析消息的方法
    /// </summary>
    private ParserMethod _parserMethod;

    #endregion

    #region Ctors
    
    /// <summary>
    /// HTTP消息解析器，用于解析请求或响应消息
    /// </summary>
    public MessageParser()
    {
      _parserMethod = ParseFirstLine;
    }

    #endregion

    #region Properties
    
    /// <summary>
    /// 获取或设置当前的行号
    /// </summary>
    public int LineNumber { get; set; }

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
      Logger.Trace("Parsing " + length + " bytes from offset " + offset + " using " + _parserMethod.Method.Name);

      _buffer = buffer;
      _reader.Assign(buffer, offset, length);

      while (_parserMethod())
      {
        Logger.Trace("Switched parser method to " + _parserMethod.Method.Name + " at index " + _reader.Index);
      }

      return _reader.Index;
    }

    /// <summary>
    /// 重置解析器以供解析新的消息
    /// </summary>
    public void Reset()
    {
      Logger.Info("Resetting..");
      _headerValue = null;
      _headerName = string.Empty;
      _bodyBytesLeft = 0;
      _parserMethod = ParseFirstLine;
    }

    #endregion

    #region Parser Function

    /// <summary>
    /// 解析请求或响应消息的第一行
    /// </summary>
    /// <returns><c>true</c> if first line is well formatted; otherwise <c>false</c>.</returns>
    /// <exception cref="BadRequestException">Invalid request/response line.</exception>
    private bool ParseFirstLine()
    {
      /* HTTP Message Example
        GET /path/file.html HTTP/1.0
        From: someuser@tutorialspoint.com
        User-Agent: HTTPTool/1.0
        [blank line here]

        HTTP/1.0 200 OK
        Date: Fri, 31 Dec 1999 23:59:59 GMT
        Content-Type: text/html
        Content-Length: 1354

        <html>
        <body>
        <h1>Happy New Millennium!</h1>
        (more file contents)
        </body>
        </html>
      */

      _reader.Consume('\r', '\n');

      // Do not contain a complete first line.
      if (!_reader.Contains('\n'))
        return false;

      var words = new string[3];
      words[0] = _reader.ReadUntil(' ');
      _reader.Consume(); // eat delimiter
      words[1] = _reader.ReadUntil(' ');
      _reader.Consume(); // eat delimiter
      words[2] = _reader.ReadLine();
      if (string.IsNullOrEmpty(words[0])
          || string.IsNullOrEmpty(words[1])
          || string.IsNullOrEmpty(words[2]))
        throw new BadRequestException("Invalid request/response line.");

      // 成功找到消息的第一行
      OnFirstLine(words);

      // 指定下一步解析方式
      _parserMethod = GetHeaderName;

      return true;
    }

    /// <summary>
    /// 尝试找到消息头
    /// </summary>
    /// <returns></returns>
    private bool GetHeaderName()
    {
      // empty line. body is begining.
      if (_reader.Current == '\r' && _reader.Peek == '\n')
      {
        // Eat the line break
        _reader.Consume('\r', '\n');

        // Don't have a body?
        if (_bodyBytesLeft == 0)
        {
          OnComplete();
          _parserMethod = ParseFirstLine;
        }
        else
          _parserMethod = GetBody;

        return true;
      }

      _headerName = _reader.ReadUntil(':');
      if (_headerName == null)
        return false;

      _reader.Consume(); // eat colon
      _parserMethod = GetHeaderValue;

      return true;
    }

    /// <summary>
    /// 获取消息头的值
    /// </summary>
    /// <returns></returns>
    /// <remarks>Will also look for multi header values and automatically merge them to one line.</remarks>
    /// <exception cref="ParserException">Content length is not a number.</exception>
    private bool GetHeaderValue()
    {
      // remove white spaces.
      _reader.Consume(' ', '\t');

      // multi line or empty value?
      if (_reader.Current == '\r' && _reader.Peek == '\n')
      {
        _reader.Consume('\r', '\n');

        // empty value.
        if (_reader.Current != '\t' && _reader.Current != ' ')
        {
          OnHeader(_headerName, string.Empty);
          _headerName = null;
          _headerValue = string.Empty;
          _parserMethod = GetHeaderName;
          return true;
        }

        if (_reader.RemainingLength < 1)
          return false;

        // consume one whitespace
        _reader.Consume();

        // and fetch the rest.
        return GetHeaderValue();
      }

      string value = _reader.ReadLine();
      if (value == null)
        return false;

      _headerValue += value;
      if (string.Compare(_headerName, "Content-Length", true) == 0)
      {
        if (!int.TryParse(value, out _bodyBytesLeft))
          throw new ParserException("Content length is not a number.");
      }

      OnHeader(_headerName, value);

      _headerName = null;
      _headerValue = string.Empty;
      _parserMethod = GetHeaderName;
      return true;
    }

    /// <summary>
    /// Parser method to copy all body bytes.
    /// </summary>
    /// <returns></returns>
    /// <remarks>Needed since a TCP packet can contain multiple messages
    /// after each other, or partial messages.</remarks>
    private bool GetBody()
    {
      if (_reader.RemainingLength == 0)
        return false;

      // Got enough bytes to complete body.
      if (_reader.RemainingLength >= _bodyBytesLeft)
      {
        OnBodyBytes(_buffer, _reader.Index, _bodyBytesLeft);
        _reader.Index += _bodyBytesLeft;
        _bodyBytesLeft = 0;
        OnComplete();
        return false;
      }

      // eat remaining bytes.
      OnBodyBytes(_buffer, _reader.Index, _reader.RemainingLength);
      _bodyBytesLeft -= _reader.RemainingLength;
      _reader.Index = _reader.Length; // place it in the end
      return _reader.Index != _reader.Length;
    }

    #endregion

    /// <summary>
    /// 发送接收到消息体事件
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    protected virtual void OnBodyBytes(byte[] bytes, int offset, int count)
    {
      _bodyEventArgs.AssignInternal(bytes, offset, count);
      BodyBytesReceived(this, _bodyEventArgs);
    }

    /// <summary>
    /// 发送已成功解析一个完整的消息事件
    /// </summary>
    protected virtual void OnComplete()
    {
      Reset();
      MessageComplete(this, EventArgs.Empty);
    }

    /// <summary>
    /// 已找到消息的第一行
    /// </summary>
    /// <param name="words">Will always contain three elements.</param>
    /// <remarks>Used to raise the <see cref="RequestLineParsed"/> or <see cref="ResponseLineParsed"/> event 
    /// depending on the words in the array.</remarks>
    /// <exception cref="BadRequestException"><c>BadRequestException</c>.</exception>
    protected virtual void OnFirstLine(string[] words)
    {
      string firstWord = words[0].ToUpper();
      if (firstWord.StartsWith("HTTP"))
      {
        _responseEventArgs.Version = words[0];
        try
        {
          _responseEventArgs.StatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), words[1]);
        }
        catch (ArgumentException err)
        {
          int code;
          if (!int.TryParse(words[1], out code))
            throw new BadRequestException("Status code '" + words[1] + "' is not known.", err);
        }
        _responseEventArgs.ReasonPhrase = words[1];
        ResponseLineParsed(this, _responseEventArgs);
      }
      else
      {
        try
        {
          _requestEventArgs.Method = words[0].ToUpper();
        }
        catch (ArgumentException err)
        {
          throw new BadRequestException("Unrecognized HTTP method: " + words[0], err);
        }

        _requestEventArgs.UriPath = words[1];
        _requestEventArgs.Version = words[2];
        RequestLineParsed(this, _requestEventArgs);
      }
    }

    private void OnHeader(string name, string value)
    {
      _headerEventArgs.Name = name;
      _headerEventArgs.Value = value;
      HeaderParsed(this, _headerEventArgs);
    }

    #region Public Events
    
    /// <summary>
    /// The request line has been parsed.
    /// </summary>
    public event EventHandler<RequestLineEventArgs> RequestLineParsed = delegate { };

    /// <summary>
    /// Response line has been parsed.
    /// </summary>
    public event EventHandler<ResponseLineEventArgs> ResponseLineParsed = delegate { };

    /// <summary>
    /// Parsed a header.
    /// </summary>
    public event EventHandler<HeaderEventArgs> HeaderParsed = delegate { };

    /// <summary>
    /// Received body bytes.
    /// </summary>
    public event EventHandler<BodyEventArgs> BodyBytesReceived = delegate { };

    /// <summary>
    /// 当一个消息已经被成功的解析时发生
    /// </summary>
    public event EventHandler MessageComplete = delegate { };

    #endregion

    #region Nested type: ParserMethod

    /// <summary>
    /// Used to be able to quickly swap parser method.
    /// </summary>
    /// <returns></returns>
    private delegate bool ParserMethod();

    #endregion
  }
}