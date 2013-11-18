using System.ServiceModel;
using Gimela.Rukbat.DVC.Contracts.DataContracts;

namespace Gimela.Rukbat.DVC.Contracts.MessageContracts
{
  [MessageContract]
  public class KeepPublishedCameraAliveRequest
  {
    [MessageBodyMember]
    public string CameraId { get; set; }

    [MessageBodyMember]
    public PublishDestinationData Destination { get; set; }
  }
}
