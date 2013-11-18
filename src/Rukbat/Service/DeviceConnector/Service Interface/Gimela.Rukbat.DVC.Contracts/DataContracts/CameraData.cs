using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.Contracts.DataContracts
{
  [DataContract]
  public class CameraData
  {
    [DataMember]
    public CameraProfileData Profile { get; set; }

    [DataMember]
    public CameraConfigData Config { get; set; }

    [DataMember]
    public byte[] Thumbnail { get; set; }
  }
}
