using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Media.Video;

namespace Gimela.Rukbat.DVC.BusinessEntities
{
  public class CameraProfile
  {
    public CameraProfile(string cameraId, FilterType filterType, string filterId)
    {
      Id = cameraId;
      FilterType = filterType;
      FilterId = filterId;
    }

    public string Id { get; private set; }

    public FilterType FilterType { get; private set; }

    public string FilterId { get; private set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Tags { get; set; }
  }
}
