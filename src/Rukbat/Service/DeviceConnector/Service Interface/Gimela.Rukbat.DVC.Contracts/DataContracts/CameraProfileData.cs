using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.Contracts.DataContracts
{
  [DataContract]
  public class CameraProfileData
  {
    public CameraProfileData()
    {
      FilterType = FilterTypeData.None;
    }

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public FilterTypeData FilterType { get; set; }

    [DataMember]
    public string FilterId { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Tags { get; set; }
  }
}
