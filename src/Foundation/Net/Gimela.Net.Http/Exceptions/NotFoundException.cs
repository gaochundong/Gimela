using System;
using System.Net;

namespace Gimela.Net.Http
{
  /// <summary>
  /// Request couldn't be parsed successfully.
  /// </summary>
  public class NotFoundException : HttpException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    /// <param name="errMsg">Exception description.</param>
    public NotFoundException(string errMsg)
      : base(HttpStatusCode.NotFound, errMsg)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    /// <param name="errMsg">Exception description.</param>
    /// <param name="inner">Exception description.</param>
    public NotFoundException(string errMsg, Exception inner)
      : base(HttpStatusCode.NotFound, errMsg, inner)
    {
    }
  }
}