namespace Gimela.Net.Http
{
  /// <summary>
  /// File sent from remote end.
  /// </summary>
  public class HttpFile
  {
    /// <summary>
    /// Gets or sets content type.
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets or sets name in form.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets name original file name
    /// </summary>
    public string OriginalFileName { get; set; }

    /// <summary>
    /// Gets or sets filename for locally stored file.
    /// </summary>
    public string TempFileName { get; set; }
  }
}