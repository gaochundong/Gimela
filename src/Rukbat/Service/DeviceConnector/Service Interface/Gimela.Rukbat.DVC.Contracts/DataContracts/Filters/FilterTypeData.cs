using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.Contracts.DataContracts
{
  [DataContract]
  public enum FilterTypeData
  {
    [EnumMember(Value = "None")]
    None = 0,

    [EnumMember(Value = "LocalCamera")]
    LocalCamera = 1,

    [EnumMember(Value = "LocalAVIFile")]
    LocalAVIFile = 2,

    [EnumMember(Value = "LocalDesktop")]
    LocalDesktop = 3,

    [EnumMember(Value = "NetworkJPEG")]
    NetworkJPEG = 4,

    [EnumMember(Value = "NetworkMJPEG")]
    NetworkMJPEG = 5,
  }
}
