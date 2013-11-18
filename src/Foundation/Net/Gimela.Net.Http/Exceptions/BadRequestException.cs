using System;
using System.Net;

namespace Gimela.Net.Http
{
  /// <summary>
  /// Request couldn't be parsed successfully.
  /// </summary>
  public class BadRequestException : HttpException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    /// <param name="errMsg">Exception description.</param>
    public BadRequestException(string errMsg)
      : base(HttpStatusCode.BadRequest, errMsg)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    /// <param name="errMsg">Exception description.</param>
    /// <param name="inner">Exception description.</param>
    public BadRequestException(string errMsg, Exception inner)
      : base(HttpStatusCode.BadRequest, errMsg, inner)
    {
    }
  }
}