using System;

namespace Gimela.Media.Video
{
  /// <summary>
  /// 视频源异常
  /// </summary>
  /// <remarks><para>The exception is thrown in the case of some video related issues, like
  /// failure of initializing codec, compression, etc.</para></remarks>
  public class VideoSourceException : Exception
  {
    /// <summary>
    /// 视频源异常
    /// </summary>
    /// <param name="message">异常信息</param>
    public VideoSourceException(string message) :
      base(message) { }
  }
}
