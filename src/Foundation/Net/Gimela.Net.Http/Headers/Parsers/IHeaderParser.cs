using System;
using Gimela.Text;

namespace Gimela.Net.Http.Headers.Parsers
{
  /// <summary>
  /// Used to parse header values
  /// </summary>
  public interface IHeaderParser
  {
    /// <summary>
    /// Parse a header
    /// </summary>
    /// <param name="name">Name of header.</param>
    /// <param name="reader">Reader containing value.</param>
    /// <returns>HTTP Header</returns>
    /// <exception cref="FormatException">Header value is not of the expected format.</exception>
    IHeader Parse(string name, ITextReader reader);
  }
}