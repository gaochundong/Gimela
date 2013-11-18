using System;

namespace Gimela.Rukbat.DomainModels
{
  public class CameraAlarmEventArgs : EventArgs
  {
    public string CameraId { get; set; }

    public MediaAlarm Alarm { get; set; }

    public CameraAlarmEventArgs()
    {
    }

    public CameraAlarmEventArgs(string cameraId, MediaAlarm alarm)
      : this()
    {
      this.CameraId = cameraId;
      this.Alarm = alarm;
    }
  }
}
