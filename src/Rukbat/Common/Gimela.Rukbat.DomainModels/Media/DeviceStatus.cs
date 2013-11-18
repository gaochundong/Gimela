using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Gimela.Rukbat.DomainModels
{
  [DataContract(Name = "DeviceStatus")]
  public enum DeviceStatus
  {
    [EnumMember(Value = "Online")]
    [XmlEnum(Name = "Online")]
    Online = 0,

    [EnumMember(Value = "Offline")]
    [XmlEnum(Name = "Offline")]
    Offline = 1,
  }
}
