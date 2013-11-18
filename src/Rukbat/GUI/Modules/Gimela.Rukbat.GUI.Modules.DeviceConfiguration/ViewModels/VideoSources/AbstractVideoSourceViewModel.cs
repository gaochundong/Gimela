using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Crust.Tectosphere;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;
using Gimela.Rukbat.DomainModels.MediaSource;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration.ViewModels
{
  public abstract class AbstractVideoSourceViewModel : ViewModelResponsive
  {
    #region Ctors

    protected AbstractVideoSourceViewModel()
      : base()
    {
    }

    #endregion

    #region Properties

    public VideoSourceDescription VideoSourceDescription { get; set; }

    #endregion

    #region Bindings

    protected override void BindCommands()
    {
      CancelCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.DeviceConfiguration_CancelUpdateVideoSourceEvent));
      });

      OKCommand = new RelayCommand(CreateVideoSource);
    }

    protected override void UnbindCommands()
    {
      CancelCommand = null;
      OKCommand = null;
    }

    protected override void SubscribeMessages()
    {

    }

    protected override void UnsubscribeMessages()
    {

    }

    public RelayCommand CancelCommand { get; protected set; }

    public RelayCommand OKCommand { get; protected set; }

    #endregion

    #region Methods

    protected abstract void MakeVideoSourceDescription();

    protected virtual void CreateVideoSource()
    {
      MakeVideoSourceDescription();

      Messenger.Default.Send(new NotificationMessage<VideoSourceDescription>(
          UIMessageType.DeviceConfiguration_VideoSourceCreateSelectedEvent, this.VideoSourceDescription));
    }

    protected virtual void UpdateVideoSource()
    {
      MakeVideoSourceDescription();

      Messenger.Default.Send(new NotificationMessage<VideoSourceDescription>(
          UIMessageType.DeviceConfiguration_VideoSourceUpdateSelectedEvent, this.VideoSourceDescription));
    }

    public virtual void SetObject(VideoSourceDescription videoSourceDescription)
    {
      VideoSourceDescription = videoSourceDescription;

      if (VideoSourceDescription != null)
      {
        OKCommand = new RelayCommand(UpdateVideoSource);
      }
      else
      {
        OKCommand = new RelayCommand(CreateVideoSource);
      }
    }

    #endregion
  }
}
