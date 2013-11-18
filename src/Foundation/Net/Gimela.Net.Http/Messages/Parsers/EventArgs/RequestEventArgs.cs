using System;
using System.Net;

namespace Gimela.Net.Http.Messages.Parsers
{
  /// <summary>
  /// A request have been received.
  /// </summary>
  public class RequestEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestEventArgs"/> class.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="endPoint">End point that the request was received from.</param>
    public RequestEventArgs(IRequest request, EndPoint endPoint)
    {
      Request = request;
      RemoteEndPoint = endPoint;
    }

    /// <summary>
    /// End point that the message was received from.
    /// </summary>
    public EndPoint RemoteEndPoint { get; private set; }

    /// <summary>
    /// Received request.
    /// </summary>
    public IRequest Request { get; private set; }
  }
}