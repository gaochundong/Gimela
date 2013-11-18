using System;

namespace Gimela.Net.Http.Messages.Parsers
{
  /// <summary>
  /// Event arguments used when a new header have been parsed.
  /// </summary>
  public class HeaderEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="HeaderEventArgs"/> class.
    /// </summary>
    /// <param name="name">Name of header.</param>
    /// <param name="value">Header value.</param>
    /// <exception cref="ArgumentException">Name cannot be empty</exception>
    /// <exception cref="ArgumentNullException"><c>value</c> is <c>null</c>.</exception>
    public HeaderEventArgs(string name, string value)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException("Name cannot be empty", "name");
      if (value == null)
        throw new ArgumentNullException("value");

      Name = name;
      Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeaderEventArgs"/> class.
    /// </summary>
    public HeaderEventArgs()
    {
    }

    /// <summary>
    /// Gets or sets header name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets header value.
    /// </summary>
    public string Value { get; set; }
  }
}