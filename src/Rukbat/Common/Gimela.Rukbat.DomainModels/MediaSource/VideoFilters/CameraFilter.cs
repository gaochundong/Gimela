using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Gimela.Rukbat.DomainModels.MediaSource.VideoFilters
{
  [Serializable]
  [DataContract]
  public class CameraFilter
  {
    [DataMember]
    [XmlAttribute]
    public string Name { get; set; }

    [DataMember]
    [XmlText]
    public string Uri { get; set; }

    public CameraFilter(string name, string uri)
    {
      this.Name = name;
      this.Uri = uri;
    }

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, @"{0} | {1}", this.Name, this.Uri);
    }

    public override bool Equals(object obj)
    {
      bool result = false;

      if (obj != null && obj is CameraFilter)
      {
        CameraFilter two = obj as CameraFilter;
        if (this.Name == two.Name && this.Uri == two.Uri)
        {
          result = true;
        }
      }

      return result;
    }

    public override int GetHashCode()
    {
      return this.Name.GetHashCode() ^ this.Uri.GetHashCode();
    }
  }
}
