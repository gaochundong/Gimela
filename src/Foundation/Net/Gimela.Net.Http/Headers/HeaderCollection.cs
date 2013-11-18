using System;
using System.Collections;
using System.Collections.Generic;

namespace Gimela.Net.Http.Headers
{
  /// <summary>
  /// Collection of headers.
  /// </summary>
  public class HeaderCollection : IHeaderCollection
  {
    private readonly HeaderFactory _factory;

    private readonly Dictionary<string, IHeader> _headers =
        new Dictionary<string, IHeader>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="HeaderCollection"/> class.
    /// </summary>
    /// <param name="factory">Factory used to created headers.</param>
    public HeaderCollection(HeaderFactory factory)
    {
      _factory = factory;
    }

    /// <summary>
    /// Adds a header
    /// </summary>
    /// <remarks>
    /// Will replace any existing header with the same name.
    /// </remarks>
    /// <param name="header">header to add</param>
    /// <exception cref="ArgumentNullException"><c>header</c> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Header name cannot be <c>null</c>.</exception>
    public void Add(IHeader header)
    {
      if (header == null)
        throw new ArgumentNullException("header");
      if (header.Name == null)
        throw new ArgumentException("Header name cannot be null.");
      _headers[header.Name] = header;
    }

    /// <summary>
    /// Add a header.
    /// </summary>
    /// <param name="name">Header name</param>
    /// <param name="value">Header value</param>
    /// <remarks>
    /// Will try to parse the header and create a <see cref="IHeader"/> object.
    /// </remarks>
    /// <exception cref="FormatException">Header value is not correctly formatted.</exception>
    /// <exception cref="ArgumentNullException"><c>name</c> or <c>value</c> is <c>null</c>.</exception>
    public void Add(string name, string value)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (value == null)
        throw new ArgumentNullException("value");
      IHeader header = _factory.Parse(name, value);
      if (header == null)
        throw new FormatException("Failed to parse header " + name + "/" + value + ".");
      Add(header);
    }

    /// <summary>
    /// Add a header.
    /// </summary>
    /// <param name="name">Header name</param>
    /// <param name="value">Header value</param>
    /// <remarks>
    /// Will try to parse the header and create a <see cref="IHeader"/> object.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><c>name</c> or <c>value</c> is <c>null</c>.</exception>
    public void Add(string name, IHeader value)
    {
      if (name == null)
        throw new ArgumentNullException("value");
      if (value == null || value.Name == null)
        throw new ArgumentNullException("value");

      _headers[name] = value;
    }

    #region IHeaderCollection Members

    /// <summary>
    /// Gets a header
    /// </summary>
    /// <param name="name">header name.</param>
    /// <returns>header if found; otherwise <c>null</c>.</returns>
    public IHeader this[string name]
    {
      get
      {
        IHeader header;
        return _headers.TryGetValue(name, out header) ? header : null;
      }
      set
      {
        if (value == null)
          _headers.Remove(name);
        else
          _headers[name] = value;
      }
    }

    /// <summary>
    /// Get a header 
    /// </summary>
    /// <typeparam name="T">Type that it should be cast to</typeparam>
    /// <param name="headerName">Name of header</param>
    /// <returns>Header if found and casted properly; otherwise <c>null</c>.</returns>
    public T Get<T>(string headerName) where T : class, IHeader
    {
      IHeader header;
      if (_headers.TryGetValue(headerName, out header))
        return header as T;
      return null;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public IEnumerator<IHeader> GetEnumerator()
    {
      return _headers.Values.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
  }
}