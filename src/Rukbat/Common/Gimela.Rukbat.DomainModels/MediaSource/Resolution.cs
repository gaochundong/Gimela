using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Gimela.Rukbat.DomainModels.MediaSource
{
  [Serializable]
  [DataContract]
  public class Resolution
  {
    public Resolution()
    {
    }

    public Resolution(int width, int height)
    {
      Width = width;
      Height = height;
    }

    [DataMember]
    [XmlAttribute]
    public int Width { get; set; }

    [DataMember]
    [XmlAttribute]
    public int Height { get; set; }

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, @"{0}x{1}", Width.ToString(), Height.ToString());
    }

    public override bool Equals(object obj)
    {
      bool result = false;

      if (obj != null && obj is Resolution)
      {
        Resolution two = obj as Resolution;
        if (this.Width == two.Width
            && this.Height == two.Height)
        {
          result = true;
        }
      }

      return result;
    }

    public override int GetHashCode()
    {
      return this.Width.GetHashCode() ^ this.Height.GetHashCode();
    }
  }
}
