using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Rukbat.DVC.BusinessEntities;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public interface IFilterManager : IDisposable
  {
    event EventHandler<FilterRemovedEventArgs> FilterRemoved;

    bool IsFilterExist(FilterType filterType, string filterId);
    IList<CameraFilter> GetCameraFilters();
    IList<DesktopFilter> GetDesktopFilters();
  }
}
