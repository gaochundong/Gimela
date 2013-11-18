using System.Globalization;

namespace Gimela.Rukbat.DVC.BusinessEntities
{
  public class CameraPublishDestination
  {
    public CameraPublishDestination(string cameraId, PublishDestination destination)
    {
      CameraId = cameraId;
      Destination = destination;
    }

    public string Id { get { return string.Format(CultureInfo.InvariantCulture, "{0}#{1}", CameraId, Destination.ToString()); } }

    public string CameraId { get; private set; }

    public PublishDestination Destination { get; private set; }

    public override bool Equals(object obj)
    {
      bool result = false;

      if (obj != null)
      {
        CameraPublishDestination other = obj as CameraPublishDestination;
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
