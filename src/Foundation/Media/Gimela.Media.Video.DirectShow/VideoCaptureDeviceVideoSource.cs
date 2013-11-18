using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using Gimela.Media.Video.DirectShow.Internals;

namespace Gimela.Media.Video.DirectShow
{
  /// <summary>
  /// Video source for local video capture device (for example USB webcam).
  /// </summary>
  /// 
  /// <remarks><para>This video source class captures video data from local video capture device,
  /// like USB web camera (or internal), frame grabber, capture board - anything which
  /// supports <b>DirectShow</b> interface. For devices which has a shutter button or
  /// support external software triggering, the class also allows to do snapshots. Both
  /// video size and snapshot size can be configured.</para>
  /// 
  /// <para>Sample usage:</para>
  /// <code>
  /// // enumerate video devices
  /// videoDevices = new FilterInfoCollection( FilterCategory.VideoInputDevice );
  /// // create video source
  /// VideoCaptureDevice videoSource = new VideoCaptureDevice( videoDevices[0].MonikerString );
  /// // set NewFrame event handler
  /// videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
  /// // start the video source
  /// videoSource.Start( );
  /// // ...
  /// // signal to stop when you no longer need capturing
  /// videoSource.SignalToStop( );
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
  /// 
  public class VideoCaptureDeviceVideoSource : IVideoSource
  {
    // moniker string of video capture device
    private string deviceMoniker;
    // received frames count
    private int framesReceived;
    // recieved byte count
    private int bytesReceived;
    // specifies desired size of captured video frames
    private Size desiredFrameSize = new Size(0, 0);
    // specifies desired video capture frame rate
    private int desiredFrameRate = 0;
    // specifies desired size of captured snapshot frames
    private Size desiredSnapshotSize = new Size(0, 0);
    // provide snapshots or not
    private bool provideSnapshots = false;

    private Thread thread = null;
    private ManualResetEvent stopEvent = null;

    private VideoCapabilities[] videoCapabilities;
    private VideoCapabilities[] snapshotCapabilities;

    private bool needToSimulateTrigger = false;
    private bool needToDisplayPropertyPage = false;
    private IntPtr parentWindowForPropertyPage = IntPtr.Zero;

    // video capture source object
    private object sourceObject = null;

    // time of starting the DirectX graph
    private DateTime startTime = new DateTime();

    // dummy object to lock for synchronization
    private object sync = new object();

    /// <summary>
    /// Specifies if snapshots should be provided or not.
    /// </summary>
    /// 
    /// <remarks><para>Some USB cameras/devices may have a shutter button, which may result into snapshot if it
    /// is pressed. So the property specifies if the video source will try providing snapshots or not - it will
    /// check if the camera supports providing still image snapshots. If camera supports snapshots and the property
    /// is set to <see langword="true"/>, then snapshots will be provided through <see cref="SnapshotFrame"/>
    /// event.</para>
    /// 
    /// <para>Check supported sizes of snapshots using <see cref="SnapshotCapabilities"/> property and set the
    /// desired size using <see cref="DesiredSnapshotSize"/> property.</para>
    /// 
    /// <para><note>The property must be set before running the video source to take effect.</note></para>
    /// 
    /// <para>Default value of the property is set to <see langword="false"/>.</para>
    /// </remarks>
    ///
    public bool ProvideSnapshots
    {
      get { return provideSnapshots; }
      set { provideSnapshots = value; }
    }

    /// <summary>
    /// New frame event.
    /// </summary>
    /// 
    /// <remarks><para>Notifies clients about new available frame from video source.</para>
    /// 
    /// <para><note>Since video source may have multiple clients, each client is responsible for
    /// making a copy (cloning) of the passed video frame, because the video source disposes its
    /// own original copy after notifying of clients.</note></para>
    /// </remarks>
    /// 
    public event NewFrameEventHandler NewFrame;

    /// <summary>
    /// Snapshot frame event.
    /// </summary>
    /// 
    /// <remarks><para>Notifies clients about new available snapshot frame - the one which comes when
    /// camera's snapshot/shutter button is pressed.</para>
    /// 
    /// <para>See documentation to <see cref="ProvideSnapshots"/> for additional information.</para>
    /// 
    /// <para><note>Since video source may have multiple clients, each client is responsible for
    /// making a copy (cloning) of the passed snapshot frame, because the video source disposes its
    /// own original copy after notifying of clients.</note></para>
    /// </remarks>
    /// 
    /// <seealso cref="ProvideSnapshots"/>
    /// 
    public event NewFrameEventHandler SnapshotFrame;

    /// <summary>
    /// Video source error event.
    /// </summary>
    /// 
    /// <remarks>This event is used to notify clients about any type of errors occurred in
    /// video source object, for example internal exceptions.</remarks>
    /// 
    public event VideoSourceExceptionEventHandler VideoSourceException;

    /// <summary>
    /// Video playing finished event.
    /// </summary>
    /// 
    /// <remarks><para>This event is used to notify clients that the video playing has finished.</para>
    /// </remarks>
    /// 
    public event VideoSourceFinishedEventHandler VideoSourceFinished;

    /// <summary>
    /// Video source.
    /// </summary>
    /// 
    /// <remarks>Video source is represented by moniker string of video capture device.</remarks>
    /// 
    public virtual string Source
    {
      get { return deviceMoniker; }
      set { deviceMoniker = value; }
    }

    /// <summary>
    /// Received frames count.
    /// </summary>
    /// 
    /// <remarks>Number of frames the video source provided from the moment of the last
    /// access to the property.
    /// </remarks>
    /// 
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
    /// Received bytes count.
    /// </summary>
    /// 
    /// <remarks>Number of bytes the video source provided from the moment of the last
    /// access to the property.
    /// </remarks>
    /// 
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
    /// State of the video source.
    /// </summary>
    /// 
    /// <remarks>Current state of video source object - running or not.</remarks>
    /// 
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
    /// Desired size of captured video frames.
    /// </summary>
    /// 
    /// <remarks><para>The property sets desired video frame size. However capture
    /// device may not always provide video frames of configured size due to the fact
    /// that the size is not supported by it.</para>
    /// 
    /// <para>If the property is set to size (0, 0), then capture device uses its own
    /// default video frame size configuration.</para>
    /// 
    /// <para>Default value of the property is set to (0, 0).</para>
    /// 
    /// <para><note>The property should be configured before video source is started
    /// to take effect.</note></para></remarks>
    /// 
    public Size DesiredFrameSize
    {
      get { return desiredFrameSize; }
      set { desiredFrameSize = value; }
    }

    /// <summary>
    /// Desired size of captured snapshot frames.
    /// </summary>
    /// 
    /// <remarks><para>The property sets desired snapshot size. However capture
    /// device may not always provide snapshots of configured size due to the fact
    /// that the size is not supported by it.</para>
    /// 
    /// <para>If the property is set to size (0, 0), then capture device uses its own
    /// default snapshot size configuration.</para>
    /// 
    /// <para>See documentation to <see cref="ProvideSnapshots"/> for additional information.</para>
    /// 
    /// <para>Default value of the property is set to (0, 0).</para>
    /// 
    /// <para><note>The property should be configured before video source is started
    /// to take effect.</note></para></remarks>
    /// 
    /// <seealso cref="ProvideSnapshots"/>
    /// 
    public Size DesiredSnapshotSize
    {
      get { return desiredSnapshotSize; }
      set { desiredSnapshotSize = value; }
    }

    /// <summary>
    /// Desired capture frame rate.
    /// </summary>
    /// 
    /// <remarks><para>The property sets desired capture frame rate. However capture
    /// device may not always provide the exact configured frame rate due to its
    /// capabilities, system performance, etc.</para>
    /// 
    /// <para>If the property is set to 0, then capture device uses its own default
    /// frame rate.</para>
    /// 
    /// <para>Default value of the property is set to 0.</para>
    /// 
    /// <para><note>The property should be configured before video source is started
    /// to take effect.</note></para></remarks>
    /// 
    public int DesiredFrameRate
    {
      get { return desiredFrameRate; }
      set { desiredFrameRate = value; }
    }

    /// <summary>
    /// Video capabilities of the device.
    /// </summary>
    /// 
    /// <remarks><para>The property provides list of device's video capabilities.</para>
    /// 
    /// <para><note>Do not call this property immediately after <see cref="Start"/> method, since
    /// device may not start yet and provide its information. It is better to call the property
    /// before starting device or a bit after (but not immediately after).</note></para>
    /// </remarks>
    /// 
    public VideoCapabilities[] VideoCapabilities
    {
      get
      {
        if (videoCapabilities == null)
        {
          if (!IsRunning)
          {
            // create graph without playing to get the video/snapshot capabilities only.
            // not very clean but it works
            WorkerThread(false);
          }
          else
          {
            for (int i = 0; (i < 500) && (videoCapabilities == null); i++)
            {
              Thread.Sleep(10);
            }
          }
        }
        return videoCapabilities;
      }
    }

    /// <summary>
    /// Snapshot capabilities of the device.
    /// </summary>
    /// 
    /// <remarks><para>The property provides list of device's snapshot capabilities.</para>
    /// 
    /// <para>If the array has zero length, then it means that this device does not support making
    /// snapshots.</para>
    /// 
    /// <para>See documentation to <see cref="ProvideSnapshots"/> for additional information.</para>
    /// 
    /// <para><note>Do not call this property immediately after <see cref="Start"/> method, since
    /// device may not start yet and provide its information. It is better to call the property
    /// before starting device or a bit after (but not immediately after).</note></para>
    /// </remarks>
    /// 
    /// <seealso cref="ProvideSnapshots"/>
    /// 
    public VideoCapabilities[] SnapshotCapabilities
    {
      get
      {
        if (snapshotCapabilities == null)
        {
          if (!IsRunning)
          {
            // create graph without playing to get the video/snapshot capabilities only.
            // not very clean but it works
            WorkerThread(false);
          }
          else
          {
            for (int i = 0; (i < 500) && (snapshotCapabilities == null); i++)
            {
              Thread.Sleep(10);
            }
          }
        }
        return snapshotCapabilities;
      }
    }

    /// <summary>
    /// Source COM object of camera capture device.
    /// </summary>
    /// 
    /// <remarks><para>The source COM object of camera capture device is exposed for the
    /// case when user may need get direct access to the object for making some custom
    /// configuration of camera through DirectShow interface, for example.
    /// </para>
    /// 
    /// <para>If camera is not running, the property is set to <see langword="null"/>.</para>
    /// </remarks>
    /// 
    public object SourceObject
    {
      get { return sourceObject; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoCaptureDeviceVideoSource"/> class.
    /// </summary>
    /// 
    public VideoCaptureDeviceVideoSource() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoCaptureDeviceVideoSource"/> class.
    /// </summary>
    /// 
    /// <param name="deviceMoniker">Moniker string of video capture device.</param>
    /// 
    public VideoCaptureDeviceVideoSource(string deviceMoniker)
    {
      this.deviceMoniker = deviceMoniker;
    }

    /// <summary>
    /// Start video source.
    /// </summary>
    /// 
    /// <remarks>Starts video source and return execution to caller. Video source
    /// object creates background thread and notifies about new frames with the
    /// help of <see cref="NewFrame"/> event.</remarks>
    /// 
    public void Start()
    {
      if (!IsRunning)
      {
        // check source
        if ((deviceMoniker == null) || (deviceMoniker == string.Empty))
          throw new ArgumentException("Video source is not specified");

        framesReceived = 0;
        bytesReceived = 0;

        // create events
        stopEvent = new ManualResetEvent(false);

        lock (sync)
        {
          // create and start new thread
          thread = new Thread(new ThreadStart(WorkerThread));
          thread.Name = deviceMoniker; // mainly for debugging
          thread.Start();
        }
      }
    }

    /// <summary>
    /// Signal video source to stop its work.
    /// </summary>
    /// 
    /// <remarks>Signals video source to stop its background thread, stop to
    /// provide new frames and free resources.</remarks>
    /// 
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
    /// Wait for video source has stopped.
    /// </summary>
    /// 
    /// <remarks>Waits for source stopping after it was signalled to stop using
    /// <see cref="SignalToStop"/> method.</remarks>
    /// 
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
    /// Stop video source.
    /// </summary>
    /// 
    /// <remarks><para>Stops video source aborting its thread.</para>
    /// 
    /// <para><note>Since the method aborts background thread, its usage is highly not preferred
    /// and should be done only if there are no other options. The correct way of stopping camera
    /// is <see cref="SignalToStop">signaling it stop</see> and then
    /// <see cref="WaitForStop">waiting</see> for background thread's completion.</note></para>
    /// </remarks>
    /// 
    public void Stop()
    {
      if (this.IsRunning)
      {
        thread.Abort();
        WaitForStop();
      }
    }

    /// <summary>
    /// Free resource.
    /// </summary>
    /// 
    private void Free()
    {
      thread = null;

      // release events
      stopEvent.Close();
      stopEvent = null;
    }

    /// <summary>
    /// Display property window for the video capture device providing its configuration
    /// capabilities.
    /// </summary>
    /// 
    /// <param name="parentWindow">Handle of parent window.</param>
    /// 
    /// <remarks><para><note>If you pass parent window's handle to this method, then the
    /// displayed property page will become modal window and none of the controls from the
    /// parent window will be accessible. In order to make it modeless it is required
    /// to pass <see cref="IntPtr.Zero"/> as parent window's handle.
    /// </note></para>
    /// </remarks>
    /// 
    /// <exception cref="NotSupportedException">The video source does not support configuration property page.</exception>
    /// 
    public void DisplayPropertyPage(IntPtr parentWindow)
    {
      // check source
      if ((deviceMoniker == null) || (deviceMoniker == string.Empty))
        throw new ArgumentException("Video source is not specified");

      lock (sync)
      {
        if (IsRunning)
        {
          // pass the request to backgroud thread if video source is running
          parentWindowForPropertyPage = parentWindow;
          needToDisplayPropertyPage = true;
          return;
        }

        object tempSourceObject = null;

        // create source device's object
        try
        {
          tempSourceObject = FilterInfo.CreateFilter(deviceMoniker);
        }
        catch
        {
          throw new ApplicationException("Failed creating device object for moniker.");
        }

        if (!(tempSourceObject is ISpecifyPropertyPages))
        {
          throw new NotSupportedException("The video source does not support configuration property page.");
        }

        // retrieve ISpecifyPropertyPages interface of the device
        ISpecifyPropertyPages pPropPages = (ISpecifyPropertyPages)tempSourceObject;

        // get property pages from the property bag
        CAUUID caGUID;
        pPropPages.GetPages(out caGUID);

        // get filter info
        FilterInfo filterInfo = new FilterInfo(deviceMoniker);

        // create and display the OlePropertyFrame form
        Win32.OleCreatePropertyFrame(parentWindow, 0, 0, filterInfo.Name, 1, ref tempSourceObject, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero);

        // release COM objects
        Marshal.FreeCoTaskMem(caGUID.pElems);
        Marshal.ReleaseComObject(tempSourceObject);
      }
    }

    /// <summary>
    /// Simulates an external trigger.
    /// </summary>
    /// 
    /// <remarks><para>The method simulates external trigger for video cameras, which support
    /// providing still image snapshots. The effect is equivalent as pressing camera's shutter
    /// button - a snapshot will be provided through <see cref="SnapshotFrame"/> event.</para>
    /// 
    /// <para><note>The <see cref="ProvideSnapshots"/> property must be set to <see langword="true"/>
    /// to enable receiving snapshots.</note></para>
    /// </remarks>
    /// 
    public void SimulateTrigger()
    {
      needToSimulateTrigger = true;
    }

    /// <summary>
    /// Worker thread.
    /// </summary>
    /// 
    private void WorkerThread()
    {
      WorkerThread(true);
    }

    private void WorkerThread(bool runGraph)
    {
      VideoSourceFinishedReasonType reasonToStop = VideoSourceFinishedReasonType.StoppedByUser;
      bool isSapshotSupported = false;

      // grabber
      Grabber videoGrabber = new Grabber(this, false);
      Grabber snapshotGrabber = new Grabber(this, true);

      // objects
      object captureGraphObject = null;
      object graphObject = null;
      object videoGrabberObject = null;
      object snapshotGrabberObject = null;

      // interfaces
      ICaptureGraphBuilder2 captureGraph = null;
      IFilterGraph2 graph = null;
      IBaseFilter sourceBase = null;
      IBaseFilter videoGrabberBase = null;
      IBaseFilter snapshotGrabberBase = null;
      ISampleGrabber videoSampleGrabber = null;
      ISampleGrabber snapshotSampleGrabber = null;
      IMediaControl mediaControl = null;
      IAMVideoControl videoControl = null;
      IMediaEventEx mediaEvent = null;
      IPin pinStillImage = null;

      try
      {
        // get type of capture graph builder
        Type type = Type.GetTypeFromCLSID(Clsid.CaptureGraphBuilder2);
        if (type == null)
          throw new ApplicationException("Failed creating capture graph builder");

        // create capture graph builder
        captureGraphObject = Activator.CreateInstance(type);
        captureGraph = (ICaptureGraphBuilder2)captureGraphObject;

        // get type of filter graph
        type = Type.GetTypeFromCLSID(Clsid.FilterGraph);
        if (type == null)
          throw new ApplicationException("Failed creating filter graph");

        // create filter graph
        graphObject = Activator.CreateInstance(type);
        graph = (IFilterGraph2)graphObject;

        // set filter graph to the capture graph builder
        captureGraph.SetFiltergraph((IGraphBuilder)graph);

        // create source device's object
        sourceObject = FilterInfo.CreateFilter(deviceMoniker);
        if (sourceObject == null)
          throw new ApplicationException("Failed creating device object for moniker");

        // get base filter interface of source device
        sourceBase = (IBaseFilter)sourceObject;

        // get video control interface of the device
        try
        {
          videoControl = (IAMVideoControl)sourceObject;
        }
        catch
        {
          // some camera drivers may not support IAMVideoControl interface
        }

        // get type of sample grabber
        type = Type.GetTypeFromCLSID(Clsid.SampleGrabber);
        if (type == null)
          throw new ApplicationException("Failed creating sample grabber");

        // create sample grabber used for video capture
        videoGrabberObject = Activator.CreateInstance(type);
        videoSampleGrabber = (ISampleGrabber)videoGrabberObject;
        videoGrabberBase = (IBaseFilter)videoGrabberObject;
        // create sample grabber used for snapshot capture
        snapshotGrabberObject = Activator.CreateInstance(type);
        snapshotSampleGrabber = (ISampleGrabber)snapshotGrabberObject;
        snapshotGrabberBase = (IBaseFilter)snapshotGrabberObject;

        // add source and grabber filters to graph
        graph.AddFilter(sourceBase, "source");
        graph.AddFilter(videoGrabberBase, "grabber_video");
        graph.AddFilter(snapshotGrabberBase, "grabber_snapshot");

        // set media type
        AMMediaType mediaType = new AMMediaType();
        mediaType.MajorType = MediaType.Video;
        mediaType.SubType = MediaSubType.RGB24;

        videoSampleGrabber.SetMediaType(mediaType);
        snapshotSampleGrabber.SetMediaType(mediaType);

        if (videoControl != null)
        {
          // find Still Image output pin of the vedio device
          captureGraph.FindPin(sourceObject, PinDirection.Output,
              PinCategory.StillImage, MediaType.Video, false, 0, out pinStillImage);
          // check if it support trigger mode
          if (pinStillImage != null)
          {
            VideoControlFlags caps;
            videoControl.GetCaps(pinStillImage, out caps);
            isSapshotSupported = ((caps & VideoControlFlags.ExternalTriggerEnable) != 0);
          }
        }

        // configure video sample grabber
        videoSampleGrabber.SetBufferSamples(false);
        videoSampleGrabber.SetOneShot(false);
        videoSampleGrabber.SetCallback(videoGrabber, 1);

        // configure snapshot sample grabber
        snapshotSampleGrabber.SetBufferSamples(true);
        snapshotSampleGrabber.SetOneShot(false);
        snapshotSampleGrabber.SetCallback(snapshotGrabber, 1);

        // configure pins
        GetPinCapabilitiesAndConfigureSizeAndRate(captureGraph, sourceBase,
            PinCategory.Capture, desiredFrameSize, desiredFrameRate, ref videoCapabilities);
        if (isSapshotSupported)
        {
          GetPinCapabilitiesAndConfigureSizeAndRate(captureGraph, sourceBase,
              PinCategory.StillImage, desiredSnapshotSize, 0, ref snapshotCapabilities);
        }
        else
        {
          snapshotCapabilities = new VideoCapabilities[0];
        }

        if (runGraph)
        {
          // render capture pin
          captureGraph.RenderStream(PinCategory.Capture, MediaType.Video, sourceBase, null, videoGrabberBase);

          if (videoSampleGrabber.GetConnectedMediaType(mediaType) == 0)
          {
            VideoInfoHeader vih = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader));

            videoGrabber.Width = vih.BmiHeader.Width;
            videoGrabber.Height = vih.BmiHeader.Height;

            mediaType.Dispose();
          }

          if ((isSapshotSupported) && (provideSnapshots))
          {
            // render snapshot pin
            captureGraph.RenderStream(PinCategory.StillImage, MediaType.Video, sourceBase, null, snapshotGrabberBase);

            if (snapshotSampleGrabber.GetConnectedMediaType(mediaType) == 0)
            {
              VideoInfoHeader vih = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader));

              snapshotGrabber.Width = vih.BmiHeader.Width;
              snapshotGrabber.Height = vih.BmiHeader.Height;

              mediaType.Dispose();
            }
          }

          // get media control
          mediaControl = (IMediaControl)graphObject;

          // get media events' interface
          mediaEvent = (IMediaEventEx)graphObject;
          IntPtr p1, p2;
          DsEvCode code;

          // run
          mediaControl.Run();

          if ((isSapshotSupported) && (provideSnapshots))
          {
            startTime = DateTime.Now;
            videoControl.SetMode(pinStillImage, VideoControlFlags.ExternalTriggerEnable);
          }

          while (!stopEvent.WaitOne(0, false))
          {
            Thread.Sleep(100);

            if (mediaEvent != null)
            {
              if (mediaEvent.GetEvent(out code, out p1, out p2, 0) >= 0)
              {
                mediaEvent.FreeEventParams(code, p1, p2);

                if (code == DsEvCode.DeviceLost)
                {
                  reasonToStop = VideoSourceFinishedReasonType.DeviceLost;
                  break;
                }
              }
            }

            if (needToSimulateTrigger)
            {
              needToSimulateTrigger = false;

              if ((isSapshotSupported) && (provideSnapshots))
              {
                videoControl.SetMode(pinStillImage, VideoControlFlags.Trigger);
              }
            }

            if (needToDisplayPropertyPage)
            {
              needToDisplayPropertyPage = false;

              try
              {
                // retrieve ISpecifyPropertyPages interface of the device
                ISpecifyPropertyPages pPropPages = (ISpecifyPropertyPages)sourceObject;

                // get property pages from the property bag
                CAUUID caGUID;
                pPropPages.GetPages(out caGUID);

                // get filter info
                FilterInfo filterInfo = new FilterInfo(deviceMoniker);

                // create and display the OlePropertyFrame
                Win32.OleCreatePropertyFrame(parentWindowForPropertyPage, 0, 0, filterInfo.Name, 1, ref sourceObject, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero);

                // release COM objects
                Marshal.FreeCoTaskMem(caGUID.pElems);
              }
              catch
              {
              }
            }
          }
          mediaControl.Stop();
        }
      }
      catch (Exception exception)
      {
        // provide information to clients
        if (VideoSourceException != null)
        {
          VideoSourceException(this, new VideoSourceExceptionEventArgs(exception.Message));
        }
      }
      finally
      {
        // release all objects
        captureGraph = null;
        graph = null;
        sourceBase = null;
        mediaControl = null;
        videoControl = null;
        mediaEvent = null;
        pinStillImage = null;

        videoGrabberBase = null;
        snapshotGrabberBase = null;
        videoSampleGrabber = null;
        snapshotSampleGrabber = null;

        if (graphObject != null)
        {
          Marshal.ReleaseComObject(graphObject);
          graphObject = null;
        }
        if (sourceObject != null)
        {
          Marshal.ReleaseComObject(sourceObject);
          sourceObject = null;
        }
        if (videoGrabberObject != null)
        {
          Marshal.ReleaseComObject(videoGrabberObject);
          videoGrabberObject = null;
        }
        if (snapshotGrabberObject != null)
        {
          Marshal.ReleaseComObject(snapshotGrabberObject);
          snapshotGrabberObject = null;
        }
        if (captureGraphObject != null)
        {
          Marshal.ReleaseComObject(captureGraphObject);
          captureGraphObject = null;
        }
      }

      if (VideoSourceFinished != null)
      {
        VideoSourceFinished(this, new VideoSourceFinishedEventArgs(reasonToStop));
      }
    }

    // Set frame's size and rate for the specified stream configuration
    private void SetFrameSizeAndRate(IAMStreamConfig streamConfig, Size size, int frameRate)
    {
      bool sizeSet = false;
      AMMediaType mediaType;

      // get current format
      streamConfig.GetFormat(out mediaType);

      // change frame size if required
      if ((size.Width != 0) && (size.Height != 0))
      {
        // iterate through device's capabilities to find mediaType for desired resolution
        int capabilitiesCount = 0, capabilitySize = 0;
        AMMediaType newMediaType = null;
        VideoStreamConfigCaps caps = new VideoStreamConfigCaps();

        streamConfig.GetNumberOfCapabilities(out capabilitiesCount, out capabilitySize);

        for (int i = 0; i < capabilitiesCount; i++)
        {
          if (streamConfig.GetStreamCaps(i, out newMediaType, caps) == 0)
          {
            if (caps.InputSize == size)
            {
              mediaType.Dispose();
              mediaType = newMediaType;
              sizeSet = true;
              break;
            }
            else
            {
              newMediaType.Dispose();
            }
          }
        }
      }

      VideoInfoHeader infoHeader = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader));

      // try changing size manually if failed finding mediaType before
      if ((size.Width != 0) && (size.Height != 0) && (!sizeSet))
      {
        infoHeader.BmiHeader.Width = size.Width;
        infoHeader.BmiHeader.Height = size.Height;
      }
      // change frame rate if required
      if (frameRate != 0)
      {
        infoHeader.AverageTimePerFrame = 10000000 / frameRate;
      }

      // copy the media structure back
      Marshal.StructureToPtr(infoHeader, mediaType.FormatPtr, false);

      // set the new format
      streamConfig.SetFormat(mediaType);

      mediaType.Dispose();
    }

    // Configure specified pin and collect its capabilities if required
    private void GetPinCapabilitiesAndConfigureSizeAndRate(ICaptureGraphBuilder2 graphBuilder, IBaseFilter baseFilter,
        Guid pinCategory, Size size, int frameRate, ref VideoCapabilities[] capabilities)
    {
      object streamConfigObject;
      graphBuilder.FindInterface(pinCategory, MediaType.Video, baseFilter, typeof(IAMStreamConfig).GUID, out streamConfigObject);

      if (streamConfigObject != null)
      {
        IAMStreamConfig streamConfig = null;

        try
        {
          streamConfig = (IAMStreamConfig)streamConfigObject;
        }
        catch (InvalidCastException)
        {
        }

        if (streamConfig != null)
        {
          if (capabilities == null)
          {
            try
            {
              // get all video capabilities
              capabilities = Gimela.Media.Video.DirectShow.VideoCapabilities.FromStreamConfig(streamConfig);
            }
            catch
            {
            }
          }

          // check if it is required to change capture settings
          if ((frameRate != 0) || ((size.Width != 0) && (size.Height != 0)))
          {
            SetFrameSizeAndRate(streamConfig, size, frameRate);
          }
        }
      }

      // if failed resolving capabilities, then just create empty capabilities array,
      // so we don't try again
      if (capabilities == null)
      {
        capabilities = new VideoCapabilities[0];
      }
    }

    /// <summary>
    /// Notifies clients about new frame.
    /// </summary>
    /// <param name="frame">New frame's image.</param>
    /// <param name="timestamp">The timestamp.</param>
    private void OnNewFrame(Bitmap frame, DateTime timestamp)
    {
      framesReceived++;
      if ((!stopEvent.WaitOne(0, false)) && (NewFrame != null))
        NewFrame(this, new NewFrameEventArgs(frame, timestamp));
    }

    /// <summary>
    /// Notifies clients about new snapshot frame.
    /// </summary>
    /// <param name="image">New snapshot's image.</param>
    /// <param name="timestamp">The timestamp.</param>
    private void OnSnapshotFrame(Bitmap image, DateTime timestamp)
    {
      TimeSpan timeSinceStarted = DateTime.Now - startTime;

      // TODO: need to find better way to ignore the first snapshot, which is sent
      // automatically (or better disable it)
      if (timeSinceStarted.TotalSeconds >= 4)
      {
        if ((!stopEvent.WaitOne(0, false)) && (SnapshotFrame != null))
          SnapshotFrame(this, new NewFrameEventArgs(image, timestamp));
      }
    }

    //
    // Video grabber
    //
    private class Grabber : ISampleGrabberCB
    {
      private VideoCaptureDeviceVideoSource parent;
      private bool snapshotMode;
      private int width, height;

      // Width property
      public int Width
      {
        get { return width; }
        set { width = value; }
      }
      // Height property
      public int Height
      {
        get { return height; }
        set { height = value; }
      }

      // Constructor
      public Grabber(VideoCaptureDeviceVideoSource parent, bool snapshotMode)
      {
        this.parent = parent;
        this.snapshotMode = snapshotMode;
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
          System.Drawing.Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

          // lock bitmap data
          BitmapData imageData = image.LockBits(
              new Rectangle(0, 0, width, height),
              ImageLockMode.ReadWrite,
              PixelFormat.Format24bppRgb);

          // copy image data
          int srcStride = imageData.Stride;
          int dstStride = imageData.Stride;

          unsafe
          {
            byte* dst = (byte*)imageData.Scan0.ToPointer() + dstStride * (height - 1);
            byte* src = (byte*)buffer.ToPointer();

            for (int y = 0; y < height; y++)
            {
              Win32.memcpy(dst, src, srcStride);
              dst -= dstStride;
              src += srcStride;
            }
          }

          // unlock bitmap data
          image.UnlockBits(imageData);

          // notify parent
          if (snapshotMode)
          {
            parent.OnSnapshotFrame(image, DateTime.Now);
          }
          else
          {
            parent.OnNewFrame(image, DateTime.Now);
          }

          // release the image
          image.Dispose();
        }

        return 0;
      }
    }
  }
}
