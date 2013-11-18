using System;

namespace Gimela.Net.Http.Headers
{
  /// <summary>
  /// Authorization response
  /// </summary>
  /// <remarks>
  /// <para>
  /// A user agent that wishes to authenticate itself with a server--
  /// usually, but not necessarily, after receiving a 401 response--does
  /// so by including an Authorization request-header field with the
  /// request.  The Authorization field value consists of credentials
  /// containing the authentication information of the user agent for
  /// the realm of the resource being requested.
  /// </para>
  /// <example>
  ///     Authorization  = "Authorization" ":" credentials
  /// </example>
  /// <para>
  /// HTTP access authentication is described in "HTTP Authentication:
  /// Basic and Digest Access Authentication" [43]. If a request is
  /// authenticated and a realm specified, the same credentials SHOULD
  /// be valid for all other requests within this realm (assuming that
  /// the authentication scheme itself does not require otherwise, such
  /// as credentials that vary according to a challenge value or using
  /// synchronized clocks).
  /// When a shared cache (see section 13.7) receives a request
  /// containing an Authorization field, it MUST NOT return the
  /// corresponding response as a reply to any other request, unless one
  /// of the following specific exceptions holds:
  /// </para>
  /// <list type="number">
  /// <item>
  ///  If the response includes the "s-maxage" cache-control
  ///    directive, the cache MAY use that response in replying to a
  ///    subsequent request. But (if the specified maximum age has
  ///    passed) a proxy cache MUST first revalidate it with the origin
  ///    server, using the request-headers from the new request to allow
  ///    the origin server to authenticate the new request. (This is the
  ///    defined behavior for s-maxage.) If the response includes "s-
  ///    maxage=0", the proxy MUST always revalidate it before re-using
  ///    it.
  /// </item><item>
  ///  If the response includes the "must-revalidate" cache-control
  ///    directive, the cache MAY use that response in replying to a
  ///    subsequent request. But if the response is stale, all caches
  ///    MUST first revalidate it with the origin server, using the
  ///    request-headers from the new request to allow the origin server
  ///    to authenticate the new request.
  /// </item><item>
  ///  If the response includes the "public" cache-control directive,
  ///    it MAY be returned in reply to any subsequent request.
  /// </item>
  /// </list>
  /// </remarks>
  public class AuthorizationHeader : IHeader
  {
    /// <summary>
    /// Name constant
    /// </summary>
    public const string AuthorizationName = "Authorization";

    /// <summary>
    /// Gets or sets authentication data.
    /// </summary>
    public string Data { get; set; }

    /// <summary>
    /// Gets or sets authentication protocol.
    /// </summary>
    public string Scheme { get; set; }

    #region IHeader Members

    /// <summary>
    /// Gets name of header.
    /// </summary>
    public string Name
    {
      get { return AuthorizationName; }
    }

    /// <summary>
    /// Gets value as it would be sent back to client.
    /// </summary>
    public string HeaderValue
    {
      get { throw new NotImplementedException(); }
    }

    #endregion
  }
}