using System;

namespace Gimela.Net.Http.Messages.Parsers
{
  /// <summary>
  /// A response have been received.
  /// </summary>
  public class FactoryResponseEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryResponseEventArgs"/> class.
    /// </summary>
    /// <param name="response">The response.</param>
    public FactoryResponseEventArgs(IResponse response)
    {
      Response = response;
    }

    /// <summary>
    /// Gets or sets response.
    /// </summary>
    public IResponse Response { get; private set; }
  }
}