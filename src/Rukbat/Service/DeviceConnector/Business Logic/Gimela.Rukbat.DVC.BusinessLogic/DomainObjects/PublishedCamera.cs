using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Gimela.Media.Video;
using Gimela.Rukbat.DVC.BusinessEntities;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public class PublishedCamera : ICamera, ICameraStream, IDisposable
  {
    private bool _disposed = false;
    private Camera _camera;
    private readonly object _sendLock = new object();
    private bool _isSending = false;
    private List<PublishDestination> _destinations = new List<PublishDestination>();
    private readonly object _destLock = new object();
    private Bitmap _latestFrame = null;
    private readonly object _latestFrameLock = new object();
    private int _sequenceNumber = 0;

    public PublishedCamera(Camera camera)
    {
      if (camera == null)
        throw new ArgumentNullException("camera");

      _camera = camera;
    }

    #region ICamera Members

    public string Id { get { return _camera.Id; } }

    #endregion

    #region ICameraStream Members

    public IVideoSource Stream { get { return _camera.Stream; } }

    #endregion

    public CameraProfile Profile { get { return _camera.Profile; } }

    public ReadOnlyCollection<PublishDestination> Destinations { get { return new ReadOnlyCollection<PublishDestination>(_destinations); } }

    public PublishedCamera AddDestination(PublishDestination destination)
    {
      lock (_destLock)
      {
        if (destination == null)
          throw new ArgumentNullException("destination");
        if (_destinations.Find(p => p == destination) == null)
        {
          _destinations.Add(destination);
        }
        return this;
      }
    }

    public PublishedCamera RemoveDestination(PublishDestination destination)
    {
      lock (_destLock)
      {
        if (destination == null)
          throw new ArgumentNullException("destination");
        _destinations.RemoveAll(p => p == destination);
        return this;
      }
    }

    public PublishedCamera ClearDestination()
    {
      lock (_destLock)
      {
        _destinations.Clear();
        return this;
      }
    }

    public IStreamSender Sender { get; set; }

    public bool IsPublishing { get { return Stream.IsRunning; } }

    public byte[] LatestFrameBytes
    {
      get
      {
        lock (_latestFrameLock)
        {
          if (_latestFrame != null)
          {
            try
            {
              Bitmap bitmap = new Bitmap(_latestFrame.Width, _latestFrame.Height, PixelFormat.Format16bppRgb555);
              Graphics draw = Graphics.FromImage(bitmap);
              draw.DrawImage(_latestFrame, 0, 0);

              using (MemoryStream stream = new MemoryStream())
              {
                bitmap.Save(stream, ImageFormat.Bmp);
                return stream.ToArray();
              }
            }
            catch { }
          }

          return null;
        }
      }
    }

    public Bitmap LatestFrame
    {
      get
      {
        lock (_latestFrameLock)
        {
          if (_latestFrame != null)
          {
            try
            {
              Bitmap bitmap = new Bitmap(_latestFrame.Width, _latestFrame.Height, PixelFormat.Format16bppRgb555);
              Graphics draw = Graphics.FromImage(bitmap);
              draw.DrawImage(_latestFrame, 0, 0);

              // everytime we get the latest will make a copy
              return bitmap;
            }
            catch { }
          }

          return null;
        }
      }
      private set
      {
        lock (_latestFrameLock)
        {
          if (_latestFrame != null)
          {
            _latestFrame.Dispose();
            _latestFrame = null;
          }
          _latestFrame = value;
        }
      }
    }

    public void Start()
    {
      if (!IsPublishing)
      {
        Stream.VideoSourceException += new VideoSourceExceptionEventHandler(OnVideoSourceException);
        Stream.NewFrame += new NewFrameEventHandler(OnNewFrame);
        Stream.Start();
      }
    }

    public void Stop()
    {
      if (IsPublishing)
      {
        Stream.NewFrame -= new NewFrameEventHandler(OnNewFrame);
        Stream.VideoSourceException -= new VideoSourceExceptionEventHandler(OnVideoSourceException);
        Stream.SignalToStop();
      }
    }

    private void OnNewFrame(object sender, NewFrameEventArgs e)
    {
      try
      {
        Bitmap bitmap = new Bitmap(e.Frame.Width, e.Frame.Height, PixelFormat.Format16bppRgb555);
        Graphics draw = Graphics.FromImage(bitmap);
        draw.DrawImage(e.Frame, 0, 0);

        // when new frame arrived, update the latest frame
        LatestFrame = bitmap;
      }
      catch { }

      if (!_isSending)
      {
        lock (_sendLock)
        {
          try
          {
            _isSending = true;

            lock (_destLock)
            {
              if (Sender != null)
              {
                foreach (var dest in Destinations)
                {
                  _sequenceNumber = _sequenceNumber + 10; // give space to packet sender to split the packet
                  Sender.Send(new StreamPacket(LatestFrame, _sequenceNumber, e.Timestamp, dest));
                }
              }
            }
          }
          finally
          {
            _isSending = false;
          }
        }
      }
    }

    private void OnVideoSourceException(object sender, VideoSourceExceptionEventArgs e)
    {
      RaiseCameraErrorOccurred(this.Id, e.Description);
    }

    public event EventHandler<CameraErrorOccurredEventArgs> CameraErrorOccurred;

    private void RaiseCameraErrorOccurred(string cameraId, string message)
    {
      if (CameraErrorOccurred != null)
      {
        CameraErrorOccurred(this, new CameraErrorOccurredEventArgs(cameraId, message));
      }
    }

    #region IDisposable Members

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!this._disposed)
      {
        if (disposing)
        {
          lock (_latestFrameLock)
          {
            if (_latestFrame != null)
            {
              _latestFrame.Dispose();
              _latestFrame = null;
            }
          }

          lock (_sendLock)
          {
            if (Sender != null)
            {
              Sender.Stop();
              Sender.Dispose();
              Sender = null;
            }
          }
        }

        _disposed = true;
      }
    }

    #endregion
  }
}
