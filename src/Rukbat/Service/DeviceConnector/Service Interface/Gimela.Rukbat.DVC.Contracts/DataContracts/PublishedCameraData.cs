using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.Contracts.DataContracts
{
  [DataContract]
  public class PublishedCameraData
  {
    [DataMember]
    public string CameraId { get; set; }

    [DataMember]
    public List<PublishDestinationData> Destinations { get; set; }
  }
}
