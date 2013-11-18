using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using Gimela.Crust;
using Gimela.Crust.Tectosphere;
using Gimela.Common.Cultures;
using Gimela.Common.Logging;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Infrastructure.Messaging;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.GUI.Modules.PublishMedia.Entities;
using Gimela.Rukbat.GUI.Modules.PublishMedia.Models;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Rukbat.GUI.ValidationRules.Enumerations;

namespace Gimela.Rukbat.GUI.Modules.PublishMedia.ViewModels
{
  public class PublishedCameraManagementViewModel : ViewModelResponsive
  {
    #region Ctors

    public PublishedCameraManagementViewModel(PublishedCameraModel model)
    {
      if (model == null)
        throw new ArgumentNullException("model");

      PublishedCameraCollection = new ObservableCollection<PublishedCamera>();

      Model = model;

      Refresh();
    }

    public override void Cleanup()
    {
      base.Cleanup();

      PublishedCameraCollection.Clear();
      Model = null;
    }

    #endregion

    #region Model

    public PublishedCameraModel Model { get; private set; }

    #endregion

    #region Properties

    public ObservableCollection<PublishedCamera> PublishedCameraCollection { get; private set; }

    private PublishedCamera _selectedPublishedCamera;
    public PublishedCamera SelectedPublishedCamera
    {
      get
      {
        return _selectedPublishedCamera;
      }
      set
      {
        if (_selectedPublishedCamera == value) return;

        _selectedPublishedCamera = value;
        RaisePropertyChanged("SelectedPublishedCamera");
      }
    }

    #endregion

    #region Commands

    protected override void BindCommands()
    {
      CreateCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.PublishMedia_CreatePublishedCameraEvent));
      });

      DeleteCommand = new RelayCommand(() =>
      {
        if (SelectedPublishedCamera == null)
        {
          Messenger.Default.Send(new ViewModelMessageBoxMessage(this, LanguageString.Find("PublishMedia_PublishedCameraManagementView_SelectTargetDeletePublishedCamera"), ViewModelMessageBoxType.Error));
          return;
        }

        Status = ViewModelStatus.Saving;
        Model.UnpublishCamera(SelectedPublishedCamera.PublishServiceUri, SelectedPublishedCamera, UnpublishCameraCallback);
      });

      RefreshCommand = new RelayCommand(() =>
      {
        Refresh();
      });
    }

    protected override void UnbindCommands()
    {
      CreateCommand = null;
      DeleteCommand = null;
      RefreshCommand = null;
    }

    protected override void SubscribeMessages()
    {
    }

    protected override void UnsubscribeMessages()
    {

    }

    public RelayCommand CreateCommand { get; private set; }

    public RelayCommand DeleteCommand { get; private set; }

    public RelayCommand RefreshCommand { get; private set; }

    #endregion

    #region Overrides

    public override void Refresh()
    {
      base.Refresh();

      Status = ViewModelStatus.Initializing;
      PublishedCameraCollection.Clear();
      Model.GetPublishedCameras(GetPublishedCamerasCallback);
    }

    #endregion

    #region Callbacks
    
    private void GetPublishedCamerasCallback(object sender, AsyncWorkerCallbackEventArgs<IList<PublishedCamera>> args)
    {
      bool result = CheckAsyncWorkerCallback<IList<PublishedCamera>>(sender, args, true, LanguageString.Find("PublishMedia_PublishedCameraManagementView_GetPublishedCamerasFailed"));

      PublishedCameraCollection.Clear();

      if (result)
      {
        foreach (var item in (args.Data as IList<PublishedCamera>))
        {
          PublishedCameraCollection.Add(item);
        }
      }

      Status = ViewModelStatus.Loaded;
    }

    private void UnpublishCameraCallback(object sender, AsyncWorkerCallbackEventArgs<bool> args)
    {
      bool result = CheckAsyncWorkerCallback<bool>(sender, args, true, LanguageString.Find("PublishMedia_PublishedCameraManagementView_DeletePublishedCameraFailed"));

      Status = ViewModelStatus.Saved;
      Refresh();
    }

    #endregion
  }
}
