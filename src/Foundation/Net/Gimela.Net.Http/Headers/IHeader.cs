namespace Gimela.Net.Http.Headers
{
  /// <summary>
  /// Header in a message
  /// </summary>
  /// <remarks>
  /// Important! Each header should override ToString() 
  /// and return it's data correctly formatted as a HTTP header value.
  /// </remarks>
  public interface IHeader
  {
    /// <summary>
    /// Gets header name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets value as it would be sent back to client.
    /// </summary>
    string HeaderValue { get; }
  }
}