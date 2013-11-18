using System;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.ServiceDiscovery.Contracts.DataContracts
{
  [DataContract]
  public class ServiceData
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string ContractName { get; set; }

    [DataMember]
    public string Binding { get; set; }

    [DataMember]
    public string Address { get; set; }

    [DataMember]
    public string HostName { get; set; }

    [DataMember]
    public Uri Uri { get; set; }
  }
}
