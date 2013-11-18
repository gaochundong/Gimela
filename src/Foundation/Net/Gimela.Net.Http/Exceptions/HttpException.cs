using System;
using System.Net;

namespace Gimela.Net.Http
{
  /// <summary>
  /// Exception thrown from HTTP server.
  /// </summary>
  public class HttpException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class.
    /// </summary>
    /// <param name="code">HTTP status code.</param>
    /// <param name="errMsg">Exception description.</param>
    public HttpException(HttpStatusCode code, string errMsg)
      : base(errMsg)
    {
      Code = code;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class.
    /// </summary>
    /// <param name="code">HTTP status code.</param>
    /// <param name="errMsg">Exception description.</param>
    /// <param name="inner">Inner exception.</param>
    protected HttpException(HttpStatusCode code, string errMsg, Exception inner)
      : base(errMsg, inner)
    {
      Code = code;
    }

    /// <summary>
    /// Gets HTTP status code.
    /// </summary>
    public HttpStatusCode Code { get; private set; }
  }
}