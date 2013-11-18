using System.Runtime.Serialization;

namespace Gimela.Rukbat.MPS.Contracts.DataContracts
{
  [DataContract]
  public class PublishedDestinationData
  {
    [DataMember(Order = 1)]
    public int Port { get; set; }
  }
}
