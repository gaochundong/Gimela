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
  public class NetworkMJPEGVideoSourceViewModel : AbstractVideoSourceViewModel
  {
    #region Ctors

    public NetworkMJPEGVideoSourceViewModel()
      : base()
    {

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

    #endregion

    #region Methods

    protected override void MakeVideoSourceDescription()
    {
      this.VideoSourceDescription = new VideoSourceDescription()
      {
        SourceType = VideoSourceType.NetworkMJPEG,
        FriendlyName = this.URL,
        SourceString = this.URL,
        OriginalSourceString = this.URL,
        UserName = this.UserName,
        Password = this.Password,
        UserAgent = this.UserAgent,
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
      }
    }

    #endregion
  }
}
