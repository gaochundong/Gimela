using System;

namespace Gimela.Net.Http.Messages
{
  /// <summary>
  /// A request have been parsed successfully by the server.
  /// </summary>
  public class FactoryRequestEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryRequestEventArgs"/> class.
    /// </summary>
    /// <param name="request">Received request.</param>
    public FactoryRequestEventArgs(IRequest request)
    {
      Request = request;
    }

    /// <summary>
    /// Gets received request.
    /// </summary>
    public IRequest Request { get; private set; }
  }
}