using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Rukbat.DVC.BusinessEntities
{
  public class FilterRemovedEventArgs : EventArgs
  {
    public FilterRemovedEventArgs(FilterType filterType, string filterId)
    {
      FilterType = filterType;
      FilterId = filterId;
    }

    public FilterType FilterType { get; private set; }
    public string FilterId { get; private set; }
  }
}
