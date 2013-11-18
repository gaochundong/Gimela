using System;
using System.Net;

namespace Gimela.Net.Http
{
  /// <summary>
  /// Requested resource may not be accessed.
  /// </summary>
  /// <remarks>
  /// Normally thrown after an authentication attempt have failed too many times.
  /// </remarks>
  /// <seealso cref="AuthenticationRequiredException"/>
  public class ForbiddenException : HttpException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
    /// </summary>
    /// <param name="errMsg">Exception description.</param>
    public ForbiddenException(string errMsg)
      : base(HttpStatusCode.Forbidden, errMsg)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
    /// </summary>
    /// <param name="errMsg">Exception description.</param>
    /// <param name="inner">Inner exception.</param>
    protected ForbiddenException(string errMsg, Exception inner)
      : base(HttpStatusCode.Forbidden, errMsg, inner)
    {
    }
  }
}