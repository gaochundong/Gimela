using System;
using System.Web;
using Gimela.Text;

namespace Gimela.Net.Http.Parameters.Parsers
{
  /// <summary>
  /// Parses query string
  /// </summary>
  public static class ParameterParser
  {
    /// <summary>
    /// Parse a query string
    /// </summary>
    /// <param name="reader">string to parse</param>
    /// <returns>A collection</returns>
    /// <exception cref="ArgumentNullException"><c>reader</c> is <c>null</c>.</exception>
    public static ParameterCollection Parse(ITextReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");

      var parameters = new ParameterCollection();
      while (!reader.EOF)
      {
        string name = HttpUtility.UrlDecode(reader.ReadToEnd("&="));
        char current = reader.Current;
        reader.Consume();
        switch (current)
        {
          case '&':
            parameters.Add(name, string.Empty);
            break;
          case '=':
            {
              string value = reader.ReadToEnd("&");
              reader.Consume();
              parameters.Add(name, HttpUtility.UrlDecode(value));
            }
            break;
          default:
            parameters.Add(name, string.Empty);
            break;
        }
      }

      return parameters;
    }

    /// <summary>
    /// Parse a query string
    /// </summary>
    /// <param name="queryString">string to parse</param>
    /// <returns>A collection</returns>
    /// <exception cref="ArgumentNullException"><c>queryString</c> is <c>null</c>.</exception>
    public static ParameterCollection Parse(string queryString)
    {
      if (queryString == null)
        throw new ArgumentNullException("queryString");
      if (queryString.Length == 0)
        return new ParameterCollection();

      var reader = new StringReader(queryString);
      return Parse(reader);
    }
  }
}