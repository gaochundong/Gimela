using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Rukbat.DVC.DataAccess.Models
{
  [Serializable]
  public class CameraConfig
  {
    public string FriendlyName { get; set; }

    public string OriginalSourceString { get; set; }

    public string SourceString { get; set; }

    public int FrameInterval { get; set; }

    public int FrameRate { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public Resolution Resolution { get; set; }

    public string UserAgent { get; set; }
  }
}
