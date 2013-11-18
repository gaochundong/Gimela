using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Gimela.Rukbat.DPS.Contracts.DataContracts;

namespace Gimela.Rukbat.DPS.Contracts.MessageContracts
{
  [MessageContract]
  public class GetCamerasResponse
  {
    public GetCamerasResponse()
    {
      Cameras = new List<Camera>();
    }

    [MessageBodyMember]
    public List<Camera> Cameras { get; set; }
  }
}
