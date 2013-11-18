using System;

namespace Gimela.Rukbat.GUI.Modules.UIMessage
{
  public partial class UIMessageType
  {
    public static readonly string DeviceConfiguration_SelectServiceEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_ServiceSelectedEvent = Guid.NewGuid().ToString();

    public static readonly string DeviceConfiguration_CreateCameraEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_UpdateCameraEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_CancelCameraEvent = Guid.NewGuid().ToString();

    public static readonly string DeviceConfiguration_NavigateVideoSourceEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_CancelNavigateVideoSourceEvent = Guid.NewGuid().ToString();

    public static readonly string DeviceConfiguration_SelectLocalCameraVideoSourceEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_SelectLocalAVIFileVideoSourceEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_SelectLocalDesktopVideoSourceEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_SelectNetworkJPEGVideoSourceEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_SelectNetworkMJPEGVideoSourceEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_SelectVLCPluginVideoSourceEvent = Guid.NewGuid().ToString();

    public static readonly string DeviceConfiguration_UpdateVideoSourceEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_CancelUpdateVideoSourceEvent = Guid.NewGuid().ToString();

    public static readonly string DeviceConfiguration_UpdateLocalCameraVideoSourceEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_UpdateLocalAVIFileVideoSourceEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_UpdateLocalDesktopVideoSourceEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_UpdateNetworkJPEGVideoSourceEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_UpdateNetworkMJPEGVideoSourceEvent = Guid.NewGuid().ToString();

    public static readonly string DeviceConfiguration_VideoSourceCreateSelectedEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_VideoSourceUpdateSelectedEvent = Guid.NewGuid().ToString();

    public static readonly string DeviceConfiguration_CameraCreatedEvent = Guid.NewGuid().ToString();
    public static readonly string DeviceConfiguration_CameraUpdatedEvent = Guid.NewGuid().ToString();
  }
}
