using System;
using Gimela.Text;

namespace Gimela.Net.Http.Headers.Parsers
{
  /// <summary>
  /// Parses <see cref="ConnectionHeader"/>.
  /// </summary>
  [ParserFor(ConnectionHeader.ConnectionName)]
  public class ConnectionParser : IHeaderParser
  {
    #region IHeaderParser Members

    /// <summary>
    /// Parse a header
    /// </summary>
    /// <param name="name">Name of header.</param>
    /// <param name="reader">Reader containing value.</param>
    /// <returns>HTTP Header</returns>
    /// <exception cref="FormatException">Header value is not of the expected format.</exception>
    public IHeader Parse(string name, ITextReader reader)
    {
      string typeStr = reader.ReadToEnd(",;");
      if (reader.Current == ',') // to get rid of the TE header.
        reader.ReadToEnd(';');

      ConnectionType type;

      try
      {
        type = (ConnectionType)Enum.Parse(typeof(ConnectionType), typeStr.Replace("-", string.Empty), true);
      }
      catch (ArgumentException err)
      {
        throw new FormatException("Unknown connection type '" + typeStr + "'.", err);
      }

      // got parameters
      if (reader.Current == ';')
      {
        HeaderParameterCollection parameters = HeaderParameterCollection.Parse(reader);
        return new ConnectionHeader(type, parameters);
      }

      return new ConnectionHeader(type);
    }

    #endregion
  }
}