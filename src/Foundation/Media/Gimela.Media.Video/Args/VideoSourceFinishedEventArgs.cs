using System;
using System.Globalization;

namespace Gimela.Media.Video
{
  /// <summary>
  /// 视频源视频结束事件参数
  /// </summary>
  public class VideoSourceFinishedEventArgs : EventArgs
  {
    private VideoSourceFinishedReasonType reason;

    /// <summary>
    /// 视频源视频结束事件参数
    /// </summary>
    /// <param name="reason">视频源视频结束原因</param>
    public VideoSourceFinishedEventArgs(VideoSourceFinishedReasonType reason)
    {
      this.reason = reason;
    }

    /// <summary>
    /// 视频源视频结束事件参数
    /// </summary>
    public VideoSourceFinishedReasonType Reason
    {
      get { return reason; }
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, "{0}", Reason);
    }
  }
}
