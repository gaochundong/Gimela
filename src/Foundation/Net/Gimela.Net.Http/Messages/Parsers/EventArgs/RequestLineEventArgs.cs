using System;

namespace Gimela.Net.Http.Messages.Parsers
{
  /// <summary>
  /// Used when the request line have been successfully parsed.
  /// </summary>
  public class RequestLineEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestLineEventArgs"/> class.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="uriPath">The URI path.</param>
    /// <param name="version">The HTTP version.</param>
    public RequestLineEventArgs(string method, string uriPath, string version)
    {
      Method = method;
      UriPath = uriPath;
      Version = version;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestLineEventArgs"/> class.
    /// </summary>
    public RequestLineEventArgs()
    {
    }

    /// <summary>
    /// Gets or sets HTTP method.
    /// </summary>
    /// <remarks>
    /// Should be one of the methods declared in <see cref="Method"/>.
    /// </remarks>
    public string Method { get; set; }

    /// <summary>
    /// Gets or sets requested URI path.
    /// </summary>
    public string UriPath { get; set; }

    /// <summary>
    /// Gets or sets the version of the SIP protocol that the client want to use.
    /// </summary>
    public string Version { get; set; }
  }
}