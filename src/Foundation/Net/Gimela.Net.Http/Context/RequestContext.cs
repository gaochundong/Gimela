namespace Gimela.Net.Http
{
  /// <summary>
  /// Request context
  /// </summary>
  /// <remarks>
  /// Contains information about a HTTP request and where it came from.
  /// </remarks>
  public class RequestContext
  {
    /// <summary>
    /// Gets or sets http context.
    /// </summary>
    public IHttpContext HttpContext { get; set; }

    /// <summary>
    /// Gets or sets http request.
    /// </summary>
    public IRequest Request { get; set; }

    /// <summary>
    /// Gets or sets http response.
    /// </summary>
    public IResponse Response { get; set; }
  }
}