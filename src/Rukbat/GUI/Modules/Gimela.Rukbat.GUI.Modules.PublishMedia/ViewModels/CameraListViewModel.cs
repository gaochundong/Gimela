using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Media.Imaging;
using Gimela.Crust;
using Gimela.Crust.Tectosphere;
using Gimela.Common.Cultures;
using Gimela.Common.Logging;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Infrastructure.Messaging;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.DomainModels.MediaSource;
using Gimela.Rukbat.DomainModels.MediaSource.VideoFilters;
using Gimela.Rukbat.GUI.Modules.PublishMedia.Models;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Rukbat.GUI.ValidationRules.Enumerations;

namespace Gimela.Rukbat.GUI.Modules.PublishMedia.ViewModels
{
  public class CameraListViewModel : ViewModelResponsive
  {
    #region Ctors

    public CameraListViewModel(CameraModel model)
    {
      if (model == null)
        throw new ArgumentNullException("model");

      CameraCollection = new ObservableCollection<Camera>();
      Model = model;

      Refresh(); ;
    }

    public override void Cleanup()
    {
      base.Cleanup();
      CameraCollection.Clear();
      Model = null;
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

    private string _searchCameraText;
    public string SearchCameraText
    {
      get
      {
        return _searchCameraText;
      }
      set
      {
        _searchCameraText = value;
        RaisePropertyChanged("SearchCameraText");

        if (string.IsNullOrEmpty(_searchCameraText))
        {
          Refresh();
        }
      }
    }

    private BitmapSource _selectedCameraThumbnail;
    public BitmapSource SelectedCameraThumbnail
    {
      get
      {
        return _selectedCameraThumbnail;
      }
      set
      {
        if (_selectedCameraThumbnail == value) return;

        _selectedCameraThumbnail = value;
        RaisePropertyChanged("SelectedCameraThumbnail");
      }
    }

    #endregion

    #region Commands

    protected override void BindCommands()
    {
      SearchCameraCommand = new RelayCommand(() =>
      {
        Status = ViewModelStatus.Loading;
        CameraCollection.Clear();
        Model.SearchCameras(SearchCameraText, SearchCamerasCallback);
      });

      RefreshCameraCommand = new RelayCommand(() =>
      {
        SearchCameraText = string.Empty;
        Refresh();
      });

      PublishCameraCommand = new RelayCommand(() =>
      {
        if (SelectedCamera == null)
        {
          Messenger.Default.Send(new ViewModelMessageBoxMessage(this, LanguageString.Find("PublishMedia_CameraListView_PublishCameraNull"), ViewModelMessageBoxType.Error));
          return;
        }

        Messenger.Default.Send(new NotificationMessage<Camera>(UIMessageType.PublishMedia_CameraSelectedEvent, SelectedCamera));
      });

      CancelCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.PublishMedia_CancelSelectCameraEvent));
      });
    }

    protected override void UnbindCommands()
    {
      SearchCameraCommand = null;
      RefreshCameraCommand = null;
      PublishCameraCommand = null;
      CancelCommand = null;
    }

    protected override void SubscribeMessages()
    {

    }

    protected override void UnsubscribeMessages()
    {

    }

    public RelayCommand SearchCameraCommand { get; private set; }

    public RelayCommand RefreshCameraCommand { get; private set; }

    public RelayCommand PublishCameraCommand { get; private set; }

    public RelayCommand CancelCommand { get; private set; }

    #endregion

    #region Overrides

    public override void Refresh()
    {
      base.Refresh();

      CameraCollection.Clear();
      Status = ViewModelStatus.Initializing;
      Model.GetCameras(GetCamerasCallback);
    }

    #endregion

    #region Private Methods

    private void GetCamerasCallback(object sender, AsyncWorkerCallbackEventArgs<IList<Camera>> args)
    {
      bool result = CheckAsyncWorkerCallback<IList<Camera>>(sender, args, true, LanguageString.Find("PublishMedia_CameraListView_GetCamerasError"));

      CameraCollection.Clear();

      if (result)
      {
        foreach (var item in (args.Data as IList<Camera>))
        {
          CameraCollection.Add(item);
        }
      }

      Status = ViewModelStatus.Loaded;
    }

    private void SearchCamerasCallback(object sender, AsyncWorkerCallbackEventArgs<IList<Camera>> args)
    {
      bool result = CheckAsyncWorkerCallback<IList<Camera>>(sender, args, true, LanguageString.Find("PublishMedia_CameraListView_SearchError"));

      CameraCollection.Clear();

      if (result)
      {
        foreach (var item in (args.Data as IList<Camera>))
        {
          CameraCollection.Add(item);
        }
      }

      Status = ViewModelStatus.Loaded;
    }

    #endregion
  }
}
