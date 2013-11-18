using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.MPS.Contracts.DataContracts
{
  [DataContract]
  public class PublishedCameraProfileData
  {
    [DataMember(Order = 1)]
    public string CameraId { get; set; }

    [DataMember(Order = 2)]
    public string CameraName { get; set; }

    [DataMember(Order = 3)]
    public byte[] CameraThumbnail { get; set; }

    [DataMember(Order = 4)]
    public string DeviceServiceHostName { get; set; }

    [DataMember(Order = 5)]
    public string DeviceServiceUri { get; set; }
  }
}
