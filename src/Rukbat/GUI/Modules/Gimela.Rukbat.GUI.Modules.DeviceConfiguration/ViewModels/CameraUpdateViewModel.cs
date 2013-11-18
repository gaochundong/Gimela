using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;
using Gimela.Crust.Tectosphere;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Common.Logging;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Models;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Rukbat.GUI.ValidationRules.Enumerations;
using Gimela.Common.Cultures;
using Gimela.Rukbat.DomainModels;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration.ViewModels
{
  public class CameraUpdateViewModel : CameraCreationViewModel
  {
    #region Ctors

    public CameraUpdateViewModel(CameraModel model, CameraFilterModel localCameraVSModel, DesktopFilterModel localDesktopVSModel)
      : base(model, localCameraVSModel, localDesktopVSModel)
    {

    }

    #endregion

    #region Properties

    public Camera UpdatedCamera { get; protected set; }

    #endregion

    #region Commands

    protected override void BindCommands()
    {
      base.BindCommands();

      OKCommand = new RelayCommand(() =>
      {
        Camera camera = MakeCamera();
        if (camera != null)
        {
          Status = ViewModelStatus.Saving;
          Model.UpdateCamera(camera, UpdateCameraCallback);
        }
      });
    }

    protected override void UnbindCommands()
    {
      OKCommand = null;
    }

    protected override void SubscribeMessages()
    {

    }

    protected override void UnsubscribeMessages()
    {

    }

    #endregion

    #region Methods

    public void SetObject(object target)
    {
      if (target == null)
        throw new ArgumentNullException("target");

      Camera camera = target as Camera;
      if (camera == null)
        throw new ArgumentNullException("camera");

      UpdatedCamera = camera;

      this.SelectedVideoSourceDescription = camera.VideoSourceDescription;
      this.SelectedVideoSourceName = camera.VideoSourceDescription.FriendlyName;
      this.CameraName = camera.Name;
      this.CameraDescription = camera.Description;
      this.CameraTags = camera.Tags;
    }

    private void UpdateCameraCallback(object sender, AsyncWorkerCallbackEventArgs<Camera> args)
    {
      bool result = CheckAsyncWorkerCallback<Camera>(sender, args, true, LanguageString.Find("DeviceConfiguration_CameraCreationView_UpdateCameraFailed"));

      Status = ViewModelStatus.Saved;

      if (result)
      {
        Messenger.Default.Send(new NotificationMessage<Camera>(UIMessageType.DeviceConfiguration_CameraUpdatedEvent, args.Data as Camera));
      }
    }

    protected override Camera MakeCamera()
    {
      if (SelectedVideoSourceDescription == null)
      {
        Messenger.Default.Send(new ViewModelMessageBoxMessage(this, LanguageString.Find("DeviceConfiguration_CameraCreationView_PleaseSelectCameraSource"), ViewModelMessageBoxType.Error));
        return null;
      }
      if (string.IsNullOrEmpty(CameraName))
      {
        Messenger.Default.Send(new ViewModelMessageBoxMessage(this, LanguageString.Find("DeviceConfiguration_CameraCreationView_CameraNameNull"), ViewModelMessageBoxType.Error));
        return null;
      }

      Camera camera = UpdatedCamera;

      camera.Name = CameraName;
      camera.Description = CameraDescription;
      camera.VideoSourceDescription = SelectedVideoSourceDescription;
      camera.Tags = CameraTags;

      return camera;
    }

    #endregion
  }
}
