using System.Collections.Generic;
using System.Windows.Controls;

namespace Gimela.Presentation.Controls
{
  internal class MDIChildComparer : IComparer<MDIChild>
  {
    public int Compare(MDIChild x, MDIChild y)
    {
      return -1 * Canvas.GetZIndex(x).CompareTo(Canvas.GetZIndex(y));
    }
  }
}
