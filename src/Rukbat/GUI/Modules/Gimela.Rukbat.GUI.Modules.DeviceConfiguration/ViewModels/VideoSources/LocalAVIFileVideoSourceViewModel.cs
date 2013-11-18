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
using Gimela.Common.ExceptionHandling;
using Gimela.Rukbat.DomainModels;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration.ViewModels
{
  public class LocalAVIFileVideoSourceViewModel : AbstractVideoSourceViewModel
  {
    #region Ctors

    public LocalAVIFileVideoSourceViewModel()
      : base()
    {
    }

    #endregion

    #region Properties

    private string _selectedFilePath;
    public string SelectedFilePath
    {
      get
      {
        return _selectedFilePath;
      }
      set
      {
        if (_selectedFilePath == value) return;

        _selectedFilePath = value;
        RaisePropertyChanged("SelectedFilePath");
      }
    }

    #endregion

    #region Commands

    protected override void BindCommands()
    {
      base.BindCommands();

      SelectFileCommand = new RelayCommand(SelectFile);
    }

    public RelayCommand SelectFileCommand { get; private set; }

    #endregion

    #region Methods

    private void SelectFile()
    {
      try
      {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = LanguageString.Find("DeviceConfiguration_VideoSourceLocalAVIFileView_AVIFileFilter");

        if (!string.IsNullOrEmpty(SelectedFilePath))
        {
          FileInfo info = new FileInfo(SelectedFilePath);
          dialog.InitialDirectory = info.DirectoryName;
        }
        else
        {
          dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos);
        }

        if (dialog.ShowDialog() == DialogResult.OK)
        {
          SelectedFilePath = dialog.FileName;
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    protected override void MakeVideoSourceDescription()
    {
      FileInfo info = new FileInfo(this.SelectedFilePath);

      this.VideoSourceDescription = new VideoSourceDescription()
      {
        SourceType = VideoSourceType.LocalAVIFile,
        FriendlyName = info.Name,
        SourceString = info.FullName,
        OriginalSourceString = info.FullName
      };
    }

    public override void SetObject(VideoSourceDescription videoSourceDescription)
    {
      base.SetObject(videoSourceDescription);

      if (VideoSourceDescription != null)
      {
        SelectedFilePath = videoSourceDescription.OriginalSourceString;
      }
    }

    #endregion
  }
}
