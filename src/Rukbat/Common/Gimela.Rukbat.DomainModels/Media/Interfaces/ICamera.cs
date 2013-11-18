using System;
using System.Windows.Media.Imaging;

namespace Gimela.Rukbat.DomainModels
{
  public interface ICamera : IMediaObject, IFrame, ICameraDecorator, ICloneable
  {
    BitmapSource ThumbnailSource { get; }
    byte[] Thumbnail { get; set; }

    void Start();
    void Stop();

    event EventHandler<CameraAlarmEventArgs> CameraAlarmEvent;
    event EventHandler<CameraStartedEventArgs> CameraStartedEvent;
    event EventHandler<CameraStoppedEventArgs> CameraStoppedEvent;
  }
}
