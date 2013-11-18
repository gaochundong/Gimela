using System.Collections.Generic;
using System.ServiceModel;
using Gimela.Rukbat.MPS.Contracts.DataContracts;

namespace Gimela.Rukbat.MPS.Contracts.MessageContracts
{
  [MessageContract]
  public class GetPublishedCamerasResponse
  {
    public GetPublishedCamerasResponse()
    {
      PublishedCameras = new List<PublishedCameraData>();
    }

    [MessageBodyMember]
    public List<PublishedCameraData> PublishedCameras { get; set; }
  }
}
