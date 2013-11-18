using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;
using Gimela.Crust.Tectosphere;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Common.Cultures;
using Gimela.Common.Logging;
using Gimela.Rukbat.DomainModels.MediaSource.VideoFilters;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Models;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Rukbat.GUI.ValidationRules.Enumerations;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration.ViewModels
{
  public class VideoSourceNavigationViewModel : ViewModelResponsive
  {
    #region Ctors

    public VideoSourceNavigationViewModel(CameraFilterModel cameraFilterModel, DesktopFilterModel desktopFilterModel)
    {
      if (cameraFilterModel == null)
        throw new ArgumentNullException("cameraFilterModel");
      if (desktopFilterModel == null)
        throw new ArgumentNullException("desktopFilterModel");

      CameraFilterModel = cameraFilterModel;
      DesktopFilterModel = desktopFilterModel;
    }

    #endregion

    #region Model

    public CameraFilterModel CameraFilterModel { get; private set; }

    public DesktopFilterModel DesktopFilterModel { get; private set; }

    #endregion

    #region Commands

    protected override void BindCommands()
    {
      SelectLocalCameraVideoSourceCommand = new RelayCommand(() =>
      {
        Status = ViewModelStatus.Initializing;
        CameraFilterModel.GetCameraFilters(GetCameraFiltersCallback);
      });
      SelectLocalAVIFileVideoSourceCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.DeviceConfiguration_SelectLocalAVIFileVideoSourceEvent));
      });
      SelectLocalDesktopVideoSourceCommand = new RelayCommand(() =>
      {
        Status = ViewModelStatus.Initializing;
        DesktopFilterModel.GetDesktopFilters(GetDesktopFiltersCallback);
      });
      SelectNetworkJPEGCameraVideoSourceCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.DeviceConfiguration_SelectNetworkJPEGVideoSourceEvent));
      });
      SelectNetworkMJPEGCameraVideoSourceCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.DeviceConfiguration_SelectNetworkMJPEGVideoSourceEvent));
      });

      CancelCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.DeviceConfiguration_CancelNavigateVideoSourceEvent));
      });
    }

    protected override void UnbindCommands()
    {
      SelectLocalCameraVideoSourceCommand = null;
      SelectLocalAVIFileVideoSourceCommand = null;
      SelectLocalDesktopVideoSourceCommand = null;
      SelectNetworkJPEGCameraVideoSourceCommand = null;
      SelectNetworkMJPEGCameraVideoSourceCommand = null;
      CancelCommand = null;
    }

    protected override void SubscribeMessages()
    {

    }

    protected override void UnsubscribeMessages()
    {

    }

    public RelayCommand SelectLocalCameraVideoSourceCommand { get; private set; }

    public RelayCommand SelectLocalAVIFileVideoSourceCommand { get; private set; }

    public RelayCommand SelectLocalDesktopVideoSourceCommand { get; private set; }

    public RelayCommand SelectNetworkJPEGCameraVideoSourceCommand { get; private set; }

    public RelayCommand SelectNetworkMJPEGCameraVideoSourceCommand { get; private set; }

    public RelayCommand CancelCommand { get; private set; }

    #endregion

    #region Video Filter Callback

    private void GetCameraFiltersCallback(object sender, AsyncWorkerCallbackEventArgs<IList<CameraFilter>> e)
    {
      bool result = CheckAsyncWorkerCallback<IList<CameraFilter>>(sender, e, true, LanguageString.Find("DeviceConfiguration_VideoSourceNavigationView_GetLocalCameraFailed"));

      Status = ViewModelStatus.Loaded;

      if (result)
      {
        Messenger.Default.Send(new NotificationMessage<IList<CameraFilter>>(UIMessageType.DeviceConfiguration_SelectLocalCameraVideoSourceEvent, e.Data));
      }
    }

    private void GetDesktopFiltersCallback(object sender, AsyncWorkerCallbackEventArgs<IList<DesktopFilter>> e)
    {
      bool result = CheckAsyncWorkerCallback<IList<DesktopFilter>>(sender, e, true, LanguageString.Find("DeviceConfiguration_VideoSourceNavigationView_GetLocalDesktopFailed"));

      Status = ViewModelStatus.Loaded;

      if (result)
      {
        Messenger.Default.Send(new NotificationMessage<IList<DesktopFilter>>(UIMessageType.DeviceConfiguration_SelectLocalDesktopVideoSourceEvent, e.Data));
      }
    }

    #endregion
  }
}
