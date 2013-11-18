using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Rukbat.DVC.BusinessEntities;
using Gimela.Rukbat.DVC.Contracts.DataContracts;

namespace Gimela.Rukbat.DVC.ServiceImplementation.Translators
{
  internal static class DesktopFilterTranslator
  {
    internal static DesktopFilterData Translate(DesktopFilter filter)
    {
      DesktopFilterData data = new DesktopFilterData()
      {
        Name = filter.Name,
        Index = filter.Index,
        Bounds = filter.Bounds,
        IsPrimary = filter.IsPrimary
      };

      return data;
    }
  }
}
