
namespace Gimela.Net.Http.Headers
{
  /// <summary>
  /// Used to store all headers that that aren't recognized.
  /// </summary>
  public class StringHeader : IHeader
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="StringHeader"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public StringHeader(string name, string value)
    {
      Name = name;
      Value = value;
    }

    /// <summary>
    /// Gets or sets value
    /// </summary>
    public string Value { get; set; }

    #region IHeader Members

    /// <summary>
    /// Gets header name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets value as it would be sent back to client.
    /// </summary>
    public string HeaderValue
    {
      get { return Value; }
    }

    #endregion

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return Value;
    }
  }
}