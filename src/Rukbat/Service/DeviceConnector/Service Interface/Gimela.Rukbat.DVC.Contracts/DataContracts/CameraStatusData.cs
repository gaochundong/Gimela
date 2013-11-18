using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.Contracts.DataContracts
{
  [DataContract]
  public enum CameraStatusData
  {
    [EnumMember]
    Unknown = 0,

    [EnumMember]
    Online = 1,

    [EnumMember]
    Offline = 2,
  }
}
