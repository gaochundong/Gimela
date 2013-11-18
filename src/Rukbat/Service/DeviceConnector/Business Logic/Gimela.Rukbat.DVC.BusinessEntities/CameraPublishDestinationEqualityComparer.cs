using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Rukbat.DVC.BusinessEntities
{
  public class CameraPublishDestinationEqualityComparer : IEqualityComparer<CameraPublishDestination>
  {
    public bool Equals(CameraPublishDestination x, CameraPublishDestination y)
    {
      return x.Id == y.Id;
    }

    public int GetHashCode(CameraPublishDestination obj)
    {
      return obj.GetHashCode();
    }
  }
}
