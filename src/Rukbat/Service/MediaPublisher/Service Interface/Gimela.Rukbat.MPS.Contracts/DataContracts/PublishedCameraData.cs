using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.MPS.Contracts.DataContracts
{
  [DataContract]
  public class PublishedCameraData
  {
    [DataMember(Order = 1)]
    public PublishedCameraProfileData Profile { get; set; }

    [DataMember(Order = 2)]
    public PublishedDestinationData Destination { get; set; }
  }
}
