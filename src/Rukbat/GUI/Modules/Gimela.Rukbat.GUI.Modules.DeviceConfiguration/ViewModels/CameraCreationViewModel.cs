using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Gimela.Common.Cultures;
using Gimela.Common.Logging;
using Gimela.Crust;
using Gimela.Crust.Tectosphere;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Infrastructure.Messaging;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.DomainModels.MediaSource;
using Gimela.Rukbat.DomainModels.MediaSource.VideoFilters;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Models;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Rukbat.GUI.ValidationRules.Enumerations;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration.ViewModels
{
  public class CameraCreationViewModel : ViewModelResponsive
  {
    #region Fields

    private AutoResetEvent _syncWork = new AutoResetEvent(false);

    #endregion

    #region Ctors

    public CameraCreationViewModel(CameraModel model, CameraFilterModel cameraFilterModel, DesktopFilterModel desktopFilterModel)
    {
      if (model == null)
        throw new ArgumentNullException("model");
      if (cameraFilterModel == null)
        throw new ArgumentNullException("cameraFilterModel");
      if (desktopFilterModel == null)
        throw new ArgumentNullException("desktopFilterModel");

      Model = model;
      CameraFilterModel = cameraFilterModel;
      DesktopFilterModel = desktopFilterModel;
    }

    public override void Cleanup()
    {
      base.Cleanup();

      Model = null;
      CameraFilterModel = null;
      DesktopFilterModel = null;
    }

    #endregion

    #region Model

    public CameraModel Model { get; private set; }

    public CameraFilterModel CameraFilterModel { get; private set; }

    public DesktopFilterModel DesktopFilterModel { get; private set; }

    #endregion

    #region Properties

    private VideoSourceDescription _selectedVideoSourceDescription;
    public VideoSourceDescription SelectedVideoSourceDescription
    {
      get
      {
        return _selectedVideoSourceDescription;
      }
      set
      {
        if (_selectedVideoSourceDescription == value) return;

        _selectedVideoSourceDescription = value;
        RaisePropertyChanged("SelectedVideoSourceDescription");
      }
    }

    private string _selectedVideoSourceName = LanguageString.Find("DeviceConfiguration_CameraCreationView_NoSelectSource");
    public string SelectedVideoSourceName
    {
      get
      {
        return _selectedVideoSourceName;
      }
      set
      {
        _selectedVideoSourceName = value;
        RaisePropertyChanged(@"SelectedVideoSourceName");
      }
    }

    private string _cameraName = string.Empty;
    public string CameraName
    {
      get
      {
        return _cameraName;
      }
      set
      {
        _cameraName = value;
        RaisePropertyChanged(@"CameraName");

        CheckCameraNameResult = CheckNameExistedResultType.UnsetValue;
      }
    }

    private CheckNameExistedResultType _checkCameraNameResult = CheckNameExistedResultType.UnsetValue;
    public CheckNameExistedResultType CheckCameraNameResult
    {
      get
      {
        return _checkCameraNameResult;
      }
      set
      {
        _checkCameraNameResult = value;
        RaisePropertyChanged(@"CheckCameraNameResult");
      }
    }

    private string _cameraDescription = string.Empty;
    public string CameraDescription
    {
      get
      {
        return _cameraDescription;
      }
      set
      {
        _cameraDescription = value;
        RaisePropertyChanged(@"CameraDescription");
      }
    }

    private string _cameraTags = string.Empty;
    public string CameraTags
    {
      get
      {
        return _cameraTags;
      }
      set
      {
        _cameraTags = value;
        RaisePropertyChanged(@"CameraTags");
      }
    }

    #endregion

    #region Bindings

    protected override void BindCommands()
    {
      OKCommand = new RelayCommand(() =>
      {
        Camera camera = MakeCamera();
        if (camera != null)
        {
          Status = ViewModelStatus.Loading;
          Model.CheckCameraName(camera, CheckCameraNameCallback);

          // 异步顺序执行两个操作
          AsyncWorkerHandle<bool> handle = AsyncWorkerHelper.DoWork<bool>(
              delegate(object sender, DoWorkEventArgs e)
              {
                _syncWork.WaitOne(); // 等待上一个操作的完成

                if (CheckCameraNameResult == CheckNameExistedResultType.NotExisted)
                {
                  Status = ViewModelStatus.Saving;
                  Model.CreateCamera(camera, CreateCameraCallback);
                }
              });
        }
      });

      CancelCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.DeviceConfiguration_CancelCameraEvent));
      });

      CheckCameraNameCommand = new RelayCommand(() =>
      {
        Camera camera = MakeCheckedCamera();
        if (camera != null)
        {
          Status = ViewModelStatus.Loading;
          Model.CheckCameraName(camera, CheckCameraNameCallback);
        }
      });

      #region SelectVideoSourceCommand

      SelectVideoSourceCommand = new RelayCommand(() =>
      {
        if (this.SelectedVideoSourceDescription == null || string.IsNullOrEmpty(this.SelectedVideoSourceDescription.OriginalSourceString))
        {
          Messenger.Default.Send(new NotificationMessage<VideoSourceDescription>(
            UIMessageType.DeviceConfiguration_NavigateVideoSourceEvent, null));
        }
        else
        {
          switch (this.SelectedVideoSourceDescription.SourceType)
          {
            case VideoSourceType.Mock:
              break;
            case VideoSourceType.LocalCamera:
              Status = ViewModelStatus.Initializing;
              CameraFilterModel.GetCameraFilters(GetCameraFiltersCallback);
              break;
            case VideoSourceType.LocalAVIFile:
              Messenger.Default.Send(new NotificationMessage<VideoSourceDescription>(
                UIMessageType.DeviceConfiguration_UpdateLocalAVIFileVideoSourceEvent, this.SelectedVideoSourceDescription));
              break;
            case VideoSourceType.LocalDesktop:
              Status = ViewModelStatus.Initializing;
              DesktopFilterModel.GetDesktopFilters(GetDesktopFiltersCallback);
              break;
            case VideoSourceType.NetworkJPEG:
              Messenger.Default.Send(new NotificationMessage<VideoSourceDescription>(
                UIMessageType.DeviceConfiguration_UpdateNetworkJPEGVideoSourceEvent, this.SelectedVideoSourceDescription));
              break;
            case VideoSourceType.NetworkMJPEG:
              Messenger.Default.Send(new NotificationMessage<VideoSourceDescription>(
                UIMessageType.DeviceConfiguration_UpdateNetworkMJPEGVideoSourceEvent, this.SelectedVideoSourceDescription));
              break;
            default:
              throw new NotSupportedException();
          }
        }
      });

      #endregion
    }

    protected override void UnbindCommands()
    {
      OKCommand = null;
      CancelCommand = null;
      CheckCameraNameCommand = null;
      SelectVideoSourceCommand = null;
    }

    protected override void SubscribeMessages()
    {
      Messenger.Default.Register<NotificationMessage<VideoSourceDescription>>(this, message =>
      {
        if (message.Notification == UIMessageType.DeviceConfiguration_VideoSourceCreateSelectedEvent)
        {
          SelectedVideoSourceDescription = message.Content;
          if (SelectedVideoSourceDescription == null) return;

          SelectedVideoSourceName = SelectedVideoSourceDescription.FriendlyName;
          CameraDescription = string.Format("{0}, {1}", SelectedVideoSourceDescription.FriendlyName, SelectedVideoSourceDescription.OriginalSourceString);
        }
        else if (message.Notification == UIMessageType.DeviceConfiguration_VideoSourceUpdateSelectedEvent)
        {
          SelectedVideoSourceDescription = message.Content;
          if (SelectedVideoSourceDescription == null) return;

          SelectedVideoSourceName = SelectedVideoSourceDescription.FriendlyName;
          CameraDescription = string.Format("{0}, {1}", SelectedVideoSourceDescription.FriendlyName, SelectedVideoSourceDescription.OriginalSourceString);
        }
      });
    }

    protected override void UnsubscribeMessages()
    {
      Messenger.Default.Unregister<NotificationMessage<VideoSourceDescription>>(this);
    }

    public RelayCommand OKCommand { get; protected set; }

    public RelayCommand CancelCommand { get; protected set; }

    public RelayCommand CheckCameraNameCommand { get; protected set; }

    public RelayCommand SelectVideoSourceCommand { get; protected set; }

    #endregion

    #region Methods

    private void CreateCameraCallback(object sender, AsyncWorkerCallbackEventArgs<Camera> e)
    {
      bool result = CheckAsyncWorkerCallback<Camera>(sender, e, true, LanguageString.Find("DeviceConfiguration_CameraCreationView_CreateCameraFailed"));

      Status = ViewModelStatus.Saved;

      if (result)
      {
        Messenger.Default.Send(new NotificationMessage<Camera>(
          UIMessageType.DeviceConfiguration_CameraCreatedEvent, e.Data as Camera));
      }
    }

    private void CheckCameraNameCallback(object sender, AsyncWorkerCallbackEventArgs<bool> e)
    {
      bool result = CheckAsyncWorkerCallback<bool>(sender, e, true, LanguageString.Find("DeviceConfiguration_CameraCreationView_CheckCameraNameError"));

      Status = ViewModelStatus.Loaded;

      if (result)
      {
        CheckCameraNameResult = (bool)(e.Data) ? CheckNameExistedResultType.NotExisted : CheckNameExistedResultType.IsExisted;
      }

      _syncWork.Set(); // 操作已完成
    }

    protected virtual Camera MakeCamera()
    {
      if (SelectedVideoSourceDescription == null)
      {
        Messenger.Default.Send(new ViewModelMessageBoxMessage(
          this, LanguageString.Find("DeviceConfiguration_CameraCreationView_PleaseSelectCameraSource"), ViewModelMessageBoxType.Error));
        return null;
      }
      if (string.IsNullOrEmpty(CameraName))
      {
        Messenger.Default.Send(new ViewModelMessageBoxMessage(
          this, LanguageString.Find("DeviceConfiguration_CameraCreationView_CameraNameNull"), ViewModelMessageBoxType.Error));
        return null;
      }

      Camera camera = CameraFactory.CreateCamera();

      camera.Name = CameraName;
      camera.Description = CameraDescription;
      camera.VideoSourceDescription = SelectedVideoSourceDescription;
      camera.Tags = CameraTags;

      return camera;
    }

    private Camera MakeCheckedCamera()
    {
      if (string.IsNullOrEmpty(CameraName))
      {
        Messenger.Default.Send(new ViewModelMessageBoxMessage(
          this, LanguageString.Find("DeviceConfiguration_CameraCreationView_CameraNameNull"), ViewModelMessageBoxType.Error));
        return null;
      }

      Camera camera = CameraFactory.CreateCamera();
      camera.Name = CameraName;

      return camera;
    }

    #endregion

    #region Video Filter Callback

    private void GetCameraFiltersCallback(object sender, AsyncWorkerCallbackEventArgs<IList<CameraFilter>> e)
    {
      bool result = CheckAsyncWorkerCallback<IList<CameraFilter>>(sender, e, true, LanguageString.Find("DeviceConfiguration_CameraCreationView_GetLocalCameraFailed"));

      Status = ViewModelStatus.Loaded;

      if (result)
      {
        Messenger.Default.Send(new MultipleContentNotificationMessage<IList<CameraFilter>, VideoSourceDescription>(
          UIMessageType.DeviceConfiguration_UpdateLocalCameraVideoSourceEvent)
          {
            FirstContent = e.Data, 
            SecondContent = this.SelectedVideoSourceDescription, 
          });
      }
    }

    private void GetDesktopFiltersCallback(object sender, AsyncWorkerCallbackEventArgs<IList<DesktopFilter>> e)
    {
      bool result = CheckAsyncWorkerCallback<IList<DesktopFilter>>(sender, e, true, LanguageString.Find("DeviceConfiguration_CameraCreationView_GetLocalDesktopFailed"));

      Status = ViewModelStatus.Loaded;

      if (result)
      {
        Messenger.Default.Send(new MultipleContentNotificationMessage<IList<DesktopFilter>, VideoSourceDescription>(
          UIMessageType.DeviceConfiguration_UpdateLocalDesktopVideoSourceEvent)
          {
            FirstContent = e.Data,
            SecondContent = this.SelectedVideoSourceDescription,
          });
      }
    }

    #endregion
  }
}
