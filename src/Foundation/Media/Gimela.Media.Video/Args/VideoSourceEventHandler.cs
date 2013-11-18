
namespace Gimela.Media.Video
{
  /// <summary>
  /// 当新的视频帧产生时发生的事件代理
  /// </summary>
  /// <param name="sender">发送者</param>
  /// <param name="e">新帧事件参数</param>
  public delegate void NewFrameEventHandler(object sender, NewFrameEventArgs e);

  /// <summary>
  /// 视频源发生异常时发生的事件代理
  /// </summary>
  /// <param name="sender">发送者</param>
  /// <param name="e">视频源异常事件参数</param>
  public delegate void VideoSourceExceptionEventHandler(object sender, VideoSourceExceptionEventArgs e);

  /// <summary>
  /// 视频源视频结束时发生的事件代理
  /// </summary>
  /// <param name="sender">发送者</param>
  /// <param name="e">视频源视频结束事件参数</param>
  public delegate void VideoSourceFinishedEventHandler(object sender, VideoSourceFinishedEventArgs e);
}
