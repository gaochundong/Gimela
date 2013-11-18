using System;
using System.Drawing;
using System.Net;
using System.Threading;

namespace Gimela.Media.Video
{
  /// <summary>
  /// 视频源
  /// </summary>
  public abstract class VideoSource : IVideoSource
  {
    #region Fields
    
    /// <summary>
    /// received frames count
    /// </summary>
    protected int framesReceived;
    /// <summary>
    /// recieved byte count
    /// </summary>
    protected int bytesReceived;

    /// <summary>
    /// buffer size used to download JPEG image
    /// </summary>
    protected const int BUFFER_SIZE = 512 * 1024;
    /// <summary>
    /// size of portion to read at once
    /// </summary>
    protected const int READ_SIZE = 1024;

    /// <summary>
    /// worker thread
    /// </summary>
    protected Thread thread = null;
    /// <summary>
    /// worker thread stop event
    /// </summary>
    protected ManualResetEvent stopEvent = null;

    #endregion

    #region Ctors

    /// <summary>
    /// 初始化JPEG视频源的实例
    /// </summary>
    /// <param name="source">JPEG视频源URL</param>
    public VideoSource(string source)
    {
      this.Source = source;

      PreventCaching = true;
      RequestTimeout = 10000;
      HttpUserAgent = "Mozilla/5.0";
    }

    #endregion

    #region IVideoSource Members

    /// <summary>
    /// 该事件在当新的视频帧产生时发生
    /// </summary>
    public event NewFrameEventHandler NewFrame;

    /// <summary>
    /// 该事件在当视频源发生异常时发生
    /// </summary>
    public event VideoSourceExceptionEventHandler VideoSourceException;

    /// <summary>
    /// 该事件在当视频源视频结束时发生
    /// </summary>
    public event VideoSourceFinishedEventHandler VideoSourceFinished;

    /// <summary>
    /// 视频源，该属性依赖于特定的视频源，其可能是文件名、URL或其他描述视频源的字符串。
    /// </summary>
    public virtual string Source { get; private set; }

    /// <summary>
    /// 接收到帧的数量
    /// </summary>
    public int FramesReceived
    {
      get
      {
        int frames = framesReceived;
        framesReceived = 0;
        return frames;
      }
    }

    /// <summary>
    /// 接收到Byte的数量
    /// </summary>
    public int BytesReceived
    {
      get
      {
        int bytes = bytesReceived;
        bytesReceived = 0;
        return bytes;
      }
    }

    /// <summary>
    /// 视频源是否在运行
    /// </summary>
    public bool IsRunning
    {
      get
      {
        if (thread != null)
        {
          // check thread status
          if (thread.Join(0) == false)
            return true;

          // the thread is not running, free resources
          Free();
        }
        return false;
      }
    }

    /// <summary>
    /// 启动视频源
    /// </summary>
    public void Start()
    {
      if (!IsRunning)
      {
        // check source
        if (string.IsNullOrEmpty(Source))
          throw new VideoSourceException("Video source is not specified.");

        framesReceived = 0;
        bytesReceived = 0;

        // create events
        stopEvent = new ManualResetEvent(false);

        // create and start new thread
        thread = new Thread(new ThreadStart(WorkerThread));
        thread.Name = Source; // mainly for debugging
        thread.Start();
      }
    }

    /// <summary>
    /// 通知视频源停止工作，停止后台线程，停止发送新帧，释放资源
    /// </summary>
    public void SignalToStop()
    {
      // stop thread
      if (thread != null)
      {
        // signal to stop
        stopEvent.Set();
      }
    }

    /// <summary>
    /// 等待视频源的停止
    /// </summary>
    public void WaitForStop()
    {
      if (thread != null)
      {
        // wait for thread stop
        thread.Join();

        Free();
      }
    }

    /// <summary>
    /// 停止视频源，中止线程
    /// </summary>
    public void Stop()
    {
      if (this.IsRunning)
      {
        stopEvent.Set();
        thread.Abort();
        WaitForStop();
      }
    }

    #endregion

    #region Properties
    
    /// <summary>
    /// 在独立的连接组中进行Web请求
    /// </summary>
    /// <remarks>The property indicates to open web request in separate connection group.</remarks>
    public bool UseSeparateConnectionGroup { get; set; }

    /// <summary>
    /// 是否阻止缓存
    /// </summary>
    /// <remarks>If the property is set to <b>true</b>, then a fake random parameter will be added
    /// to URL to prevent caching. It's required for clients, who are behind proxy server.</remarks>
    public bool PreventCaching { get; set; }

    /// <summary>
    /// 登录用户名
    /// </summary>
    /// <remarks>Login required to access video source.</remarks>
    public string Login { get; set; }

    /// <summary>
    /// 登录密码
    /// </summary>
    /// <remarks>Password required to access video source.</remarks>
    public string Password { get; set; }

    /// <summary>
    /// Web请求代理
    /// </summary>
    /// <remarks><para>The local computer or application config file may specify that a default
    /// proxy to be used. If the Proxy property is specified, then the proxy settings from the Proxy
    /// property overridea the local computer or application config file and the instance will use
    /// the proxy settings specified. If no proxy is specified in a config file
    /// and the Proxy property is unspecified, the request uses the proxy settings
    /// inherited from Internet Explorer on the local computer. If there are no proxy settings
    /// in Internet Explorer, the request is sent directly to the server.
    /// </para></remarks>
    public IWebProxy Proxy { get; set; }

    /// <summary>
    /// User agent to specify in HTTP request header.
    /// </summary>
    /// <remarks><para>Some IP cameras check what is the requesting user agent and depending
    /// on it they provide video in different formats or do not provide it at all. The property
    /// sets the value of user agent string, which is sent to camera in request header.
    /// </para>
    /// <para>Default value is set to "Mozilla/5.0". If the value is set to <see langword="null"/>,
    /// the user agent string is not sent in request header.</para>
    /// </remarks>
    public string HttpUserAgent { get; set; }

    /// <summary>
    /// 请求超时时长
    /// </summary>
    /// <remarks><para>The property sets timeout value in milliseconds for web requests.</para>
    /// <para>Default value is set <b>10000</b> milliseconds.</para></remarks>
    public int RequestTimeout { get; set; }

    #endregion

    #region Protected Methods

    /// <summary>
    /// 工作线程
    /// </summary>
    protected abstract void WorkerThread();

    #endregion

    #region Raise Events

    /// <summary>
    /// 是否能够触发新帧事件通知
    /// </summary>
    /// <returns>是否能够触发新帧事件通知</returns>
    protected bool CanRaiseNewFrameEvent()
    {
      bool canRaise = false;

      if (NewFrame != null)
      {
        canRaise = true;
      }

      return canRaise;
    }

    /// <summary>
    /// 触发新帧事件通知
    /// </summary>
    /// <param name="frame">新帧</param>
    /// <param name="timestamp">时间戳</param>
    protected void RaiseNewFrameEvent(Bitmap frame, DateTime timestamp)
    {
      if (NewFrame != null)
      {
        NewFrame(this, new NewFrameEventArgs(frame, timestamp));
      }
    }

    /// <summary>
    /// 触发视频源异常事件
    /// </summary>
    /// <param name="description">异常描述</param>
    protected void RaiseVideoSourceExceptionEvent(string description)
    {
      if (VideoSourceException != null)
      {
        VideoSourceException(this, new VideoSourceExceptionEventArgs(description));
      }
    }

    /// <summary>
    /// 触发视频源结束事件
    /// </summary>
    /// <param name="reason">视频源结束原因</param>
    protected void RaiseVideoSourceFinishedEvent(VideoSourceFinishedReasonType reason)
    {
      if (VideoSourceFinished != null)
      {
        VideoSourceFinished(this, new VideoSourceFinishedEventArgs(reason));
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 释放资源
    /// </summary>
    private void Free()
    {
      thread = null;

      // release events
      stopEvent.Close();
      stopEvent = null;
    }

    #endregion
  }
}
