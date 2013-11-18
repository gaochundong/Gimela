using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.Contracts.DataContracts
{
  [DataContract]
  public class PublishDestinationData
  {
    [DataMember]
    public string Address { get; set; }

    [DataMember]
    public int Port { get; set; }
  }
}
