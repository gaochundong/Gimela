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
  public class PublishServiceSelectionViewModel : ViewModelResponsive
  {
    #region Ctors

    public PublishServiceSelectionViewModel(PublishServiceModel model, Camera camera)
    {
      if (model == null)
        throw new ArgumentNullException("model");
      if (camera == null)
        throw new ArgumentNullException("camera");

      Model = model;
      SelectedCamera = camera;

      ServiceCollection = new ObservableCollection<MediaService>();
      Status = ViewModelStatus.Initializing;        
      Model.GetServices(GetServicesCallback);
    }

    public override void Cleanup()
    {
      base.Cleanup();

      ServiceCollection.Clear();
      Model = null;
      SelectedCamera = null;
    }

    #endregion

    #region Model

    public PublishServiceModel Model { get; private set; }

    #endregion

    #region Properties

    public Camera SelectedCamera { get; private set; }

    public ObservableCollection<MediaService> ServiceCollection { get; set; }

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

    #endregion

    #region Commands

    protected override void BindCommands()
    {
      RefreshServiceCommand = new RelayCommand(() =>
      {
        ServiceCollection.Clear();
        Status = ViewModelStatus.Initializing;
        Model.GetServices(GetServicesCallback);
      });

      SelectServiceCommand = new RelayCommand(() =>
      {
        if (SelectedService == null)
        {
          Messenger.Default.Send(new ViewModelMessageBoxMessage(this, LanguageString.Find("PublishMedia_PublishServiceSelectionView_SelectedServiceNull"), ViewModelMessageBoxType.Error));
          return;
        }

        Messenger.Default.Send(new NotificationMessage<PublishPair>(UIMessageType.PublishMedia_ServiceSelectedEvent, new PublishPair(SelectedService, SelectedCamera)));
      });

      CancelCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.PublishMedia_CancelSelectServiceEvent));
      });
    }

    protected override void UnbindCommands()
    {
      RefreshServiceCommand = null;
      SelectServiceCommand = null;
      CancelCommand = null;
    }

    protected override void SubscribeMessages()
    {

    }

    protected override void UnsubscribeMessages()
    {

    }

    public RelayCommand RefreshServiceCommand { get; private set; }

    public RelayCommand SelectServiceCommand { get; private set; }

    public RelayCommand CancelCommand { get; private set; }

    #endregion

    #region Private Methods

    private void GetServicesCallback(object sender, AsyncWorkerCallbackEventArgs<IList<MediaService>> e)
    {
      bool result = CheckAsyncWorkerCallback<IList<MediaService>>(sender, e, true, LanguageString.Find("PublishMedia_PublishServiceSelectionView_GetServicesFailed"));

      Status = ViewModelStatus.Loaded;

      ServiceCollection.Clear();
      if (result)
      {
        foreach (var item in e.Data)
        {
          ServiceCollection.Add(item);
        }
      }
    }

    #endregion
  }
}
