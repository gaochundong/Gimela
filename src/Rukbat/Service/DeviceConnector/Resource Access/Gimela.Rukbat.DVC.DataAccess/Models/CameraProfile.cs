using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Rukbat.DVC.DataAccess.Models
{
  [Serializable]
  public class CameraProfile
  {
    public string Id { get; set; }

    public string Name { get; set; }

    public int FilterType { get; set; }

    public string FilterId { get; set; }

    public string Description { get; set; }

    public string Tags { get; set; }
  }
}
