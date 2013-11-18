using System.Collections.Generic;
using System.ServiceModel;
using Gimela.Rukbat.ServiceDiscovery.Contracts.DataContracts;

namespace Gimela.Rukbat.ServiceDiscovery.Contracts.MessageContracts
{
  [MessageContract]
  public class GetServicesResponse
  {
    [MessageBodyMember]
    public List<ServiceData> Services { get; set; }
  }
}
