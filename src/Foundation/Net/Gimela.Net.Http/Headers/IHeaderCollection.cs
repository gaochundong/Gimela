using System.Collections.Generic;

namespace Gimela.Net.Http.Headers
{
  /// <summary>
  /// Collection of headers.
  /// </summary>
  public interface IHeaderCollection : IEnumerable<IHeader>
  {
    /// <summary>
    /// Gets a header
    /// </summary>
    /// <param name="name">header name.</param>
    /// <returns>header if found; otherwise <c>null</c>.</returns>
    IHeader this[string name] { get; }

  }
}