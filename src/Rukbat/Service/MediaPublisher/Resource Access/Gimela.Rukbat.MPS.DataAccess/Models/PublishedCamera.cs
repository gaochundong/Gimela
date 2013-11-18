using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Data.Magpie;

namespace Gimela.Rukbat.MPS.DataAccess.Models
{
  [Serializable]
  public class PublishedCamera : IMagpieDocumentId
  {
    public string Id { get; set; }

    public PublishedCameraProfile Profile { get; set; }

    public Destination Destination { get; set; }
  }
}
