using System;
using System.IO;
using System.Windows.Forms;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;
using Gimela.Crust.Tectosphere;
using Gimela.Common.Cultures;
using Gimela.Common.Logging;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using System.Collections.ObjectModel;
using Gimela.Rukbat.DomainModels.MediaSource.VideoFilters;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.DomainModels.MediaSource;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Models;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration.ViewModels
{
  public class LocalCameraVideoSourceViewModel : AbstractVideoSourceViewModel
  {
    #region Ctors

    public LocalCameraVideoSourceViewModel()
    {
      VideoFilterCollection = new ObservableCollection<CameraFilter>();

      FrameRate = 10;
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

    public ObservableCollection<CameraFilter> VideoFilterCollection { get; set; }

    private CameraFilter _selectedVideoFilter;
    public CameraFilter SelectedVideoFilter
    {
      get
      {
        return _selectedVideoFilter;
      }
      set
      {
        if (_selectedVideoFilter == value) return;

        _selectedVideoFilter = value;
        RaisePropertyChanged("SelectedVideoFilter");
      }
    }

    private int _frameRate;
    public int FrameRate
    {
      get
      {
        return _frameRate;
      }
      set
      {
        if (_frameRate == value) return;

        _frameRate = value;
        RaisePropertyChanged("FrameRate");
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
        if (_frameWidth == value) return;

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
        if (_frameHeight == value) return;

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
        SourceType = VideoSourceType.LocalCamera,
        FriendlyName = SelectedVideoFilter.Name,
        SourceString = SelectedVideoFilter.Uri,
        OriginalSourceString = SelectedVideoFilter.Uri,
        FrameRate = this.FrameRate,
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
          if (item.Uri == VideoSourceDescription.OriginalSourceString)
          {
            SelectedVideoFilter = item;
            FrameRate = VideoSourceDescription.FrameRate;
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
