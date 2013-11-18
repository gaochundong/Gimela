using System;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.IO;

namespace Gimela.Rukbat.GUI.Modules.PublishMedia.Entities
{
  [Serializable]
  public class PublishedCameraProfile
  {
    private BitmapImage _thumbnailSource = null;

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

    public BitmapSource CameraThumbnailSource
    {
      get
      {
        if (_thumbnailSource == null)
        {
          if (CameraThumbnail != null && CameraThumbnail.Length > 0)
          {
            using (MemoryStream stream = new MemoryStream(CameraThumbnail))
            {
              _thumbnailSource = new BitmapImage();
              _thumbnailSource.BeginInit();
              _thumbnailSource.StreamSource = stream;
              _thumbnailSource.CacheOption = BitmapCacheOption.OnLoad;
              _thumbnailSource.EndInit();
              _thumbnailSource.Freeze();
            }
          }
        }

        return _thumbnailSource;
      }
    }

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
