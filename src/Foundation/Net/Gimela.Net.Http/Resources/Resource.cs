using System;
using System.IO;

namespace Gimela.Net.Http.Resources
{
  /// <summary>
  /// Resource information.
  /// </summary>
  /// <remarks>
  /// Used by content providers to be able to get information
  /// on resources (views, files etc).
  /// </remarks>
  public class Resource
  {
    /// <summary>
    /// Gets or sets date when resource was modified.
    /// </summary>
    /// <value>
    /// <see cref="DateTime.MinValue"/> if not used.
    /// </value>
    /// <remarks>
    /// Should always be universal time.
    /// </remarks>
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets resource stream.
    /// </summary>
    public Stream Stream { get; set; }
  }
}