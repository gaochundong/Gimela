using System;

namespace Gimela.Rukbat.DomainModels
{
  public class CameraStartedEventArgs : EventArgs
  {
    public string CameraId { get; set; }

    public CameraStartedEventArgs()
    {
    }

    public CameraStartedEventArgs(string cameraId)
      : this()
    {
      this.CameraId = cameraId;
    }
  }
}
