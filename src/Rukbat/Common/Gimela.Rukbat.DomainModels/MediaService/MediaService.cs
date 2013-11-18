using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Gimela.Rukbat.DomainModels
{
  [Serializable]
  [DataContract]
  public class MediaService : BaseObject
  {
    [DataMember]
    [XmlAttribute]
    public string ContractName { get; set; }

    [DataMember]
    [XmlAttribute]
    public string HostName { get; set; }

    [DataMember]
    [XmlAttribute]
    public Uri Uri { get; set; }

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, @"Id=[{0}], Name=[{1}], ContractName=[{2}], HostName=[{3}], Uri=[{4}]",
          Id, Name, ContractName, HostName, Uri);
    }
  }
}
