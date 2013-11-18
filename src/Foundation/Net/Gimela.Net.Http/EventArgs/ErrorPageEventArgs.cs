using System;

namespace Gimela.Net.Http
{
  /// <summary>
  /// Arguments for <see cref="Server.ErrorPageRequested"/>.
  /// </summary>
  public class ErrorPageEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorPageEventArgs"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public ErrorPageEventArgs(IHttpContext context)
    {
      Context = context;
    }

    internal IHttpContext Context { get; private set; }

    /// <summary>
    /// Gets or sets thrown exception
    /// </summary>
    public Exception Exception { get; internal set; }

    /// <summary>
    /// Gets or sets if error page was provided.
    /// </summary>
    public bool IsHandled { get; set; }

    /// <summary>
    /// Gets requested resource.
    /// </summary>
    public IRequest Request { get { return Context.Request; } }

    /// <summary>
    /// Gets response to send
    /// </summary>
    public IResponse Response { get { return Context.Response; } }
  }
}