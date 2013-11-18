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
using Gimela.Rukbat.DomainModels.MediaSource;
using Gimela.Rukbat.DomainModels.MediaSource.VideoFilters;
using Gimela.Rukbat.GUI.Modules.PublishMedia.Entities;
using Gimela.Rukbat.GUI.Modules.PublishMedia.Models;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Rukbat.GUI.ValidationRules.Enumerations;

namespace Gimela.Rukbat.GUI.Modules.PublishMedia.ViewModels
{
  public class PublishedCameraConfigurationViewModel : ViewModelResponsive
  {
    #region Ctors

    public PublishedCameraConfigurationViewModel(PublishedCameraModel model, MediaService service, Camera camera)
    {
      if (model == null)
        throw new ArgumentNullException("model");
      if (service == null)
        throw new ArgumentNullException("service");
      if (camera == null)
        throw new ArgumentNullException("camera");

      Model = model;
      SelectedService = service;
      SelectedCamera = camera;
      DestinationPort = 9999;
    }

    public override void Cleanup()
    {
      base.Cleanup();

      Model = null;
      SelectedService = null;
      SelectedCamera = null;
    }

    #endregion

    #region Model

    public PublishedCameraModel Model { get; private set; }

    #endregion

    #region Properties

    private MediaService _selectedService;
    public MediaService SelectedService
    {
      get
      {
        return _selectedService;
      }
      set
      {
        if (_selectedService == value) return;

        _selectedService = value;
        RaisePropertyChanged("SelectedService");
      }
    }

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

    private int _destinationPort;
    public int DestinationPort
    {
      get
      {
        return _destinationPort;
      }
      set
      {
        if (_destinationPort == value) return;

        _destinationPort = value;
        RaisePropertyChanged("DestinationPort");
      }
    }

    private CheckPortAvailableResultResultType _checkPortAvailableResult = CheckPortAvailableResultResultType.UnsetValue;
    public CheckPortAvailableResultResultType CheckPortAvailableResult
    {
      get
      {
        return _checkPortAvailableResult;
      }
      set
      {
        _checkPortAvailableResult = value;
        RaisePropertyChanged(@"CheckPortAvailableResult");
      }
    }

    #endregion

    #region Commands

    protected override void BindCommands()
    {
      OKCommand = new RelayCommand(() =>
      {
        PublishedCameraProfile profile = new PublishedCameraProfile(SelectedCamera.Id, SelectedCamera.Name)
        {
          CameraThumbnail = SelectedCamera.Thumbnail,
          DeviceServiceHostName = SelectedCamera.HostName,
          DeviceServiceUri = SelectedCamera.HostUri,
        };
        PublishedDestination destination = new PublishedDestination(DestinationPort);

        Status = ViewModelStatus.Saving;
        Model.PublishCamera(SelectedService.Uri.ToString(), profile, destination, PublishCameraCallback);
      });

      CancelCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.PublishMedia_CancelConfigCameraEvent));
      });

      CheckPortAvailableCommand = new RelayCommand(() =>
      {
        Status = ViewModelStatus.Loading;
        Model.CheckPortAvailable(SelectedService.Uri.ToString(), DestinationPort, CheckPortAvailableCallback);
      });
    }

    protected override void UnbindCommands()
    {
      OKCommand = null;
      CancelCommand = null;
    }

    protected override void SubscribeMessages()
    {
    }

    protected override void UnsubscribeMessages()
    {

    }

    public RelayCommand OKCommand { get; protected set; }

    public RelayCommand CancelCommand { get; protected set; }

    public RelayCommand CheckPortAvailableCommand { get; protected set; }

    #endregion

    #region Callbacks

    private void PublishCameraCallback(object sender, AsyncWorkerCallbackEventArgs<bool> args)
    {
      bool result = CheckAsyncWorkerCallback<bool>(sender, args, true, LanguageString.Find("PublishMedia_PublishedCameraConfigurationView_PublishCameraFailed"));

      Status = ViewModelStatus.Saved;

      Messenger.Default.Send(new NotificationMessage(UIMessageType.PublishMedia_CameraPublishedEvent));
    }

    private void CheckPortAvailableCallback(object sender, AsyncWorkerCallbackEventArgs<bool> e)
    {
      bool result = CheckAsyncWorkerCallback<bool>(sender, e, true, LanguageString.Find("PublishMedia_PublishedCameraConfigurationView_CheckPortAvailableError"));

      Status = ViewModelStatus.Loaded;

      if (result)
      {
        CheckPortAvailableResult = (bool)(e.Data) ? CheckPortAvailableResultResultType.Available : CheckPortAvailableResultResultType.Unavailable;
      }
    }

    #endregion
  }
}
