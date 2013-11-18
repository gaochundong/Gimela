using System;

namespace Gimela.Net.Http.Headers.Parsers
{
  /// <summary>
  /// Used to define which headers a parse is for.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class ParserForAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ParserForAttribute"/> class.
    /// </summary>
    /// <param name="headerName">Name of the header.</param>
    public ParserForAttribute(string headerName)
    {
      HeaderName = headerName;
    }

    /// <summary>
    /// Gets name of header that this parser is for.
    /// </summary>
    public string HeaderName { get; private set; }
  }
}