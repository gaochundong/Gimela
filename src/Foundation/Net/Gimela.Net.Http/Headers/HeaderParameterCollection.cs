using System;
using System.Collections.Generic;
using System.Text;
using Gimela.Text;

namespace Gimela.Net.Http.Headers
{
  /// <summary>
  /// Contains parameters for HTTP headers.
  /// </summary>
  public class HeaderParameterCollection
  {
    private readonly Dictionary<string, string> _items =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets a value
    /// </summary>
    /// <param name="name">parameter name</param>
    /// <returns>value if found; otherwise <c>null</c>.</returns>
    public string this[string name]
    {
      get
      {
        string value;
        return _items.TryGetValue(name, out value) ? value : null;
      }
      set { _items[name] = value; }
    }

    /// <summary>
    /// Add a parameter
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="value">value</param>
    /// <remarks>
    /// Existing parameter with the same name will be replaced.
    /// </remarks>
    public void Add(string name, string value)
    {
      _items[name] = value;
    }

    /// <summary>
    /// Parse parameters.
    /// </summary>
    /// <param name="reader">Parser containing buffer to parse.</param>
    /// <returns>A collection with all parameters (or just a empty collection).</returns>
    /// <exception cref="FormatException">Expected a value after equal sign.</exception>
    public static HeaderParameterCollection Parse(ITextReader reader)
    {
      return Parse(reader, ';');
    }

    /// <summary>
    /// Parse parameters.
    /// </summary>
    /// <param name="reader">Parser containing buffer to parse.</param>
    /// <param name="delimiter">Parameter delimiter</param>
    /// <returns>A collection with all parameters (or just a empty collection).</returns>
    /// <exception cref="FormatException">Expected a value after equal sign.</exception>
    public static HeaderParameterCollection Parse(ITextReader reader, char delimiter)
    {
      if (reader.Current == delimiter)
        reader.Consume();
      reader.ConsumeWhiteSpaces();

      var collection = new HeaderParameterCollection();
      string name = reader.ReadToEnd("=" + delimiter);
      while (name != string.Empty && !reader.EOF)
      {
        // got a parameter value
        if (reader.Current == '=')
        {
          reader.ConsumeWhiteSpaces('=');

          string value = reader.Current == '"'
                             ? reader.ReadQuotedString()
                             : reader.ReadToEnd(delimiter);

          reader.ConsumeWhiteSpaces();
          if (value == string.Empty && reader.Current != delimiter)
            throw new FormatException("Expected a value after equal sign.");

          collection.Add(name, value);
        }
        else // got no value
        {
          collection.Add(name, string.Empty);
        }

        reader.ConsumeWhiteSpaces(delimiter); // consume delimiter and white spaces
        name = reader.ReadToEnd("=" + delimiter);
      }
      return collection;
    }


    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      var sb = new StringBuilder();
      foreach (var pair in _items)
        sb.AppendFormat("{0}={1};", pair.Key, pair.Value);
      return sb.ToString();
    }
  }
}