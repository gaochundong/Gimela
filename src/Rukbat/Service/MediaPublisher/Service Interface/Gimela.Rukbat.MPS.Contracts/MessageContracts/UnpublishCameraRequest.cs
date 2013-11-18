using System.ServiceModel;
using Gimela.Rukbat.MPS.Contracts.DataContracts;

namespace Gimela.Rukbat.MPS.Contracts.MessageContracts
{
  [MessageContract]
  public class UnpublishCameraRequest
  {
    [MessageBodyMember]
    public string CameraId { get; set; }

    [MessageBodyMember]
    public PublishedDestinationData Destination { get; set; }
  }
}
