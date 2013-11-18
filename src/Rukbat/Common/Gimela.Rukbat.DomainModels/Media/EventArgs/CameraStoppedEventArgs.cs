using System;

namespace Gimela.Rukbat.DomainModels
{
  public class CameraStoppedEventArgs : EventArgs
  {
    public string CameraId { get; set; }

    public CameraStoppedEventArgs()
    {
    }

    public CameraStoppedEventArgs(string cameraId)
      : this()
    {
      this.CameraId = cameraId;
    }
  }
}
