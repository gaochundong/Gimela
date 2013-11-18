using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.Contracts.DataContracts
{
  [DataContract]
  public class ResolutionData
  {
    [DataMember]
    public int Height { get; set; }

    [DataMember]
    public int Width { get; set; }
  }
}
