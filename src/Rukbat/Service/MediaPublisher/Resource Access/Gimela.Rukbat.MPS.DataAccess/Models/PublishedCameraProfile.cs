using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Data.Magpie;

namespace Gimela.Rukbat.MPS.DataAccess.Models
{
  [Serializable]
  public class PublishedCameraProfile
  {
    public string CameraId { get; set; }

    public string CameraName { get; set; }

    public byte[] CameraThumbnail { get; set; }

    public string DeviceServiceHostName { get; set; }

    public string DeviceServiceUri { get; set; }
  }
}
