using System;
using Gimela.Common.Logging;

namespace Gimela.Net.Http.Routing
{
  /// <summary>
  /// redirects from one URL to another.
  /// </summary>
  public class SimpleRouter : IRouter
  {
    private readonly string _fromUrl;
    private readonly bool _shouldRedirect;
    private readonly string _toUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleRouter"/> class.
    /// </summary>
    /// <param name="fromUrl">Absolute path (no server name)</param>
    /// <param name="toUrl">Absolute path (no server name)</param>
    /// <example>
    /// server.Add(new RedirectRule("/", "/user/index"));
    /// </example>
    public SimpleRouter(string fromUrl, string toUrl)
    {
      _fromUrl = fromUrl;
      _toUrl = toUrl;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleRouter"/> class.
    /// </summary>
    /// <param name="fromUrl">Absolute path (no server name)</param>
    /// <param name="toUrl">Absolute path (no server name)</param>
    /// <param name="shouldRedirect"><c>true</c> if request should be redirected, <c>false</c> if the request URI should be replaced.</param>
    /// <example>
    /// server.Add(new RedirectRule("/", "/user/index"));
    /// </example>
    public SimpleRouter(string fromUrl, string toUrl, bool shouldRedirect)
    {
      _fromUrl = fromUrl;
      _toUrl = toUrl;
      _shouldRedirect = shouldRedirect;
    }

    /// <summary>
    /// Gets string to match request URI with.
    /// </summary>
    /// <remarks>Is compared to request.Uri.AbsolutePath</remarks>
    public string FromUrl
    {
      get { return _fromUrl; }
    }

    /// <summary>
    /// Gets whether the server should redirect the client instead of simply modifying the URI.
    /// </summary>
    /// <remarks>
    /// <c>false</c> means that the rule will replace
    /// the current request URI with the new one from this class.
    /// <c>true</c> means that a redirect response is sent to the client.
    /// </remarks>
    public bool ShouldRedirect
    {
      get { return _shouldRedirect; }
    }

    /// <summary>
    /// Gets where to redirect.
    /// </summary>
    public string ToUrl
    {
      get { return _toUrl; }
    }

    #region IRouter Members

    /// <summary>
    /// Process the incoming request.
    /// </summary>
    /// <param name="context">Request context.</param>
    /// <returns>Processing result.</returns>
    /// <exception cref="ArgumentNullException">If any parameter is <c>null</c>.</exception>
    public virtual ProcessingResult Process(RequestContext context)
    {
      IRequest request = context.Request;
      IResponse response = context.Response;

      if (request.Uri.AbsolutePath == FromUrl)
      {
        if (!ShouldRedirect)
        {
          Logger.Debug("Redirecting (internally) from " + FromUrl + " to " + ToUrl);
          request.Uri = new Uri(request.Uri, ToUrl);
          return ProcessingResult.Continue;
        }

        Logger.Debug("Redirecting browser from " + FromUrl + " to " + ToUrl);
        response.Redirect(ToUrl);
        return ProcessingResult.SendResponse;
      }

      return ProcessingResult.Continue;
    }

    #endregion
  }
}