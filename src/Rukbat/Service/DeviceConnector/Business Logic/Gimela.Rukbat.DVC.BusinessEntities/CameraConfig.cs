using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Media.Video;

namespace Gimela.Rukbat.DVC.BusinessEntities
{
  public class CameraConfig
  {
    public CameraConfig()
    {
    }

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
