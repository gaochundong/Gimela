using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Media.Video;
using Gimela.Rukbat.DVC.BusinessEntities;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public class Camera : ICamera, ICameraStream
  {
    private CameraProfile _profile;
    private CameraConfig _config;
    private IVideoSource _videoSource;

    public Camera(CameraProfile profile, CameraConfig config, IVideoSource videoSource)
    {
      if (profile == null)
        throw new ArgumentNullException("profile");
      if (config == null)
        throw new ArgumentNullException("config");
      if (videoSource == null)
        throw new ArgumentNullException("videoSource");

      _profile = profile;
      _config = config;
      _videoSource = videoSource;
    }

    #region ICamera Members

    public string Id { get { return _profile.Id; } }

    #endregion

    #region ICameraStream Members

    public IVideoSource Stream { get { return _videoSource; } }

    #endregion

    #region Properties
    
    public CameraProfile Profile { get { return _profile; } }

    public CameraConfig Config { get { return _config; } }

    public byte[] Thumbnail { get; set; }

    #endregion
  }
}
