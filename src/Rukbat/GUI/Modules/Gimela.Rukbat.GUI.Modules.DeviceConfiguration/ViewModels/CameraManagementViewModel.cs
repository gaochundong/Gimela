using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;
using Gimela.Crust.Tectosphere;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Common.Cultures;
using Gimela.Common.Logging;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Models;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Rukbat.GUI.ValidationRules.Enumerations;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration.ViewModels
{
  public class CameraManagementViewModel : ViewModelResponsive
  {
    #region Ctors

    public CameraManagementViewModel(CameraModel model)
    {
      if (model == null)
        throw new ArgumentNullException("model");

      CameraCollection = new ObservableCollection<Camera>();

      Model = model;

      Refresh();
    }

    public override void Cleanup()
    {
      base.Cleanup();
    }

    #endregion

    #region Model

    public CameraModel Model { get; private set; }

    #endregion

    #region Properties

    public ObservableCollection<Camera> CameraCollection { get; private set; }

    private Camera _selectedCamera;
    public Camera SelectedCamera
    {
      get
      {
        return _selectedCamera;
      }
      set
      {
        if (_selectedCamera == value) return;

        _selectedCamera = value;
        RaisePropertyChanged("SelectedCamera");
      }
    }

    #endregion

    #region Commands

    protected override void BindCommands()
    {
      CreateCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.DeviceConfiguration_CreateCameraEvent));
      });

      UpdateCommand = new RelayCommand(() =>
      {
        if (SelectedCamera == null)
        {
          Messenger.Default.Send(new ViewModelMessageBoxMessage(this, LanguageString.Find("DeviceConfiguration_CameraManagementView_SelectTargetEditCamera"), ViewModelMessageBoxType.Error));
          return;
        }

        Messenger.Default.Send(new NotificationMessage<Camera>(UIMessageType.DeviceConfiguration_UpdateCameraEvent, SelectedCamera));
      });

      DeleteCommand = new RelayCommand(() =>
      {
        if (SelectedCamera == null)
        {
          Messenger.Default.Send(new ViewModelMessageBoxMessage(this, LanguageString.Find("DeviceConfiguration_CameraManagementView_SelectTargetDeleteCamera"), ViewModelMessageBoxType.Error));
          return;
        }

        Status = ViewModelStatus.Saving;
        Model.DeleteCamera(SelectedCamera, DeleteCameraCallback);
      });

      CancelCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.DeviceConfiguration_SelectServiceEvent));
      });

      RefreshCommand = new RelayCommand(() =>
      {
        Refresh();
      });
    }

    protected override void UnbindCommands()
    {
      CreateCommand = null;
      UpdateCommand = null;
      DeleteCommand = null;
      CancelCommand = null; 
      RefreshCommand = null;
    }

    protected override void SubscribeMessages()
    {
      Messenger.Default.Register<NotificationMessage<Camera>>(this, message =>
      {
        if (message.Notification == UIMessageType.DeviceConfiguration_CameraCreatedEvent)
        {
        }
        else if (message.Notification == UIMessageType.DeviceConfiguration_CameraUpdatedEvent)
        {
        }
      });
    }

    protected override void UnsubscribeMessages()
    {
      Messenger.Default.Unregister<NotificationMessage<Camera>>(this);
    }

    public RelayCommand CreateCommand { get; private set; }

    public RelayCommand UpdateCommand { get; private set; }

    public RelayCommand DeleteCommand { get; private set; }

    public RelayCommand CancelCommand { get; private set; }

    public RelayCommand RefreshCommand { get; private set; }

    #endregion

    #region Overrides

    public override void Refresh()
    {
      base.Refresh();

      Status = ViewModelStatus.Initializing;
      CameraCollection.Clear();
      Model.GetCameras(GetCamerasCallback);
    }

    #endregion

    #region Private Methods

    private void GetCamerasCallback(object sender, AsyncWorkerCallbackEventArgs<IList<Camera>> args)
    {
      bool result = CheckAsyncWorkerCallback<IList<Camera>>(sender, args, true, LanguageString.Find("DeviceConfiguration_CameraManagementView_GetCamerasFailed"));

      CameraCollection.Clear();
      Status = ViewModelStatus.Loaded;

      if (result)
      {
        foreach (var item in (args.Data as IList<Camera>))
        {
          CameraCollection.Add(item);
        }
      }
    }

    private void DeleteCameraCallback(object sender, AsyncWorkerCallbackEventArgs<bool> args)
    {
      bool result = CheckAsyncWorkerCallback<bool>(sender, args, true, LanguageString.Find("DeviceConfiguration_CameraManagementView_DeleteCameraFailed"));

      Status = ViewModelStatus.Saved;
      Refresh();
    }

    #endregion
  }
}
