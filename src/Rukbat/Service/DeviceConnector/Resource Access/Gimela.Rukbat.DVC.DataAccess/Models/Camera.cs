using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Data.Magpie;

namespace Gimela.Rukbat.DVC.DataAccess.Models
{
  [Serializable]
  public class Camera : IMagpieDocumentId
  {
    public string Id { get; set; }

    public CameraProfile Profile { get; set; }

    public CameraConfig Config { get; set; }

    public byte[] Thumbnail { get; set; }
  }
}
