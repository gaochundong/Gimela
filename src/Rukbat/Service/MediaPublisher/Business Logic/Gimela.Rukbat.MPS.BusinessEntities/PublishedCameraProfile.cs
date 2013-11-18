using System.Globalization;

namespace Gimela.Rukbat.MPS.BusinessEntities
{
  public class PublishedCameraProfile
  {
    public PublishedCameraProfile()
    {
    }

    public PublishedCameraProfile(string cameraId, string cameraName)
      : this()
    {
      CameraId = cameraId;
      CameraName = cameraName;
    }

    public string CameraId { get; set; }

    public string CameraName { get; set; }

    public byte[] CameraThumbnail { get; set; }

    public string DeviceServiceHostName { get; set; }

    public string DeviceServiceUri { get; set; }

    public override bool Equals(object obj)
    {
      bool result = false;

      if (obj != null)
      {
        PublishedCameraProfile other = obj as PublishedCameraProfile;
        if (other != null && this.CameraId == other.CameraId)
        {
          result = true;
        }
      }

      return result;
    }

    public override int GetHashCode()
    {
      return this.CameraId.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, "CameraId[{0}], CameraName[{1}]", CameraId, CameraName);
    }
  }
}
