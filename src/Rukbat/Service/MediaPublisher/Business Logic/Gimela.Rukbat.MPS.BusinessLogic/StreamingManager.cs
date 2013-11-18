using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Rukbat.MPS.BusinessEntities;
using Gimela.Rukbat.MPS.BusinessLogic.DomainObjects;

namespace Gimela.Rukbat.MPS.BusinessLogic
{
  public class StreamingManager : IStreamingManager
  {
    private readonly object _syncRoot = new object();
    private List<StreamingCamera> _cameras = new List<StreamingCamera>();

    public void StartCameraStreaming(PublishedCamera camera)
    {
      if (camera == null)
        throw new ArgumentNullException("camera");

      lock (_syncRoot)
      {
        if (_cameras.Count(c => c.Id == camera.Id) > 0) return;

        StreamingCamera streamingCamera = new StreamingCamera(camera);
        streamingCamera.Start();

        _cameras.Add(streamingCamera);
      }
    }

    public void StopCameraStreaming(PublishedCamera camera)
    {
      if (camera == null)
        throw new ArgumentNullException("camera");

      lock (_syncRoot)
      {
        if (_cameras.Count(c => c.Id == camera.Id) == 0) return;

        StreamingCamera streamingCamera = _cameras.Find(c => c.Id == camera.Id);
        streamingCamera.Stop();

        _cameras.Remove(streamingCamera);
      }
    }
  }
}
