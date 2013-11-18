using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Gimela.Rukbat.DVC.BusinessEntities
{
  public class CameraFilter : IFilter
  {
    public CameraFilter(string uri)
    {
      Uri = uri;
    }

    public string Id { get { return this.Uri; } }

    public FilterType Type { get { return FilterType.LocalCamera; } }

    public string Name { get; set; }

    public string Uri { get; private set; }

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, @"{0} | {1}", this.Name, this.Uri);
    }

    public override bool Equals(object obj)
    {
      bool result = false;

      if (obj != null)
      {
        CameraFilter second = obj as CameraFilter;
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
