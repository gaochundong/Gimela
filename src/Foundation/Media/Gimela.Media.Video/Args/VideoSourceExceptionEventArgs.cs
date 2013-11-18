using System;
using System.Globalization;

namespace Gimela.Media.Video
{
  /// <summary>
  /// 视频源发生异常事件参数
  /// </summary>
  public class VideoSourceExceptionEventArgs : EventArgs
  {
    private string description;

    /// <summary>
    /// 视频源发生异常事件参数
    /// </summary>
    /// <param name="description">视频源异常描述</param>
    public VideoSourceExceptionEventArgs(string description)
    {
      this.description = description;
    }

    /// <summary>
    /// 视频源发生异常事件参数
    /// </summary>
    public string Description
    {
      get { return description; }
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, "{0}", Description);
    }
  }
}
