using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Gimela.Rukbat.DPS.Contracts.MessageContracts
{
  [MessageContract]
  public class GetCameraRequest
  {
    [MessageBodyMember]
    public string CameraId { get; set; }
  }
}
