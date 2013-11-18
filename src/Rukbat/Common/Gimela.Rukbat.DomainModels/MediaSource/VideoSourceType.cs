using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Gimela.Rukbat.DomainModels.MediaSource
{
  [DataContract(Name = "VideoSourceType")]
  public enum VideoSourceType
  {
    [EnumMember(Value = "Mock")]
    [XmlEnum(Name = "Mock")]
    Mock = 0,

    [EnumMember(Value = "LocalCamera")]
    [XmlEnum(Name = "LocalCamera")]
    LocalCamera = 1,

    [EnumMember(Value = "LocalAVIFile")]
    [XmlEnum(Name = "LocalAVIFile")]
    LocalAVIFile = 2,

    [EnumMember(Value = "LocalDesktop")]
    [XmlEnum(Name = "LocalDesktop")]
    LocalDesktop = 3,

    [EnumMember(Value = "NetworkJPEG")]
    [XmlEnum(Name = "NetworkJPEG")]
    NetworkJPEG = 4,

    [EnumMember(Value = "NetworkMJPEG")]
    [XmlEnum(Name = "NetworkMJPEG")]
    NetworkMJPEG = 5,

    [EnumMember(Value = "NetworkRtpStream")]
    [XmlEnum(Name = "NetworkRtpStream")]
    NetworkRtpStream = 6,
  }
}
