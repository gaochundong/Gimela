using Gimela.Infrastructure.Patterns;
using Gimela.Rukbat.GUI.Modules.PublishMedia.Models;
using Gimela.Rukbat.GUI.Modules.PublishMedia.ViewModels;
using Gimela.Rukbat.DomainModels;

namespace Gimela.Rukbat.GUI.Modules.PublishMedia
{
  internal static class ViewModelLocator
  {
    public static DeviceConnectorServiceClient ServiceClient
    {
      get
      {
        return Singleton<DeviceConnectorServiceClient>.Instance;
      }
    }
  }
}
