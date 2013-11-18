
namespace Gimela.Media.Video
{
  /// <summary>
  /// 视频源接口，该接口描述不同视频源类型的通用接口。
  /// </summary>
  public interface IVideoSource
  {
    /// <summary>
    /// 该事件在当新的视频帧产生时发生
    /// </summary>
    event NewFrameEventHandler NewFrame;

    /// <summary>
    /// 该事件在当视频源发生异常时发生
    /// </summary>
    event VideoSourceExceptionEventHandler VideoSourceException;

    /// <summary>
    /// 该事件在当视频源视频结束时发生
    /// </summary>
    event VideoSourceFinishedEventHandler VideoSourceFinished;

    /// <summary>
    /// 视频源，该属性依赖于特定的视频源，其可能是文件名、URL或其他描述视频源的字符串。
    /// </summary>
    string Source { get; }

    /// <summary>
    /// 接收到帧的数量
    /// </summary>
    int FramesReceived { get; }

    /// <summary>
    /// 接收到Byte的数量
    /// </summary>
    int BytesReceived { get; }

    /// <summary>
    /// 视频源是否在运行
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// 启动视频源
    /// </summary>
    void Start();

    /// <summary>
    /// 通知视频源停止工作，停止后台线程，停止发送新帧，释放资源
    /// </summary>
    void SignalToStop();

    /// <summary>
    /// 等待视频源的停止
    /// </summary>
    void WaitForStop();

    /// <summary>
    /// 停止视频源，中止线程
    /// </summary>
    void Stop();
  }
}
