using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using Gimela.ServiceModel.ManagedDiscovery;
using Gimela.Rukbat.ServiceDiscovery.Contracts.DataContracts;
using Gimela.Rukbat.ServiceDiscovery.Contracts.FaultContracts;
using Gimela.Rukbat.ServiceDiscovery.Contracts.MessageContracts;
using Gimela.Rukbat.ServiceDiscovery.Contracts.ServiceContracts;

namespace Gimela.Rukbat.SVD.ServiceImplementation
{
  public partial class ServiceDiscoveryService : IServiceDiscoveryService
  {
    #region IServiceDiscoveryService Members

    public GetServicesResponse GetServices(GetServicesRequest request)
    {
      try
      {
        if (request == null)
          throw new ArgumentNullException("request");

        GetServicesResponse response = new GetServicesResponse();
        response.Services = new List<ServiceData>();

        Type contractType = (from c in contracts
                             where c.ContractType.FullName == request.ServiceName
                             select c.ContractType).FirstOrDefault();
        if (contractType != null)
        {
          IList<ServiceEndpoint> endpoints = EndpointProvider.GetEndpoints(contractType);
          foreach (var endpoint in endpoints)
          {
            ServiceData service = new ServiceData()
            {
              Name = endpoint.Name,
              ContractName = endpoint.Contract.ContractType.FullName,
              Binding = endpoint.Binding.Name,
              Address = endpoint.Address.Uri.ToString(),
              HostName = endpoint.ListenUri.Host,
              Uri = endpoint.ListenUri,
            };
            response.Services.Add(service);
          }
        }

        return response;
      }
      catch (Exception ex)
      {
        throw new FaultException<ServiceDiscoveryServiceFault>(new ServiceDiscoveryServiceFault(ex.Message, ex), ex.Message);
      }
    }

    #endregion
  }
}
