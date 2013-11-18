using System.ServiceModel;
using Gimela.Rukbat.DVC.Contracts.DataContracts;

namespace Gimela.Rukbat.DVC.Contracts.MessageContracts
{
  [MessageContract]
  public class GetPublishedCamerasResponse
  {
    [MessageBodyMember]
    public PublishedCameraDataCollection PublishedCameras { get; set; }
  }
}
