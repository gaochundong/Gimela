using System;

namespace Gimela.Rukbat.GUI.Modules.UIMessage
{
  public partial class UIMessageType
  {
    // Device
    public static readonly string Navigation_ManageCameraEvent = Guid.NewGuid().ToString();

    // LiveVideo
    public static readonly string Navigation_ShowLiveVideoCameraListEvent = Guid.NewGuid().ToString();

    // Publish
    public static readonly string Navigation_PublishMediaEvent = Guid.NewGuid().ToString();

    // Layout
    public static readonly string Navigation_ChangeLayoutCascadeEvent = Guid.NewGuid().ToString();
    public static readonly string Navigation_ChangeLayoutTileHorizontalEvent = Guid.NewGuid().ToString();
    public static readonly string Navigation_ChangeLayoutTileVerticalEvent = Guid.NewGuid().ToString();

    // Options
    public static readonly string Navigation_UpdateSkinEvent = Guid.NewGuid().ToString();
    public static readonly string Navigation_ChangeLanguage_zh_CN_Event = Guid.NewGuid().ToString();
    public static readonly string Navigation_ChangeLanguage_en_US_Event = Guid.NewGuid().ToString();

    // Help
    public static readonly string Navigation_HelpOnlineEvent = Guid.NewGuid().ToString();
    public static readonly string Navigation_CheckUpdateEvent = Guid.NewGuid().ToString();
    public static readonly string Navigation_ReportBugEvent = Guid.NewGuid().ToString();
    public static readonly string Navigation_SeeAboutEvent = Guid.NewGuid().ToString();
  }
}
