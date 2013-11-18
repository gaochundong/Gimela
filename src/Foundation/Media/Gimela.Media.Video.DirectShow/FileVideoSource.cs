using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using Gimela.Media.Video.DirectShow.Internals;

namespace Gimela.Media.Video.DirectShow
{
  /// <summary>
  /// 文件视频源
  /// </summary>
  public class FileVideoSource : IVideoSource
  {
    #region Fields

    /// <summary>
    /// received frames count
    /// </summary>
    private int framesReceived;
    /// <summary>
    /// recieved byte count
    /// </summary>
    private int bytesReceived;

    /// <summary>
    /// worker thread
    /// </summary>
    private Thread thread = null;
    /// <summary>
    /// worker thread stop event
    /// </summary>
    private ManualResetEvent stopEvent = null;

    #endregion

    #region Ctors
    
    /// <summary>
    /// 初始化文件视频源新的实例
    /// </summary>
    /// <param name="fileName">视频文件名</param>
    public FileVideoSource(string fileName)
    {
      this.Source = fileName;
      ReferenceClockEnabled = true;
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
          throw new VideoSourceException("Video source is not specified");

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
        thread.Abort();
        WaitForStop();
      }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Prevent video freezing after screen saver and workstation lock or not.
    /// </summary>
    /// <remarks>
    /// <para>The value specifies if the class should prevent video freezing during and
    /// after screen saver or workstation lock. To prevent freezing the <i>DirectShow</i> graph
    /// should not contain <i>Renderer</i> filter, which is added by <i>Render()</i> method
    /// of graph. However, in some cases it may be required to call <i>Render()</i> method of graph, since
    /// it may add some more filters, which may be required for playing video. So, the property is
    /// a trade off - it is possible to prevent video freezing skipping adding renderer filter or
    /// it is possible to keep renderer filter, but video may freeze during screen saver.</para>
    /// <para><note>The property may become obsolete in the future when approach to disable freezing
    /// and adding all required filters is found.</note></para>
    /// <para><note>The property should be set before calling <see cref="Start"/> method
    /// of the class to have effect.</note></para>
    /// <para>Default value of this property is set to <b>false</b>.</para>
    /// </remarks>
    public bool PreventFreezing { get; set; }

    /// <summary>
    /// Enables/disables reference clock on the graph.
    /// </summary>
    /// <remarks><para>Disabling reference clocks causes DirectShow graph to run as fast as
    /// it can process data. When enabled, it will process frames according to presentation
    /// time of a video file.</para>
    /// <para><note>The property should be set before calling <see cref="Start"/> method
    /// of the class to have effect.</note></para>
    /// <para>Default value of this property is set to <b>true</b>.</para>
    /// </remarks>
    public bool ReferenceClockEnabled { get; set; }

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

    /// <summary>
    /// 工作线程
    /// </summary>
    private void WorkerThread()
    {
      VideoSourceFinishedReasonType reasonToStop = VideoSourceFinishedReasonType.StoppedByUser;

      // grabber
      Grabber grabber = new Grabber(this);

      // objects
      object graphObject = null;
      object grabberObject = null;

      // interfaces
      IGraphBuilder graph = null;
      IBaseFilter sourceBase = null;
      IBaseFilter grabberBase = null;
      ISampleGrabber sampleGrabber = null;
      IMediaControl mediaControl = null;
      IMediaEventEx mediaEvent = null;

      try
      {
        // get type for filter graph
        Type type = Type.GetTypeFromCLSID(Clsid.FilterGraph);
        if (type == null)
          throw new VideoSourceException("Failed creating filter graph");

        // create filter graph
        graphObject = Activator.CreateInstance(type);
        graph = (IGraphBuilder)graphObject;

        // create source device's object
        graph.AddSourceFilter(Source, "source", out sourceBase);
        if (sourceBase == null)
          throw new VideoSourceException("Failed creating source filter");

        // get type for sample grabber
        type = Type.GetTypeFromCLSID(Clsid.SampleGrabber);
        if (type == null)
          throw new VideoSourceException("Failed creating sample grabber");

        // create sample grabber
        grabberObject = Activator.CreateInstance(type);
        sampleGrabber = (ISampleGrabber)grabberObject;
        grabberBase = (IBaseFilter)grabberObject;

        // add grabber filters to graph
        graph.AddFilter(grabberBase, "grabber");

        // set media type
        AMMediaType mediaType = new AMMediaType();
        mediaType.MajorType = MediaType.Video;
        mediaType.SubType = MediaSubType.RGB24;
        sampleGrabber.SetMediaType(mediaType);

        // connect pins
        int pinToTry = 0;

        IPin inPin = PinHelper.GetInPin(grabberBase, 0);
        IPin outPin = null;

        // find output pin acceptable by sample grabber
        while (true)
        {
          outPin = PinHelper.GetOutPin(sourceBase, pinToTry);

          if (outPin == null)
          {
            Marshal.ReleaseComObject(inPin);
            throw new VideoSourceException("Cannot find acceptable output video pin in the given source");
          }

          if (graph.Connect(outPin, inPin) < 0)
          {
            Marshal.ReleaseComObject(outPin);
            outPin = null;
            pinToTry++;
          }
          else
          {
            break;
          }
        }

        Marshal.ReleaseComObject(outPin);
        Marshal.ReleaseComObject(inPin);

        // get media type
        if (sampleGrabber.GetConnectedMediaType(mediaType) == 0)
        {
          VideoInfoHeader vih = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader));

          grabber.Width = vih.BmiHeader.Width;
          grabber.Height = vih.BmiHeader.Height;
          mediaType.Dispose();
        }

        // let's do rendering, if we don't need to prevent freezing
        if (!PreventFreezing)
        {
          // render pin
          graph.Render(PinHelper.GetOutPin(grabberBase, 0));

          // configure video window
          IVideoWindow window = (IVideoWindow)graphObject;
          window.put_AutoShow(false);
          window = null;
        }

        // configure sample grabber
        sampleGrabber.SetBufferSamples(false);
        sampleGrabber.SetOneShot(false);
        sampleGrabber.SetCallback(grabber, 1);

        // disable clock, if someone requested it
        if (!ReferenceClockEnabled)
        {
          IMediaFilter mediaFilter = (IMediaFilter)graphObject;
          mediaFilter.SetSyncSource(null);
        }

        // get media control
        mediaControl = (IMediaControl)graphObject;

        // get media events' interface
        mediaEvent = (IMediaEventEx)graphObject;
        IntPtr p1, p2;
        DsEvCode code;

        // run
        mediaControl.Run();

        while (!stopEvent.WaitOne(0, false))
        {
          Thread.Sleep(100);

          if (mediaEvent != null)
          {
            if (mediaEvent.GetEvent(out code, out p1, out p2, 0) >= 0)
            {
              mediaEvent.FreeEventParams(code, p1, p2);

              if (code == DsEvCode.Complete)
              {
                reasonToStop = VideoSourceFinishedReasonType.EndOfStreamReached;
                break;
              }
            }
          }
        }

        // stop
        mediaControl.Stop();
      }
      catch (Exception ex)
      {
        // provide information to clients
        if (VideoSourceException != null)
        {
          VideoSourceException(this, new VideoSourceExceptionEventArgs(ex.Message));
        }
      }
      finally
      {
        // release all objects
        graph = null;
        grabberBase = null;
        sampleGrabber = null;
        mediaControl = null;
        mediaEvent = null;

        if (graphObject != null)
        {
          Marshal.ReleaseComObject(graphObject);
          graphObject = null;
        }
        if (sourceBase != null)
        {
          Marshal.ReleaseComObject(sourceBase);
          sourceBase = null;
        }
        if (grabberObject != null)
        {
          Marshal.ReleaseComObject(grabberObject);
          grabberObject = null;
        }
      }

      if (VideoSourceFinished != null)
      {
        VideoSourceFinished(this, new VideoSourceFinishedEventArgs(reasonToStop));
      }
    }

    /// <summary>
    /// 发送新帧事件通知
    /// </summary>
    /// <param name="frame">新帧</param>
    /// <param name="timestamp">时间戳</param>
    protected void RaiseNewFrameEvent(Bitmap frame, DateTime timestamp)
    {
      framesReceived++;
      if ((!stopEvent.WaitOne(0, false)) && (NewFrame != null))
        NewFrame(this, new NewFrameEventArgs(frame, timestamp));
    }

    /// <summary>
    /// Video grabber
    /// </summary>
    private class Grabber : ISampleGrabberCB
    {
      private FileVideoSource parent;

      // Width property
      public int Width { get; set; }

      // Height property
      public int Height { get; set; }

      // Constructor
      public Grabber(FileVideoSource parent)
      {
        this.parent = parent;
      }

      // Callback to receive samples
      public int SampleCB(double sampleTime, IntPtr sample)
      {
        return 0;
      }

      // Callback method that receives a pointer to the sample buffer
      public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
      {
        if (parent.NewFrame != null)
        {
          // create new image
          System.Drawing.Bitmap image = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

          // lock bitmap data
          BitmapData imageData = image.LockBits(
              new Rectangle(0, 0, Width, Height),
              ImageLockMode.ReadWrite,
              PixelFormat.Format24bppRgb);

          // copy image data
          int srcStride = imageData.Stride;
          int dstStride = imageData.Stride;

          unsafe
          {
            byte* dst = (byte*)imageData.Scan0.ToPointer() + dstStride * (Height - 1);
            byte* src = (byte*)buffer.ToPointer();

            for (int y = 0; y < Height; y++)
            {
              Win32.memcpy(dst, src, srcStride);
              dst -= dstStride;
              src += srcStride;
            }
          }

          // unlock bitmap data
          image.UnlockBits(imageData);

          // notify parent
          parent.RaiseNewFrameEvent(image, DateTime.Now);

          // release the image
          image.Dispose();
        }

        return 0;
      }
    }

    #endregion
  }
}
