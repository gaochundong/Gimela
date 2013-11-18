
namespace Gimela.Media.Video
{
  /// <summary>
  /// 视频源视频结束原因
  /// </summary>
  public enum VideoSourceFinishedReasonType
  {
    /// <summary>
    /// 已播放至终点
    /// </summary>
    EndOfStreamReached,
    /// <summary>
    /// 被用户停止
    /// </summary>
    StoppedByUser,
    /// <summary>
    /// 设备流失
    /// </summary>
    DeviceLost,
    /// <summary>
    /// 视频源结束错误
    /// </summary>
    VideoSourceException
  }
}
