using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.Contracts.DataContracts
{
  [DataContract]
  public class CameraFilterData
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Uri { get; set; }
  }
}
