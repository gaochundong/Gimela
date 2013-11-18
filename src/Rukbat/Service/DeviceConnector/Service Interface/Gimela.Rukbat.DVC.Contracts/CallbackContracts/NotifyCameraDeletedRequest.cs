using System.ServiceModel;
using Gimela.Rukbat.DVC.Contracts.DataContracts;

namespace Gimela.Rukbat.DVC.Contracts.CallbackContracts
{
  [MessageContract]
  public class NotifyCameraDeletedRequest
  {
    [MessageBodyMember]
    public string CameraId { get; set; }
  }
}
