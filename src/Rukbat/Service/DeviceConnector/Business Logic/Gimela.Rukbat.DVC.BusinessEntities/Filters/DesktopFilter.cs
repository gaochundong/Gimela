using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Gimela.Rukbat.DVC.BusinessEntities
{
  public class DesktopFilter : IFilter
  {
    public DesktopFilter(int index)
    {
      Index = index;
    }

    public string Id { get { return this.Index.ToString(CultureInfo.InvariantCulture); } }

    public FilterType Type { get { return FilterType.LocalDesktop; } }

    public string Name { get; set; }

    public int Index { get; private set; }

    public Rectangle Bounds { get; set; }

    public bool IsPrimary { get; set; }

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, @"{0} | {1} | {2}", this.Index, this.Name, this.Bounds);
    }

    public override bool Equals(object obj)
    {
      bool result = false;

      if (obj != null)
      {
        DesktopFilter second = obj as DesktopFilter;
        if (second != null && this.Id == second.Id)
        {
          result = true;
        }
      }

      return result;
    }

    public override int GetHashCode()
    {
      return this.Id.GetHashCode();
    }
  }
}
