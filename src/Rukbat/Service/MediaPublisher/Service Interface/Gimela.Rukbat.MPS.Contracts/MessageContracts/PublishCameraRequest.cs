using System.ServiceModel;
using Gimela.Rukbat.MPS.Contracts.DataContracts;

namespace Gimela.Rukbat.MPS.Contracts.MessageContracts
{
  [MessageContract]
  public class PublishCameraRequest
  {
    [MessageBodyMember]
    public PublishedCameraProfileData Profile { get; set; }

    [MessageBodyMember]
    public PublishedDestinationData Destination { get; set; }
  }
}
