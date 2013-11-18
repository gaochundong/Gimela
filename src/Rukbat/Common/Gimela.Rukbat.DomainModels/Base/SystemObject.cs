using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Gimela.Rukbat.DomainModels
{
  [Serializable]
  [DataContract]
  public abstract class SystemObject : BaseObject, ISystemObject
  {
    public SystemObject()
    {
    }

    [IgnoreDataMember]
    [XmlIgnoreAttribute]
    public virtual ImageSource ObjectImage { get; set; }

    public abstract void RefreshObjectImage();
  }
}
