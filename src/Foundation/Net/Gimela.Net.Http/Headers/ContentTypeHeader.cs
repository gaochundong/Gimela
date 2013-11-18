
namespace Gimela.Net.Http.Headers
{
  /// <summary>
  /// Content-type
  /// </summary>
  public class ContentTypeHeader : IHeader
  {
    /// <summary>
    /// Header name.
    /// </summary>
    public const string ContentTypeName = "Content-Type";
    private readonly HeaderParameterCollection _parameters;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentTypeHeader"/> class.
    /// </summary>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="parameterCollection">Value parameters.</param>
    public ContentTypeHeader(string contentType, HeaderParameterCollection parameterCollection)
    {
      Value = contentType;
      _parameters = parameterCollection;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentTypeHeader"/> class.
    /// </summary>
    /// <param name="contentType">Type of the content.</param>
    public ContentTypeHeader(string contentType)
    {
      Value = contentType;
      _parameters = new HeaderParameterCollection();
    }

    /// <summary>
    /// Gets all parameters.
    /// </summary>
    public HeaderParameterCollection Parameters
    {
      get { return _parameters; }
    }

    /// <summary>
    /// Gets content type.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Returns data formatted as a HTTP header value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      return HeaderValue;
    }

    #region IHeader Members

    /// <summary>
    /// Gets header name
    /// </summary>
    public string Name
    {
      get { return ContentTypeName; }
    }

    /// <summary>
    /// Gets value as it would be sent back to client.
    /// </summary>
    public string HeaderValue
    {
      get
      {
        string value = Value;
        string parameters = Parameters.ToString();
        if (!string.IsNullOrEmpty(parameters))
          return value + ";" + parameters;
        return value;
      }
    }

    #endregion
  }
}