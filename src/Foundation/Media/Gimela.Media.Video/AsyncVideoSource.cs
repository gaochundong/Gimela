using System;
using System.Drawing;
using System.Threading;

namespace Gimela.Media.Video
{
  /// <summary>
  /// 视频源的代理，封装嵌套的视频源为异步处理模型
  /// </summary>
  /// 
  /// <remarks><para>The class represents a simple proxy, which wraps the specified <see cref="NestedVideoSource"/>
  /// with the aim of asynchronous processing of received video frames. The class intercepts <see cref="NewFrame"/>
  /// event from the nested video source and fires it to clients from its own thread, which is different from the thread
  /// used by nested video source for video acquisition. This allows clients to perform processing of video frames
  /// without blocking video acquisition thread, which continue to run and acquire next video frame while current is still
  /// processed.</para>
  /// 
  /// <para>For example, let’s suppose that it takes 100 ms for the nested video source to acquire single frame, so the original
  /// frame rate is 10 frames per second. Also let’s assume that we have an image processing routine, which also takes
  /// 100 ms to process a single frame. If the acquisition and processing are done sequentially, then resulting
  /// frame rate will drop to 5 frames per second. However, if doing both in parallel, then there is a good chance to
  /// keep resulting frame rate equal (or close) to the original frame rate.</para>
  /// 
  /// <para>The class provides a bonus side effect - easer debugging of image processing routines, which are put into
  /// <see cref="NewFrame"/> event handler. In many cases video source classes fire their <see cref="IVideoSource.NewFrame"/>
  /// event from a try/catch block, which makes it very hard to spot error made in user's code - the catch block simply
  /// hides exception raised in user’s code. The <see cref="AsyncVideoSource"/> does not have any try/catch blocks around
  /// firing of <see cref="NewFrame"/> event, so always user gets exception in the case it comes from his code. At the same time
  /// nested video source is not affected by the user's exception, since it runs in different thread.</para>
  /// 
  /// <para>Sample usage:</para>
  /// <code>
  /// // usage of AsyncVideoSource is the same as usage of any
  /// // other video source class, so code change is very little
  /// 
  /// // create nested video source, for example JPEGStream
  /// JPEGStream stream = new JPEGStream( "some url" );
  /// // create async video source
  /// AsyncVideoSource asyncSource = new AsyncVideoSource( stream );
  /// // set NewFrame event handler
  /// asyncSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
  /// // start the video source
  /// asyncSource.Start( );
  /// // ...
  /// 
  /// private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
  /// {
  ///     // get new frame
  ///     Bitmap bitmap = eventArgs.Frame;
  ///     // process the frame
  /// }
  /// </code>
  /// </remarks>
  public class AsyncVideoSource : IVideoSource
  {
    #region Fields

    private readonly IVideoSource nestedVideoSource = null;
    private Bitmap lastVideoFrame = null;
    private int framesProcessed;
    private Thread imageProcessingThread = null;
    private AutoResetEvent isNewFrameAvailable = null;
    private AutoResetEvent isProcessingThreadAvailable = null;

    #endregion

    #region Ctors

    /// <summary>
    /// 视频源的代理，封装嵌套的视频源为异步处理模型
    /// </summary>
    /// <param name="nestedVideoSource">嵌套的视频源</param>
    public AsyncVideoSource(IVideoSource nestedVideoSource)
      : this(nestedVideoSource, true)
    {
    }

    /// <summary>
    /// 视频源的代理，封装嵌套的视频源为异步处理模型
    /// </summary>
    /// <param name="nestedVideoSource">嵌套的视频源</param>
    /// <param name="skipFramesIfBusy">当上一帧视频未被处理完毕时，新的帧已到达，是否跳过新的帧的处理</param>
    public AsyncVideoSource(IVideoSource nestedVideoSource, bool skipFramesIfBusy)
    {
      this.nestedVideoSource = nestedVideoSource;
      this.SkipFramesIfBusy = skipFramesIfBusy;
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
    public event VideoSourceExceptionEventHandler VideoSourceException
    {
      add { nestedVideoSource.VideoSourceException += value; }
      remove { nestedVideoSource.VideoSourceException -= value; }
    }

    /// <summary>
    /// 该事件在当视频源视频结束时发生
    /// </summary>
    public event VideoSourceFinishedEventHandler VideoSourceFinished
    {
      add { nestedVideoSource.VideoSourceFinished += value; }
      remove { nestedVideoSource.VideoSourceFinished -= value; }
    }

    /// <summary>
    /// 视频源，该属性依赖于特定的视频源，其可能是文件名、URL或其他描述视频源的字符串。
    /// </summary>
    public string Source
    {
      get { return nestedVideoSource.Source; }
    }

    /// <summary>
    /// 接收到帧的数量
    /// </summary>
    public int FramesReceived
    {
      get { return nestedVideoSource.FramesReceived; }
    }

    /// <summary>
    /// 接收到Byte的数量
    /// </summary>
    public int BytesReceived
    {
      get { return nestedVideoSource.BytesReceived; }
    }

    /// <summary>
    /// 视频源是否在运行
    /// </summary>
    public bool IsRunning
    {
      get
      {
        bool isRunning = nestedVideoSource.IsRunning;

        if (!isRunning)
        {
          Free();
        }

        return isRunning;
      }
    }

    /// <summary>
    /// 启动视频源
    /// </summary>
    public void Start()
    {
      if (!IsRunning)
      {
        framesProcessed = 0;

        // create all synchronization events
        isNewFrameAvailable = new AutoResetEvent(false);
        isProcessingThreadAvailable = new AutoResetEvent(true);

        // create image processing thread
        imageProcessingThread = new Thread(new ThreadStart(ImageProcessingWorkerThread));
        imageProcessingThread.Start();

        // start the nested video source
        nestedVideoSource.NewFrame += new NewFrameEventHandler(OnNestedVideoSourceNewFrame);
        nestedVideoSource.Start();
      }
    }

    /// <summary>
    /// 通知视频源停止工作，停止后台线程，停止发送新帧，释放资源
    /// </summary>
    public void SignalToStop()
    {
      nestedVideoSource.SignalToStop();
    }

    /// <summary>
    /// 等待视频源的停止
    /// </summary>
    public void WaitForStop()
    {
      nestedVideoSource.WaitForStop();
      Free();
    }

    /// <summary>
    /// 停止视频源，中止线程
    /// </summary>
    public void Stop()
    {
      nestedVideoSource.Stop();
      Free();
    }

    #endregion

    #region Properties

    /// <summary>
    /// 嵌套的视频源
    /// </summary>
    /// <remarks>
    /// <para>All calls to this object are actually redirected to the nested video source. The only
    /// exception is the <see cref="NewFrame"/> event, which is handled differently. This object gets
    /// <see cref="IVideoSource.NewFrame"/> event from the nested class and then fires another
    /// <see cref="NewFrame"/> event, but from a different thread.</para>
    /// </remarks>
    public IVideoSource NestedVideoSource
    {
      get { return nestedVideoSource; }
    }

    /// <summary>
    /// 处理帧的数量，每次获取时清零，记录自上一次获取起至今的处理帧的数量
    /// </summary>
    public int FramesProcessed
    {
      get
      {
        int frames = framesProcessed;
        framesProcessed = 0;
        return frames;
      }
    }

    /// <summary>
    /// 当上一帧视频未被处理完毕时，新的帧已到达，是否跳过新的帧的处理
    /// </summary>
    public bool SkipFramesIfBusy { get; set; }

    #endregion

    #region Private Methods

    /// <summary>
    /// 释放资源
    /// </summary>
    private void Free()
    {
      if (imageProcessingThread != null)
      {
        nestedVideoSource.NewFrame -= new NewFrameEventHandler(OnNestedVideoSourceNewFrame);

        // make sure processing thread does nothing
        isProcessingThreadAvailable.WaitOne();
        // signal worker thread to stop and wait for it
        lastVideoFrame = null;
        isNewFrameAvailable.Set();
        imageProcessingThread.Join();
        imageProcessingThread = null;

        // release events
        isNewFrameAvailable.Close();
        isNewFrameAvailable = null;

        isProcessingThreadAvailable.Close();
        isProcessingThreadAvailable = null;
      }
    }

    /// <summary>
    /// 从嵌套的视频源处接收到新的帧
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="e">新的帧</param>
    private void OnNestedVideoSourceNewFrame(object sender, NewFrameEventArgs e)
    {
      // don't even try doing something if there are no clients
      if (NewFrame == null)
        return;

      // 是否判断跳帧
      if (SkipFramesIfBusy)
      {
        // 如果还在处理上一帧，则跳帧
        if (!isProcessingThreadAvailable.WaitOne(0, false))
        {
          // return in the case if image processing thread is still busy and
          // we are allowed to skip frames
          return;
        }
      }
      else
      {
        // make sure image processing thread is available in the case we cannot skip frames
        isProcessingThreadAvailable.WaitOne();
      }

      // pass the image to processing frame and exit
      lastVideoFrame = (Bitmap)e.Frame.Clone();
      isNewFrameAvailable.Set();
    }

    /// <summary>
    /// 帧处理工作线程
    /// </summary>
    private void ImageProcessingWorkerThread()
    {
      while (true)
      {
        // wait for new frame to process
        isNewFrameAvailable.WaitOne();

        // if it is null, then we need to exit
        if (lastVideoFrame == null)
        {
          break;
        }

        if (NewFrame != null)
        {
          NewFrame(this, new NewFrameEventArgs(lastVideoFrame, DateTime.Now));
        }

        lastVideoFrame.Dispose();
        lastVideoFrame = null;
        framesProcessed++;

        // we are free now for new image
        isProcessingThreadAvailable.Set();
      }
    }

    #endregion
  }
}
