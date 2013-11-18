using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Rukbat.DVC.BusinessEntities;
using Gimela.Rukbat.DVC.Contracts.DataContracts;

namespace Gimela.Rukbat.DVC.ServiceImplementation.Translators
{
  internal static class CameraFilterTranslator
  {
    internal static CameraFilterData Translate(CameraFilter filter)
    {
      CameraFilterData data = new CameraFilterData();
      data.Name = filter.Name;
      data.Uri = filter.Uri;
      return data;
    }
  }
}
