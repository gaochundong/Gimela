using System;
using System.Collections.Generic;
using System.Reflection;
using Gimela.Net.Http.Headers.Parsers;
using Gimela.Infrastructure.Patterns;
using Gimela.Text;

namespace Gimela.Net.Http.Headers
{
  /// <summary>
  /// Used to build headers.
  /// </summary>
  public class HeaderFactory
  {
    private readonly Dictionary<string, IHeaderParser> _parsers =
        new Dictionary<string, IHeaderParser>(StringComparer.OrdinalIgnoreCase);

    private readonly FlyweightObjectPool<StringReader> _readers =
        new FlyweightObjectPool<StringReader>(() => new StringReader(string.Empty));

    private readonly IHeaderParser _stringParser = new StringParser();


    /// <summary>
    /// Add a parser
    /// </summary>
    /// <param name="name">Header that the parser is for.</param>
    /// <param name="parser">Parser implementation</param>
    /// <remarks>
    /// Will replace any existing parser for the specified header.
    /// </remarks>
    public void Add(string name, IHeaderParser parser)
    {
      _parsers[name] = parser;
    }

    /// <summary>
    /// Add all default (built-in) parsers.
    /// </summary>
    /// <remarks>
    /// Will not replace previously added parsers.
    /// </remarks>
    public void AddDefaultParsers()
    {
      Type interfaceType = typeof(IHeaderParser);
      foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
      {
        if (type.IsInterface || type.IsAbstract)
          continue;

        if (!interfaceType.IsAssignableFrom(type))
          continue;

        CreateParser(type);
      }
    }

    /// <summary>
    /// Create a header parser
    /// </summary>
    /// <param name="type"><see cref="IHeaderParser"/> implementation.</param>
    /// <remarks>
    /// <para>
    /// Uses <see cref="ParserForAttribute"/> attribute to find which headers
    /// the parser is for.
    /// </para>
    /// <para>Will not replace previously added parsers.</para>
    /// </remarks>
    private void CreateParser(Type type)
    {
      var parser = (IHeaderParser)Activator.CreateInstance(type);

      object[] attributes = type.GetCustomAttributes(true);
      foreach (object attr in attributes)
      {
        var attribute = attr as ParserForAttribute;
        if (attribute == null)
          continue;

        // do not replace already added parsers.
        if (_parsers.ContainsKey(attribute.HeaderName))
          continue;

        _parsers[attribute.HeaderName] = parser;
      }
    }

    /// <summary>
    /// Parse a header.
    /// </summary>
    /// <param name="name">Name of header</param>
    /// <param name="value">Header value</param>
    /// <returns>Header.</returns>
    /// <exception cref="FormatException">Value is not a well formatted header value.</exception>
    public IHeader Parse(string name, string value)
    {
      IHeaderParser parser;
      if (!_parsers.TryGetValue(name, out parser))
        parser = _stringParser;

      StringReader reader = _readers.Dequeue();
      reader.Assign(value);
      try
      {
        return parser.Parse(name, reader);
      }
      finally
      {
        _readers.Enqueue(reader);
      }
    }
  }
}