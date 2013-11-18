using System;
using System.IO;
using System.Windows.Forms;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;
using Gimela.Crust.Tectosphere;
using Gimela.Common.Cultures;
using Gimela.Common.Logging;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Rukbat.DomainModels.MediaSource;
using Gimela.Rukbat.DomainModels;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration.ViewModels
{
  public class NetworkJPEGVideoSourceViewModel : AbstractVideoSourceViewModel
  {
    #region Ctors

    public NetworkJPEGVideoSourceViewModel()
      : base()
    {
      FrameInterval = 200;
    }

    #endregion

    #region Properties

    private string _userName;
    public string UserName
    {
      get
      {
        return _userName;
      }
      set
      {
        _userName = value;
        RaisePropertyChanged("UserName");
      }
    }

    private string _password;
    public string Password
    {
      get
      {
        return _password;
      }
      set
      {
        _password = value;
        RaisePropertyChanged("Password");
      }
    }

    private string _userAgent;
    public string UserAgent
    {
      get
      {
        return _userAgent;
      }
      set
      {
        _userAgent = value;
        RaisePropertyChanged("UserAgent");
      }
    }

    private string _url;
    public string URL
    {
      get
      {
        return _url;
      }
      set
      {
        _url = value;
        RaisePropertyChanged("URL");
      }
    }

    private int _frameInterval;
    public int FrameInterval
    {
      get
      {
        return _frameInterval;
      }
      set
      {
        _frameInterval = value;
        RaisePropertyChanged("FrameInterval");
      }
    }

    #endregion

    #region Methods

    protected override void MakeVideoSourceDescription()
    {
      this.VideoSourceDescription = new VideoSourceDescription()
      {
        SourceType = VideoSourceType.NetworkJPEG,
        FriendlyName = this.URL,
        SourceString = this.URL,
        OriginalSourceString = this.URL,
        UserName = this.UserName,
        Password = this.Password,
        UserAgent = this.UserAgent,
        FrameInterval = this.FrameInterval,
      };
    }

    public override void SetObject(VideoSourceDescription videoSourceDescription)
    {
      base.SetObject(videoSourceDescription);

      if (VideoSourceDescription != null)
      {
        this.UserName = videoSourceDescription.UserName;
        this.Password = videoSourceDescription.Password;
        this.UserAgent = videoSourceDescription.UserAgent;
        this.URL = videoSourceDescription.OriginalSourceString;
        this.FrameInterval = videoSourceDescription.FrameInterval;
      }
    }

    #endregion
  }
}
