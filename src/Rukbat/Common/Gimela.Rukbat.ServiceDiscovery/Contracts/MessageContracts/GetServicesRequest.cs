using System.ServiceModel;
using Gimela.Rukbat.ServiceDiscovery.Contracts.DataContracts;

namespace Gimela.Rukbat.ServiceDiscovery.Contracts.MessageContracts
{
  [MessageContract]
  public class GetServicesRequest
  {
    [MessageBodyMember]
    public string ServiceName { get; set; }
  }
}
