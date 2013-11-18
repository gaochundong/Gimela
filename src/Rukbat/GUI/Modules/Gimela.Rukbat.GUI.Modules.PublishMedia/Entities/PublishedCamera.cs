using System;
using System.Globalization;

namespace Gimela.Rukbat.GUI.Modules.PublishMedia.Entities
{
  [Serializable]
  public class PublishedCamera
  {
    public PublishedCamera(PublishedCameraProfile profile, PublishedDestination destination)
    {
      if (profile == null)
        throw new ArgumentNullException("profile");
      if (destination == null)
        throw new ArgumentNullException("destination");

      Profile = profile;
      Destination = destination;
    }

    public string Id { get { return string.Format(CultureInfo.InvariantCulture, "{0}#{1}", Profile.CameraId, Destination.Port); } }

    public PublishedCameraProfile Profile { get; private set; }

    public PublishedDestination Destination { get; private set; }

    public string PublishServiceHostName { get; set; }

    public string PublishServiceUri { get; set; }

    public override bool Equals(object obj)
    {
      bool result = false;

      if (obj != null)
      {
        PublishedCamera other = obj as PublishedCamera;
        if (other != null && this.Id == other.Id)
        {
          result = true;
        }
      }

      return result;
    }

    public override int GetHashCode()
    {
      return this.Id.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, "{0}", Id);
    }
  }
}
