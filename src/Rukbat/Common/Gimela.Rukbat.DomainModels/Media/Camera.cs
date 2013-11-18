using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Gimela.Common.Logging;
using Gimela.Media.Imaging;
using Gimela.Media.Video;
using Gimela.Rukbat.DomainModels.MediaSource;
using Gimela.Media.Vision.Motion;

namespace Gimela.Rukbat.DomainModels
{
  [Serializable]
  [DataContract]
  public class Camera : MediaObject, ICamera
  {
    #region Fields

    private readonly ILogger _logger = null;
    private UnmanagedImage _lastFrame = null;
    private BitmapImage _thumbnailSource = null;

    #endregion

    #region Ctors

    public Camera()
    {
      _logger = LogFactory.CreateLogger();

      InitializeCameraDecorators();
    }

    #endregion

    #region ISystemObject Members

    public override void RefreshObjectImage()
    {

    }

    #endregion

    #region ICamera Members

    public BitmapSource ThumbnailSource
    {
      get
      {
        if (_thumbnailSource == null)
        {
          if (Thumbnail != null && Thumbnail.Length > 0)
          {
            using (MemoryStream stream = new MemoryStream(Thumbnail))
            {
              _thumbnailSource = new BitmapImage();
              _thumbnailSource.BeginInit();
              _thumbnailSource.StreamSource = stream;
              _thumbnailSource.CacheOption = BitmapCacheOption.OnLoad;
              _thumbnailSource.EndInit();
              _thumbnailSource.Freeze();
            }
          }
        }

        return _thumbnailSource;
      }
    }

    public byte[] Thumbnail { get; set; }

    public void Start()
    {
      BindVideoSource();

      if (VideoSource != null)
      {
        VideoSource.Start();
      }

      RaiseCameraStartedEvent();
    }

    public void Stop()
    {
      if (VideoSource != null)
      {
        VideoSource.Stop();
      }

      RaiseCameraStoppedEvent();

      UnbindVideoSource();
    }

    public event EventHandler<CameraAlarmEventArgs> CameraAlarmEvent;
    public event EventHandler<CameraStartedEventArgs> CameraStartedEvent;
    public event EventHandler<CameraStoppedEventArgs> CameraStoppedEvent;

    protected void RaiseCameraAlarmEvent(string alarmDescription)
    {
      if (CameraAlarmEvent != null)
      {
        CameraAlarmEvent(this, new CameraAlarmEventArgs(this.Id, new MediaAlarm(alarmDescription)));
      }
    }

    protected void RaiseCameraStartedEvent()
    {
      if (CameraStartedEvent != null)
      {
        CameraStartedEvent(this, new CameraStartedEventArgs(this.Id));
      }
    }

    protected void RaiseCameraStoppedEvent()
    {
      if (CameraStoppedEvent != null)
      {
        CameraStoppedEvent(this, new CameraStoppedEventArgs(this.Id));
      }
    }

    #endregion

    #region ICameraDecorator Members

    public bool IsFlipX { get; set; }
    public bool IsFlipY { get; set; }

    public bool IsDisplayTimestamp { get; set; }
    public string TimestampFormat { get; set; }
    public string TimestampFontFamily { get; set; }
    public float TimestampFontSize { get; set; }
    public Color TimestampColor { get; set; }
    public bool IsDisplayTimestampBackground { get; set; }
    public Color TimestampBackgroundColor { get; set; }

    public bool IsDisplayOnScreenDisplay { get; set; }
    public string OnScreenDisplayText { get; set; }
    public string OnScreenDisplayFontFamily { get; set; }
    public float OnScreenDisplayFontSize { get; set; }
    public Color OnScreenDisplayColor { get; set; }
    public bool IsDisplayOnScreenDisplayBackground { get; set; }
    public Color OnScreenDisplayBackgroundColor { get; set; }

    public bool IsDisplayLogo { get; set; }
    public Image LogoImage { get; set; }
    public Point LogoPoint { get; set; }
    public Size LogoSize { get; set; }

    public bool IsDisplayProtectionMask { get; set; }
    public Image ProtectionMaskImage { get; set; }
    public Point ProtectionMaskPoint { get; set; }
    public Size ProtectionMaskSize { get; set; }

    public bool IsMotionDetection { get; set; }
    public MotionDetector MotionDetector { get; set; }

    private void InitializeCameraDecorators()
    {
      IsFlipX = false;
      IsFlipY = false;

      IsDisplayTimestamp = true;
      TimestampFormat = @"yyyy-MM-dd HH:mm:ss";
      TimestampFontFamily = "Consolas";
      TimestampFontSize = 20;
      TimestampColor = Color.Red;
      IsDisplayTimestampBackground = true;
      TimestampBackgroundColor = Color.FromArgb(128, 0, 0, 0);

      IsDisplayOnScreenDisplay = true;
      OnScreenDisplayText = Name;
      OnScreenDisplayFontFamily = "Consolas";
      OnScreenDisplayFontSize = 20;
      OnScreenDisplayColor = Color.Red;
      IsDisplayOnScreenDisplayBackground = true;
      OnScreenDisplayBackgroundColor = Color.FromArgb(128, 0, 0, 0);

      IsDisplayLogo = false;
      LogoPoint = new Point(0, 0);
      LogoSize = new Size(32, 32);

      IsDisplayProtectionMask = false;
      ProtectionMaskPoint = new Point(0, 0);
      ProtectionMaskSize = new Size(32, 32);

      IsMotionDetection = false;
    }

    #endregion

    #region IFrame Members

    public event EventHandler NewFrameEvent;

    protected void RaiseNewFrameEvent()
    {
      if (NewFrameEvent != null)
      {
        NewFrameEvent(this, new EventArgs());
      }
    }

    [IgnoreDataMember]
    [XmlIgnore]
    public bool IsLastFrameNull { get; private set; }

    [IgnoreDataMember]
    [XmlIgnore]
    public Bitmap LastFrame
    {
      get
      {
        Bitmap bitmap = null;

        lock (this)
        {
          if (_lastFrame != null)
          {
            try
            {
              bitmap = _lastFrame.ToManagedImage();
              IsLastFrameNull = false;
            }
            catch
            {
              if (bitmap != null)
              {
                bitmap.Dispose();
              }

              IsLastFrameNull = true;
            }
          }
        }

        return bitmap;
      }
    }

    private void SetUnmanagedLastFrame(Bitmap frame)
    {
      lock (this)
      {
        try
        {
          if (_lastFrame != null)
            _lastFrame.Dispose();

          _lastFrame = UnmanagedImage.FromManagedImage(frame);
        }
        catch { }
      }
    }

    #endregion

    #region Video Source

    [DataMember]
    [XmlElement(typeof(VideoSourceDescription))]
    public VideoSourceDescription VideoSourceDescription { get; set; }

    [IgnoreDataMember]
    [XmlIgnore]
    public IVideoSource VideoSource { get; private set; }

    private void BindVideoSource()
    {
      if (VideoSourceDescription == null)
        throw new InvalidProgramException("VideoSourceDescription cannot be null.");

      VideoSource = VideoSourceFactory.BuildVideoSource(VideoSourceDescription);

      VideoSource.VideoSourceFinished += new VideoSourceFinishedEventHandler(OnVideoSourceFinished);
      VideoSource.VideoSourceException += new VideoSourceExceptionEventHandler(OnVideoSourceException);
      VideoSource.NewFrame += new NewFrameEventHandler(OnVideoSourceNewFrame);
    }

    private void UnbindVideoSource()
    {
      if (VideoSourceDescription != null && VideoSource != null)
      {
        VideoSource.VideoSourceFinished -= new VideoSourceFinishedEventHandler(OnVideoSourceFinished);
        VideoSource.VideoSourceException -= new VideoSourceExceptionEventHandler(OnVideoSourceException);
        VideoSource.NewFrame -= new NewFrameEventHandler(OnVideoSourceNewFrame);

        VideoSource = null;
      }
    }

    void OnVideoSourceException(object sender, VideoSourceExceptionEventArgs e)
    {
      if (_logger != null)
      {
        Logger.Error(string.Format(CultureInfo.InvariantCulture, @"{0} : {1}", sender.GetType().ToString(), e.ToString()));
      }
    }

    void OnVideoSourceFinished(object sender, VideoSourceFinishedEventArgs e)
    {
      if (_logger != null)
      {
        Logger.Info(string.Format(CultureInfo.InvariantCulture, @"{0} : {1}", sender.GetType().ToString(), e.ToString()));
      }
    }

    #endregion

    #region New Frame Process

    private void OnVideoSourceNewFrame(object sender, NewFrameEventArgs e)
    {
      if (e == null || e.Frame == null) return;

      DateTime now = DateTime.Now;
      if (now > e.Timestamp && (now - e.Timestamp).TotalSeconds >= 3)
      {
        // 如果时差超过一定时间则丢帧
        return;
      }

      try
      {
        Bitmap frame = e.Frame;

        // 原始图像翻转
        ProcessImageRotation(ref frame);

        // 原始图像时间戳
        ProcessImageTimestamp(e.Timestamp, ref frame);

        // 原始图像OSD
        ProcessImageOnScreenDisplay(ref frame);

        // 原始图像Logo
        ProcessImageLogo(ref frame);

        // 原始图像遮挡
        ProcessImageProtectionMask(ref frame);

        // 原始图像移动侦测
        ProcessImageMotionDetection(ref frame);

        // 设置非托管内存
        SetUnmanagedLastFrame(frame);

        // 触发新帧事件
        RaiseNewFrameEvent();
      }
      catch (Exception ex)
      {
        Logger.Error(ex.Message);
      }
    }

    #endregion

    #region Image Preprocessing

    private void ProcessImageRotation(ref Bitmap frame)
    {
      if (IsFlipX)
      {
        frame.RotateFlip(RotateFlipType.RotateNoneFlipX);
      }
      if (IsFlipY)
      {
        frame.RotateFlip(RotateFlipType.RotateNoneFlipY);
      }
    }

    private void ProcessImageTimestamp(DateTime timestamp, ref Bitmap frame)
    {
      if (IsDisplayTimestamp)
      {
        Font font = new Font(TimestampFontFamily, TimestampFontSize);
        SolidBrush textBrush = new SolidBrush(TimestampColor);
        SolidBrush rectBrush = new SolidBrush(TimestampBackgroundColor);

        string text = timestamp.ToString(TimestampFormat, CultureInfo.InvariantCulture);

        Graphics graphics = Graphics.FromImage(frame);
        Size size = graphics.MeasureString(text, font).ToSize();
        Point point = new Point(frame.Width - size.Width - 1, 1); // 图像右上角

        if (IsDisplayTimestampBackground)
        {
          Rectangle rect = new Rectangle(point, size);
          graphics.FillRectangle(rectBrush, rect);
        }

        graphics.DrawString(text, font, textBrush, point);
      }
    }

    private void ProcessImageOnScreenDisplay(ref Bitmap frame)
    {
      if (IsDisplayOnScreenDisplay)
      {
        if (string.IsNullOrEmpty(OnScreenDisplayText))
        {
          OnScreenDisplayText = Name;
        }

        Font font = new Font(OnScreenDisplayFontFamily, OnScreenDisplayFontSize);
        SolidBrush textBrush = new SolidBrush(OnScreenDisplayColor);
        SolidBrush rectBrush = new SolidBrush(OnScreenDisplayBackgroundColor);

        Graphics graphics = Graphics.FromImage(frame);
        Size size = graphics.MeasureString(OnScreenDisplayText, font).ToSize();
        Point point = new Point(frame.Width - size.Width - 1, frame.Height - size.Height - 1); // 图像右下角

        if (IsDisplayOnScreenDisplayBackground)
        {
          Rectangle rect = new Rectangle(point, size);
          graphics.FillRectangle(rectBrush, rect);
        }

        graphics.DrawString(OnScreenDisplayText, font, textBrush, point);
      }
    }

    private void ProcessImageLogo(ref Bitmap frame)
    {
      if (IsDisplayLogo)
      {
        Graphics graphics = Graphics.FromImage(frame);
        graphics.DrawImage(LogoImage, LogoPoint.X, LogoPoint.Y, LogoSize.Width, LogoSize.Height);
      }
    }

    private void ProcessImageProtectionMask(ref Bitmap frame)
    {
      if (IsDisplayProtectionMask)
      {
        Graphics graphics = Graphics.FromImage(frame);
        graphics.DrawImage(ProtectionMaskImage, ProtectionMaskPoint.X, ProtectionMaskPoint.Y, ProtectionMaskSize.Width, ProtectionMaskSize.Height);
      }
    }

    private void ProcessImageMotionDetection(ref Bitmap frame)
    {
      if (IsMotionDetection && MotionDetector != null)
      {
        float motionLevel = MotionDetector.ProcessFrame(frame);
        if (motionLevel >= 0.0005)
        {
          RaiseCameraAlarmEvent("Motion detected.");
        }
      }
    }

    #endregion

    #region IClonable

    public object Clone()
    {
      Camera camera = new Camera();

      camera.Id = this.Id;
      camera.Name = this.Name;
      camera.HostName = this.HostName;
      camera.Tags = this.Tags;
      camera.Description = this.Description;
      camera.VideoSourceDescription = this.VideoSourceDescription;
      camera.Thumbnail = this.Thumbnail;

      return camera;
    }

    #endregion
  }
}
