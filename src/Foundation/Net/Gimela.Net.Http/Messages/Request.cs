using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gimela.Net.Http.Headers;
using Gimela.Net.Http.Parameters.Parsers;

namespace Gimela.Net.Http.Messages
{
  /// <summary>
  /// Request implementation.
  /// </summary>
  internal class Request : IRequest, IDisposable
  {
    private readonly HeaderCollection _headers;
    private NumericHeader _contentLength = new NumericHeader("Content-Length", 0);
    private RequestCookieCollection _cookies;
    private IParameterCollection _form;
    private string _bodyFileName;

    /// <summary>
    /// Initializes a new instance of the <see cref="Request"/> class.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="path">The path.</param>
    /// <param name="version">The version.</param>
    public Request(string method, string path, string version)
    {
      Body = new MemoryStream();
      Method = method;
      HttpVersion = version;
      Encoding = Encoding.UTF8;

      // HttpFactory is not set during tests.
      HeaderFactory headerFactory = HttpFactory.Current == null
                                        ? new HeaderFactory()
                                        : HttpFactory.Current.Get<HeaderFactory>();

      _headers = new HeaderCollection(headerFactory);

      // Parse query string.
      int pos = path.IndexOf("?");
      QueryString = pos != -1 ? ParameterParser.Parse(path.Substring(pos + 1)) : new ParameterCollection();

      Parameters = QueryString;
      Uri = new Uri("http://not.specified.yet" + path);
    }

    /// <summary>
    /// Gets a header.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IHeader this[string name]
    {
      get { return _headers[name]; }
    }

    /// <summary>
    /// Gets request URI.
    /// </summary>
    public Uri Uri { get; private set; }

    #region IRequest Members

    /// <summary>
    /// Gets cookies.
    /// </summary>
    public RequestCookieCollection Cookies
    {
      get { return _cookies ?? (_cookies = new RequestCookieCollection()); }

      private set { _cookies = value; }
    }

    /// <summary>
    /// Gets all uploaded files.
    /// </summary>
    public HttpFileCollection Files { get; internal set; }

    /// <summary>
    /// Gets query string and form parameters
    /// </summary>
    public IParameterCollection Parameters { get; internal set; }

    /// <summary>
    /// Gets form parameters.
    /// </summary>
    public IParameterCollection Form
    {
      get { return _form; }
      internal set
      {
        _form = value;
        Parameters = new ParameterCollection(QueryString, _form);
      }
    }

    /// <summary>
    /// Gets query string.
    /// </summary>
    public IParameterCollection QueryString { get; internal set; }

    /// <summary>
    /// Gets if request is an Ajax request.
    /// </summary>
    public bool IsAjax
    {
      get
      {
        var header = _headers["X-Requested-With"] as StringHeader;
        return header != null && header.Value == "XMLHttpRequest";
      }
    }

    /// <summary>
    /// Gets or sets connection header.
    /// </summary>
    public ConnectionHeader Connection
    {
      get { return (ConnectionHeader)Headers[ConnectionHeader.ConnectionName]; }
      set { _headers[ConnectionHeader.ConnectionName] = value; }
    }

    /// <summary>
    /// Gets or sets HTTP version.
    /// </summary>
    public string HttpVersion { get; set; }

    /// <summary>
    /// Get a header 
    /// </summary>
    /// <typeparam name="T">Type that it should be cast to</typeparam>
    /// <param name="headerName">Name of header</param>
    /// <returns>Header if found and casted properly; otherwise <c>null</c>.</returns>
    public T Get<T>(string headerName) where T : class, IHeader
    {
      return _headers.Get<T>(headerName);
    }

    /// <summary>
    /// Gets or sets HTTP method.
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// Gets requested URI.
    /// </summary>
    Uri IRequest.Uri
    {
      get { return Uri; }
      set { Uri = value; }
    }

    /// <summary>
    /// Kind of content in the body
    /// </summary>
    /// <remarks>Default is <c>text/html</c></remarks>
    public ContentTypeHeader ContentType { get; set; }

    /// <summary>
    /// Gets or sets encoding
    /// </summary>
    public Encoding Encoding
    {
      get;
      set;
    }

    /// <summary>
    /// Add a new header.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void Add(string name, IHeader value)
    {
      string lowerName = name.ToLower();
      if (lowerName == "host")
      {
        var header = (StringHeader)value;
        string method = HttpContext.Current.IsSecure ? "https://" : "http://";
        Uri = new Uri(method + header.Value + Uri.PathAndQuery);
        return;
      }
      if (lowerName == "content-length")
        ContentLength = (NumericHeader)value;
      if (lowerName == "content-type")
      {
        ContentType = (ContentTypeHeader)value;
        string charset = ContentType.Parameters["charset"];
        if (!string.IsNullOrEmpty(charset))
          Encoding = Encoding.GetEncoding(charset);
      }
      if (lowerName == "cookie")
        Cookies = ((CookieHeader)value).Cookies;

      _headers.Add(name, value);
    }

    /// <summary>
    /// Add a new header.
    /// </summary>
    /// <param name="header">Header to add.</param>
    public void Add(IHeader header)
    {
      Add(header.Name, header);
    }

    /// <summary>
    /// Gets headers.
    /// </summary>
    public IHeaderCollection Headers
    {
      get { return _headers; }
    }

    /// <summary>
    /// Gets body stream.
    /// </summary>
    public Stream Body { get; private set; }

    /// <summary>
    /// Size of the body. MUST be specified before sending the header,
    /// unless property Chunked is set to <c>true</c>.
    /// </summary>
    /// <value>
    /// Any specifically assigned value or Body stream length.
    /// </value>
    public NumericHeader ContentLength
    {
      get
      {
        if (Body.Length > 0)
          _contentLength.Value = Body.Length;
        return _contentLength;
      }
      set
      {
        _contentLength = value;


        if (_contentLength.Value <= 2000000)
          return;

        _bodyFileName = Path.GetTempFileName();
        Body = new FileStream(_bodyFileName, FileMode.CreateNew);
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public IEnumerator<IHeader> GetEnumerator()
    {
      return _headers.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion

    public void Dispose()
    {
      if (!string.IsNullOrEmpty(_bodyFileName))
      {
        File.Delete(_bodyFileName);
        _bodyFileName = null;
      }
    }
  }
}