using System.ServiceModel;
using Gimela.Rukbat.DVC.Contracts.DataContracts;

namespace Gimela.Rukbat.DVC.Contracts.CallbackContracts
{
  [MessageContract]
  public class NotifyCameraCreatedRequest
  {
    [MessageBodyMember]
    public CameraData Camera { get; set; }
  }
}
