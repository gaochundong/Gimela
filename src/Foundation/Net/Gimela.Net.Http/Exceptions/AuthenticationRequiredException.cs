using System;
using System.Net;
using Gimela.Net.Http.Authentication;

namespace Gimela.Net.Http
{
  /// <summary>
  /// User needs to authenticate.
  /// </summary>
  /// <seealso cref="ForbiddenException"/>
  /// <seealso cref="AuthenticationProvider"/>
  public class AuthenticationRequiredException : HttpException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class.
    /// </summary>
    /// <param name="errMsg">Exception description.</param>
    public AuthenticationRequiredException(string errMsg)
      : base(HttpStatusCode.Unauthorized, errMsg)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class.
    /// </summary>
    /// <param name="errMsg">Exception description.</param>
    /// <param name="inner">Inner exception.</param>
    protected AuthenticationRequiredException(string errMsg, Exception inner)
      : base(HttpStatusCode.Unauthorized, errMsg, inner)
    {
    }
  }
}
