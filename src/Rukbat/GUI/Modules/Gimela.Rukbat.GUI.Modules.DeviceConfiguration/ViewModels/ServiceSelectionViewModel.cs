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
  public class ServiceSelectionViewModel : ViewModelResponsive
  {
    #region Ctors

    public ServiceSelectionViewModel(Models.ServiceModel model)
    {
      if (model == null)
        throw new ArgumentNullException("model");

      Model = model;

      ServiceCollection = new ObservableCollection<MediaService>();
      Status = ViewModelStatus.Initializing;        
      Model.GetServices(GetServicesCallback);
    }

    public override void Cleanup()
    {
      base.Cleanup();

      ServiceCollection.Clear();
      Model = null;
    }

    #endregion

    #region Model

    public Models.ServiceModel Model { get; private set; }

    #endregion

    #region Properties

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

      ConnectServiceCommand = new RelayCommand(() =>
      {
        if (SelectedService == null)
        {
          Messenger.Default.Send(new ViewModelMessageBoxMessage(this, LanguageString.Find("DeviceConfiguration_ServiceSelectionView_PleaseSelectService"), ViewModelMessageBoxType.Error));
          return;
        }

        // navigate to camera list view
        Messenger.Default.Send(new NotificationMessage<MediaService>(UIMessageType.DeviceConfiguration_ServiceSelectedEvent, SelectedService));
      });
    }

    protected override void UnbindCommands()
    {
      RefreshServiceCommand = null;
      ConnectServiceCommand = null;
    }

    protected override void SubscribeMessages()
    {

    }

    protected override void UnsubscribeMessages()
    {

    }

    public RelayCommand RefreshServiceCommand { get; private set; }

    public RelayCommand ConnectServiceCommand { get; private set; }

    #endregion

    #region Private Methods

    private void GetServicesCallback(object sender, AsyncWorkerCallbackEventArgs<IList<MediaService>> e)
    {
      bool result = CheckAsyncWorkerCallback<IList<MediaService>>(sender, e, true, LanguageString.Find("DeviceConfiguration_ServiceSelectionView_GetServicesFailed"));

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
