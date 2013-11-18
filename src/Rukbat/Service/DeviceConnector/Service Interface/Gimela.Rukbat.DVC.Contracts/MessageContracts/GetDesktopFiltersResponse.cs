using System.ServiceModel;
using Gimela.Rukbat.DVC.Contracts.DataContracts;

namespace Gimela.Rukbat.DVC.Contracts.MessageContracts
{
  [MessageContract]
  public class GetDesktopFiltersResponse
  {
    [MessageBodyMember(Order = 1)]
    public DesktopFilterDataCollection Filters { get; set; }
  }
}
