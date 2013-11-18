using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Gimela.Rukbat.MPS.BusinessEntities;

namespace Gimela.Rukbat.MPS.BusinessLogic
{
  public interface IStreamingManager
  {
    void StartCameraStreaming(PublishedCamera camera);
    void StopCameraStreaming(PublishedCamera camera);
  }
}
