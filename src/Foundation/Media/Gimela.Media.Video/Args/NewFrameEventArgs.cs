using System;
using System.Drawing;

namespace Gimela.Media.Video
{
  /// <summary>
  /// 视频源新帧事件参数
  /// </summary>
  public class NewFrameEventArgs : EventArgs
  {
    /// <summary>
    /// 视频源新帧事件参数
    /// </summary>
    /// <param name="frame">新帧</param>
    /// <param name="timestamp">时间戳</param>
    public NewFrameEventArgs(Bitmap frame, DateTime timestamp)
    {
      Frame = frame;
      Timestamp = timestamp;
    }

    /// <summary>
    /// 新帧
    /// </summary>
    public Bitmap Frame { get; private set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; private set; }
  }
}
