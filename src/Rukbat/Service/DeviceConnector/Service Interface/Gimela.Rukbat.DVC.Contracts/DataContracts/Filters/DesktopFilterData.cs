using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Gimela.Rukbat.DVC.Contracts.DataContracts
{
  [DataContract]
  public class DesktopFilterData
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int Index { get; set; }

    [DataMember]
    public Rectangle Bounds { get; set; }

    [DataMember]
    public bool IsPrimary { get; set; }
  }
}
