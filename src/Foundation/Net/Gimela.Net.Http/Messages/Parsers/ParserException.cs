using System;

namespace Gimela.Net.Http.Messages.Parsers
{
  /// <summary>
  /// Something failed during parsing.
  /// </summary>
  public class ParserException : BadRequestException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ParserException"/> class.
    /// </summary>
    /// <param name="errMsg">Exception description.</param>
    public ParserException(string errMsg)
      : base(errMsg)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParserException"/> class.
    /// </summary>
    /// <param name="errMsg">Exception description.</param>
    /// <param name="inner">Inner exception.</param>
    public ParserException(string errMsg, Exception inner)
      : base(errMsg, inner)
    {
    }
  }
}