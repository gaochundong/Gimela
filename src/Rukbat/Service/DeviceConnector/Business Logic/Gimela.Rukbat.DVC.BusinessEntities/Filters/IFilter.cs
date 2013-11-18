using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Media.Video;

namespace Gimela.Rukbat.DVC.BusinessEntities
{
  public interface IFilter
  {
    string Id { get; }

    FilterType Type { get; }

    string Name { get; set; }
  }
}
