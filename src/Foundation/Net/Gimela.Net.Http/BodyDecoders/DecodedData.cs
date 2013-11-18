namespace Gimela.Net.Http.BodyDecoders
{
  /// <summary>
  /// Data decoded from a POST body.
  /// </summary>
  public class DecodedData
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DecodedData"/> class.
    /// </summary>
    public DecodedData()
    {
      Parameters = new ParameterCollection();
      Files = new HttpFileCollection();
    }

    /// <summary>
    /// Gets or sets decoded files.
    /// </summary>
    public HttpFileCollection Files { get; set; }

    /// <summary>
    /// Gets or sets decoded parameters.
    /// </summary>
    public ParameterCollection Parameters { get; set; }
  }
}