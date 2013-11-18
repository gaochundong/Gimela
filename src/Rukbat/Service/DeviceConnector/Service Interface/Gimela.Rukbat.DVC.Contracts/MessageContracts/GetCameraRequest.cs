using System.ServiceModel;

namespace Gimela.Rukbat.DVC.Contracts.MessageContracts
{
  [MessageContract]
  public class GetCameraRequest
  {
    [MessageBodyMember]
    public string CameraId { get; set; }
  }
}
