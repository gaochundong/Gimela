using System.ServiceModel;
using Gimela.Rukbat.DVC.Contracts.DataContracts;

namespace Gimela.Rukbat.DVC.Contracts.MessageContracts
{
  [MessageContract]
  public class PingCameraResponse
  {
    [MessageBodyMember]
    public CameraStatusData Status { get; set; }
  }
}
