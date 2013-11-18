
namespace Gimela.Net.Rtp
{
  /// <summary>
  /// RTP消息中携带的Payload类型(RFC3551)
  /// </summary>
  public enum RtpPayloadType
  {
    /// <summary>
    /// G721
    /// </summary>
    G721 = 2,
    /// <summary>
    /// G722
    /// </summary>
    G722 = 9,
    /// <summary>
    /// G728
    /// </summary>
    G728 = 15,
    /// <summary>
    /// G729
    /// </summary>
    G729 = 18,
    /// <summary>
    /// JPEG
    /// </summary>
    JPEG = 26,
    /// <summary>
    /// H261
    /// </summary>
    H261 = 31,
    /// <summary>
    /// H263
    /// </summary>
    H263 = 34,
  }
}
