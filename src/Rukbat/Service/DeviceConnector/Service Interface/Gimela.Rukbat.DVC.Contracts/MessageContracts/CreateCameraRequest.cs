using System.ServiceModel;
using Gimela.Rukbat.DVC.Contracts.DataContracts;

namespace Gimela.Rukbat.DVC.Contracts.MessageContracts
{
  [MessageContract]
  public class CreateCameraRequest
  {
    [MessageBodyMember]
    public CameraData Camera { get; set; }
  }
}
