using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Gimela.Rukbat.DomainModels
{
  [Serializable]
  [DataContract]
  [KnownType(typeof(DeviceStatus))]
  public abstract class MediaObject : SystemObject, IMediaObject
  {
    public MediaObject()
    {
    }

    #region IMediaObject Members

    [DataMember]
    [XmlAttribute]
    public string HostName { get; set; }

    [DataMember]
    [XmlAttribute]
    public string HostUri { get; set; }

    [DataMember]
    [XmlAttribute]
    public string Description { get; set; }

    [DataMember]
    [XmlAttribute]
    public string Tags { get; set; }

    [IgnoreDataMember]
    [XmlIgnore]
    public DeviceStatus Status { get; private set; }

    #endregion

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, @"ID={0}, Name={1}, HostName={2}, Status={3}",
          this.Id, this.Name, this.HostName, this.Status);
    }
  }
}
