using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Gimela.Rukbat.DomainModels.MediaSource
{
  [Serializable]
  [DataContract]
  [KnownType(typeof(VideoSourceType))]
  public class VideoSourceDescription
  {
    [DataMember(EmitDefaultValue = false)]
    [XmlAttribute("SourceType")]
    public VideoSourceType SourceType { get; set; }

    [DataMember]
    [XmlText]
    public string SourceString { get; set; }

    [DataMember]
    [XmlText]
    public string OriginalSourceString { get; set; }

    [DataMember]
    [XmlAttribute]
    public string FriendlyName { get; set; }

    [DataMember]
    [XmlElement]
    public Resolution Resolution { get; set; }

    [DataMember]
    [XmlAttribute]
    public int FrameRate { get; set; }

    [DataMember]
    [XmlAttribute]
    public int FrameInterval { get; set; }

    [DataMember]
    [XmlAttribute]
    public string UserName { get; set; }

    [DataMember]
    [XmlAttribute]
    public string Password { get; set; }

    [DataMember]
    [XmlAttribute]
    public string UserAgent { get; set; }

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, @"SourceType : {0}, FriendlyName : {1}, SourceString : {2}",
          SourceType.ToString(), FriendlyName, SourceString);
    }

    public override bool Equals(object obj)
    {
      bool result = false;

      if (obj != null && obj is VideoSourceDescription)
      {
        VideoSourceDescription two = obj as VideoSourceDescription;
        if (this.SourceType == two.SourceType
            && this.FriendlyName == two.FriendlyName
            && this.SourceString == two.SourceString
            && this.Resolution == two.Resolution
            && this.FrameRate == two.FrameRate
            && this.FrameInterval == two.FrameInterval
            && this.UserName == two.UserName
            && this.Password == two.Password
            && this.UserAgent == two.UserAgent)
        {
          result = true;
        }
      }

      return result;
    }

    public override int GetHashCode()
    {
      int hash = this.SourceType.GetHashCode();

      if (!string.IsNullOrEmpty(this.FriendlyName))
        hash = hash ^ this.FriendlyName.GetHashCode();
      if (!string.IsNullOrEmpty(this.SourceString))
        hash = hash ^ this.SourceString.GetHashCode();
      if (this.Resolution != null)
        hash = hash ^ this.Resolution.GetHashCode();
      if (this.FrameRate > 0)
        hash = hash ^ this.FrameRate.GetHashCode();
      if (this.FrameInterval > 0)
        hash = hash ^ this.FrameInterval.GetHashCode();
      if (!string.IsNullOrEmpty(this.UserName))
        hash = hash ^ this.UserName.GetHashCode();
      if (!string.IsNullOrEmpty(this.Password))
        hash = hash ^ this.Password.GetHashCode();
      if (!string.IsNullOrEmpty(this.UserAgent))
        hash = hash ^ this.UserAgent.GetHashCode();

      return hash;
    }
  }
}
