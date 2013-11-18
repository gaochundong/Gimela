using System.ServiceModel;
using Gimela.Rukbat.DVC.Contracts.DataContracts;

namespace Gimela.Rukbat.DVC.Contracts.MessageContracts
{
  [MessageContract]
  public class UpdateCameraRequest
  {
    [MessageBodyMember]
    public string CameraId { get; set; }

    [MessageBodyMember]
    public string CameraName { get; set; }

    [MessageBodyMember]
    public string Description { get; set; }

    [MessageBodyMember]
    public string Tags { get; set; }
  }
}
