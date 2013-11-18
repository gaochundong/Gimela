using System;

namespace Gimela.Net.Http.Headers
{
  /// <summary>
  /// The Connection general-header field allows the sender to specify options
  /// that are desired for that particular connection and MUST NOT be
  /// communicated by proxies over further connections.
  /// </summary>
  /// <remarks>
  /// <para>
  ///   HTTP/1.1 proxies MUST parse the Connection header field before a
  ///   message is forwarded and, for each connection-token in this field,
  ///   remove any header field(s) from the message with the same name as the
  ///   connection-token. Connection options are signaled by the presence of
  ///   a connection-token in the Connection header field, not by any
  ///   corresponding additional header field(s), since the additional header
  ///   field may not be sent if there are no parameters associated with that
  ///   connection option.
  ///</para><para>
  ///   Message headers listed in the Connection header MUST NOT include
  ///   end-to-end headers, such as Cache-Control.
  ///</para><para>
  ///   HTTP/1.1 defines the "close" connection option for the sender to
  ///   signal that the connection will be closed after completion of the
  ///   response. For example,
  ///<example>
  ///       Connection: close
  ///</example>
  ///   in either the request or the response header fields indicates that
  ///   the connection SHOULD NOT be considered `persistent' (section 8.1)
  ///   after the current request/response is complete.
  ///</para><para>
  ///   HTTP/1.1 applications that do not support persistent connections MUST
  ///   include the "close" connection option in every message.
  ///</para><para>
  ///   A system receiving an HTTP/1.0 (or lower-version) message that
  ///   includes a Connection header MUST, for each connection-token in this
  ///   field, remove and ignore any header field(s) from the message with
  ///   the same name as the connection-token. This protects against mistaken
  ///   forwarding of such header fields by pre-HTTP/1.1 proxies. See section
  ///   19.6.2 in RFC2616.
  /// </para>
  /// </remarks>
  public class ConnectionHeader : IHeader
  {
    /// <summary>
    /// Header name
    /// </summary>
    public const string ConnectionName = "Connection";

    /// <summary>
    /// Default connection header for HTTP/1.0
    /// </summary>
    public static readonly ConnectionHeader Default10 = new ConnectionHeader(ConnectionType.Close);

    /// <summary>
    /// Default connection header for HTTP/1.1
    /// </summary>
    public static readonly ConnectionHeader Default11 = new ConnectionHeader(ConnectionType.KeepAlive);

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionHeader"/> class.
    /// </summary>
    /// <param name="type">Connection type.</param>
    /// <param name="parameters">The parameters.</param>
    public ConnectionHeader(ConnectionType type, HeaderParameterCollection parameters)
    {
      Parameters = parameters;
      Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionHeader"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public ConnectionHeader(ConnectionType type)
    {
      Type = type;
      Parameters = new HeaderParameterCollection();
    }

    /// <summary>
    /// Gets connection parameters.
    /// </summary>
    public HeaderParameterCollection Parameters { get; private set; }

    /// <summary>
    /// Gets or sets connection type
    /// </summary>
    public ConnectionType Type { get; set; }

    /// <summary>
    /// Returns data formatted as a HTTP header value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      return HeaderValue;
    }

    #region IHeader Members

    /// <summary>
    /// Gets header name
    /// </summary>
    public string Name
    {
      get { return ConnectionName; }
    }

    /// <summary>
    /// Gets value as it would be sent back to client.
    /// </summary>
    public string HeaderValue
    {
      get { return Type == ConnectionType.KeepAlive ? "Keep-Alive" : Type.ToString(); }
    }

    #endregion
  }

  /// <summary>
  /// Type of HTTP connection
  /// </summary>
  public enum ConnectionType
  {
    /// <summary>
    /// Connection is closed after each request-response
    /// </summary>
    Close,

    /// <summary>
    /// Connection is kept alive for X seconds (unless another request have been made)
    /// </summary>
    KeepAlive
  }
}