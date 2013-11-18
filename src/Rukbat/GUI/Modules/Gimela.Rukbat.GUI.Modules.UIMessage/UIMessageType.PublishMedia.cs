using System;

namespace Gimela.Rukbat.GUI.Modules.UIMessage
{
  public partial class UIMessageType
  {
    public static readonly string PublishMedia_CreatePublishedCameraEvent = Guid.NewGuid().ToString();

    public static readonly string PublishMedia_CameraSelectedEvent = Guid.NewGuid().ToString();
    public static readonly string PublishMedia_ServiceSelectedEvent = Guid.NewGuid().ToString();

    public static readonly string PublishMedia_CancelSelectCameraEvent = Guid.NewGuid().ToString();
    public static readonly string PublishMedia_CancelSelectServiceEvent = Guid.NewGuid().ToString();    
    public static readonly string PublishMedia_CancelConfigCameraEvent = Guid.NewGuid().ToString();

    public static readonly string PublishMedia_CameraPublishedEvent = Guid.NewGuid().ToString();    
  }
}
