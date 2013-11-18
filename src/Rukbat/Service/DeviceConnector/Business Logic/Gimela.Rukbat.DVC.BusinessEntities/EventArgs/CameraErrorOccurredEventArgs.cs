using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Rukbat.DVC.BusinessEntities
{
  public class CameraErrorOccurredEventArgs : EventArgs
  {
    public CameraErrorOccurredEventArgs(string cameraId, string message)
    {
      CameraId = cameraId;
      Message = message;
    }

    public string CameraId { get; private set; }
    public string Message { get; private set; }
  }
}
