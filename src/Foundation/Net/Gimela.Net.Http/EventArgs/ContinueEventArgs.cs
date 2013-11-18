using System;

namespace Gimela.Net.Http
{
  /// <summary>
  /// Used to notify about 100-continue header.
  /// </summary>
  public class ContinueEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ContinueEventArgs"/> class.
    /// </summary>
    /// <param name="request">request that want to continue.</param>
    public ContinueEventArgs(IRequest request)
    {
      Request = request;
    }

    /// <summary>
    /// Gets request that want to continue
    /// </summary>
    public IRequest Request { get; private set; }
  }
}
