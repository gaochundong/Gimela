using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;
using Gimela.Crust.Tectosphere;
using Gimela.Common.Cultures;
using Gimela.Common.Logging;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.DomainModels.MediaSource;
using Gimela.Rukbat.DomainModels.MediaSource.VideoFilters;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Models;
using Gimela.Rukbat.GUI.Modules.UIMessage;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration.ViewModels
{
  public class LocalDesktopVideoSourceViewModel : AbstractVideoSourceViewModel
  {
    #region Ctors

    public LocalDesktopVideoSourceViewModel()
    {
      VideoFilterCollection = new ObservableCollection<DesktopFilter>();

      FrameInterval = 200;
      FrameWidth = 640;
      FrameHeight = 480;
    }

    public override void Cleanup()
    {
      base.Cleanup();

      VideoFilterCollection = null;
    }

    #endregion

    #region Properties

    public ObservableCollection<DesktopFilter> VideoFilterCollection { get; set; }

    public DesktopFilter SelectedVideoFilter { get; set; }

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

    private int _frameWidth;
    public int FrameWidth
    {
      get
      {
        return _frameWidth;
      }
      set
      {
        _frameWidth = value;
        RaisePropertyChanged("FrameWidth");
      }
    }

    private int _frameHeight;
    public int FrameHeight
    {
      get
      {
        return _frameHeight;
      }
      set
      {
        _frameHeight = value;
        RaisePropertyChanged("FrameHeight");
      }
    }

    #endregion

    #region Methods

    protected override void MakeVideoSourceDescription()
    {
      if (SelectedVideoFilter == null) return;

      this.VideoSourceDescription = new VideoSourceDescription()
      {
        SourceType = VideoSourceType.LocalDesktop,
        FriendlyName = SelectedVideoFilter.DeviceName,
        SourceString = SelectedVideoFilter.DeviceIndex.ToString(),
        OriginalSourceString = SelectedVideoFilter.DeviceIndex.ToString(),
        FrameInterval = this.FrameInterval,
        Resolution = new Resolution(this.FrameWidth, this.FrameHeight)
      };
    }

    public override void SetObject(VideoSourceDescription videoSourceDescription)
    {
      base.SetObject(videoSourceDescription);

      if (VideoSourceDescription != null)
      {
        foreach (var item in VideoFilterCollection)
        {
          if (item.DeviceIndex.ToString() == VideoSourceDescription.OriginalSourceString)
          {
            SelectedVideoFilter = item;
            FrameInterval = VideoSourceDescription.FrameInterval;
            FrameWidth = VideoSourceDescription.Resolution.Width;
            FrameHeight = VideoSourceDescription.Resolution.Height;
            break;
          }
        }
      }
    }

    #endregion
  }
}
