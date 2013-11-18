using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Infrastructure.Patterns;

namespace Gimela.Rukbat.GUI.Modules.LiveVideo
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
