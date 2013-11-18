
namespace Gimela.Net.Http.Headers
{
  /// <summary>
  ///   The Cache-Control general-header field is used to specify directives that
  ///   MUST be obeyed by all caching mechanisms along the request/response
  ///   chain. .
  /// </summary>
  /// <remarks>
  /// <para>
  /// The directives specify behavior intended to prevent caches from adversely
  /// interfering with the request or response. These directives typically
  /// override the default caching algorithms. Cache directives are
  /// unidirectional in that the presence of a directive in a request does not
  /// imply that the same directive is to be given in the response.
  ///</para><para>Note that HTTP/1.0 caches might not implement Cache-Control and
  ///might only implement Pragma: no-cache (see section 14.32 in RFC2616).
  ///</para><para>Cache directives MUST be passed through by a proxy or gateway
  ///application, regardless of their significance to that application, since the
  ///directives might be applicable to all recipients along the request/response
  ///chain. It is not possible to specify a cache- directive for a specific cache
  /// </para>
  /// <para>
  ///   When a directive appears without any 1#field-name parameter, the
  ///   directive applies to the entire request or response. When such a
  ///   directive appears with a 1#field-name parameter, it applies only to
  ///   the named field or fields, and not to the rest of the request or
  ///   response. This mechanism supports extensibility; implementations of
  ///   future versions of the HTTP protocol might apply these directives to
  ///   header fields not defined in HTTP/1.1.
  /// </para>
  /// <para>
  ///   The cache-control directives can be broken down into these general
  ///   categories:
  /// <list type="bullet">
  /// <item>
  ///      Restrictions on what are cacheable; these may only be imposed by
  ///      the origin server.
  ///</item><item>
  ///      Restrictions on what may be stored by a cache; these may be
  ///      imposed by either the origin server or the user agent.
  ///</item><item>
  ///      Modifications of the basic expiration mechanism; these may be
  ///      imposed by either the origin server or the user agent.
  ///</item><item>
  ///      Controls over cache revalidation and reload; these may only be
  ///      imposed by a user agent.
  ///</item><item>
  ///      Control over transformation of entities.
  ///</item><item>
  ///      Extensions to the caching system.
  /// </item>
  /// </list>
  ///	</para>
  /// </remarks>
  public class CacheControlHeader : IHeader
  {
    /// <summary>
    /// Header name
    /// </summary>
    public const string CacheControlName = "Cache-Control";

    #region IHeader Members

    /// <summary>
    /// Gets header name
    /// </summary>
    public string Name
    {
      get { return CacheControlName; }
    }

    /// <summary>
    /// Gets value as it would be sent back to client.
    /// </summary>
    public string HeaderValue
    {
      get { return "NotImplementedYet"; }
    }

    #endregion
  }
}