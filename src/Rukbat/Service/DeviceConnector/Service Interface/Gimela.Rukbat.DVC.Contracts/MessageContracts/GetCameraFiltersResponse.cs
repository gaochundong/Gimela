using System.ServiceModel;
using Gimela.Rukbat.DVC.Contracts.DataContracts;

namespace Gimela.Rukbat.DVC.Contracts.MessageContracts
{
  [MessageContract]
  public class GetCameraFiltersResponse
  {
    [MessageBodyMember(Order = 1)]
    public CameraFilterDataCollection Filters { get; set; }
  }
}
