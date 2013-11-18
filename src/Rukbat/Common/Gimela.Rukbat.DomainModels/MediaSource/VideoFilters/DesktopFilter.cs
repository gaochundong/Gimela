using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Gimela.Rukbat.DomainModels.MediaSource.VideoFilters
{
  [Serializable]
  [DataContract]
  public class DesktopFilter
  {
    [DataMember]
    [XmlAttribute]
    public string DeviceName { get; set; }

    [DataMember]
    [XmlAttribute]
    public bool Primary { get; set; }

    [DataMember]
    [XmlAttribute]
    public int DeviceIndex { get; set; }

    [DataMember]
    [XmlElement]
    public Rectangle Bounds { get; set; }

    public DesktopFilter(string deviceName, int deviceIndex)
    {
      this.DeviceName = deviceName;
      this.DeviceIndex = deviceIndex;
    }

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, @"{0} | {1}", this.DeviceIndex, this.DeviceName);
    }

    public override bool Equals(object obj)
    {
      bool result = false;

      if (obj != null && obj is DesktopFilter)
      {
        DesktopFilter two = obj as DesktopFilter;
        if (this.DeviceName == two.DeviceName
            && this.Primary == two.Primary
            && this.DeviceIndex == two.DeviceIndex
            && this.Bounds == two.Bounds)
        {
          result = true;
        }
      }

      return result;
    }

    public override int GetHashCode()
    {
      return this.DeviceName.GetHashCode()
        ^ this.Primary.GetHashCode()
        ^ this.DeviceIndex.GetHashCode()
        ^ this.Bounds.GetHashCode();
    }
  }
}
