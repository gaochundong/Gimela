using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Gimela.ServiceModel.ManagedService.Attributes;
using Gimela.Rukbat.ServiceDiscovery.Contracts.FaultContracts;
using Gimela.Rukbat.ServiceDiscovery.Contracts.MessageContracts;

namespace Gimela.Rukbat.ServiceDiscovery.Contracts.ServiceContracts
{
  [ManagedServiceContractAttribute]
  [ServiceContract(SessionMode = SessionMode.Allowed)]
  public interface IServiceDiscoveryService
  {
    [OperationContract]
    [FaultContract(typeof(ServiceDiscoveryServiceFault))]
    GetServicesResponse GetServices(GetServicesRequest request);
  }
}
