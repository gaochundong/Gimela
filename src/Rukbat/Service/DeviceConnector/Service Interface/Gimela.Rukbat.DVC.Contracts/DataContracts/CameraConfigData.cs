using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.Contracts.DataContracts
{
  [DataContract]
  public class CameraConfigData
  {
    [DataMember]
    public string FriendlyName { get; set; }

    [DataMember]
    public string OriginalSourceString { get; set; }

    [DataMember]
    public string SourceString { get; set; }

    [DataMember]
    public int FrameInterval { get; set; }

    [DataMember]
    public int FrameRate { get; set; }

    [DataMember]
    public string UserName { get; set; }

    [DataMember]
    public string Password { get; set; }

    [DataMember]
    public ResolutionData Resolution { get; set; }

    [DataMember]
    public string UserAgent { get; set; }
  }
}
