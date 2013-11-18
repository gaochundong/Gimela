using System;
using System.Net;

namespace Gimela.Net.Http.Messages.Parsers
{
  /// <summary>
  /// First line in a response have been received
  /// </summary>
  public class ResponseLineEventArgs : EventArgs
  {
    /// <summary>
    /// Gets or sets motivation to why the status code was used.
    /// </summary>
    public string ReasonPhrase { get; set; }

    /// <summary>
    /// Gets or sets message status code
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Gets or sets sip protocol version used.
    /// </summary>
    public string Version { get; set; }
  }
}