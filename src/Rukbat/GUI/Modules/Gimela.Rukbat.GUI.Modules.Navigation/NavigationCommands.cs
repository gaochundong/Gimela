using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Presentation.Controls;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;

namespace Gimela.Rukbat.GUI.Modules.Navigation
{
  public static class NavigationCommands
  {
    // Device
    public static RelayCommand ManageCameraCommand { get; private set; }

    // LiveVideo
    public static RelayCommand ShowLiveVideoCameraListCommand { get; private set; }

    // Publish
    public static RelayCommand PublishMediaCommand { get; private set; }

    // Layout
    public static RelayCommand ChangeLayoutCascadeCommand { get; private set; }
    public static RelayCommand ChangeLayoutTileHorizontalCommand { get; private set; }
    public static RelayCommand ChangeLayoutTileVerticalCommand { get; private set; }

    // Options
    public static RelayCommand UpdateSkinCommand { get; private set; }
    public static RelayCommand ChangeLanguage_zh_CN_Command { get; private set; }
    public static RelayCommand ChangeLanguage_en_US_Command { get; private set; }

    // Help
    public static RelayCommand HelpOnlineCommand { get; private set; }
    public static RelayCommand CheckUpdateCommand { get; private set; }
    public static RelayCommand ReportBugCommand { get; private set; }
    public static RelayCommand SeeAboutCommand { get; private set; }

    static NavigationCommands()
    {
      #region Device

      ManageCameraCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.Navigation_ManageCameraEvent));
      });

      #endregion

      #region LiveVideo

      ShowLiveVideoCameraListCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.Navigation_ShowLiveVideoCameraListEvent));
      });

      #endregion

      #region Publish

      PublishMediaCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.Navigation_PublishMediaEvent));
      });

      #endregion

      #region Layout

      ChangeLayoutCascadeCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage<MDILayoutType>(
          UIMessageType.Navigation_ChangeLayoutCascadeEvent, MDILayoutType.Cascade));
      });

      ChangeLayoutTileHorizontalCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage<MDILayoutType>(
          UIMessageType.Navigation_ChangeLayoutTileHorizontalEvent, MDILayoutType.TileHorizontal));
      });

      ChangeLayoutTileVerticalCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage<MDILayoutType>(
          UIMessageType.Navigation_ChangeLayoutTileVerticalEvent, MDILayoutType.TileVertical));
      });

      #endregion

      #region Options

      UpdateSkinCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.Navigation_UpdateSkinEvent));
      });

      ChangeLanguage_zh_CN_Command = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.Navigation_ChangeLanguage_zh_CN_Event));
      });

      ChangeLanguage_en_US_Command = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.Navigation_ChangeLanguage_en_US_Event));
      });

      #endregion

      #region Help

      HelpOnlineCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.Navigation_HelpOnlineEvent));
      });
      CheckUpdateCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.Navigation_CheckUpdateEvent));
      });
      ReportBugCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.Navigation_ReportBugEvent));
      });
      SeeAboutCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.Navigation_SeeAboutEvent));
      });

      #endregion
    }
  }
}
