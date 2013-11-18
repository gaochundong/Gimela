using System;

namespace Gimela.Net.Http
{
  /// <summary>
  /// A request have been received.
  /// </summary>
  /// <remarks>
  /// </remarks>
  public class RequestEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestEventArgs"/> class.
    /// </summary>
    /// <param name="context">context that received the request.</param>
    /// <param name="request">Received request.</param>
    /// <param name="response">Response to send.</param>
    public RequestEventArgs(IHttpContext context, IRequest request, IResponse response)
    {
      Context = context;
      Response = response;
      Request = request;
    }

    /// <summary>
    /// Gets context that received the request.
    /// </summary>
    /// <remarks>
    /// Do not forget to set <see cref="IsHandled"/> to <c>true</c> if you are sending
    /// back a response manually through <see cref="IHttpContext.Stream"/>.
    /// </remarks>
    public IHttpContext Context { get; private set; }

    /// <summary>
    /// Gets or sets if the request have been handled.
    /// </summary>
    /// <remarks>
    /// The library will not attempt to send the response object
    /// back to the client if this property is set to <c>true</c>.
    /// </remarks>
    public bool IsHandled { get; set; }

    /// <summary>
    /// Gets request object.
    /// </summary>
    public IRequest Request { get; private set; }

    /// <summary>
    /// Gets response object.
    /// </summary>
    public IResponse Response { get; private set; }
  }
}