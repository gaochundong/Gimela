using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Gimela.Common.ExceptionHandling;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Media.Video;
using Gimela.Rukbat.DVC.BusinessEntities;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public class StreamingManager : IStreamingManager
  {
    private readonly object _accessLock = new object();
    private List<PublishedCamera> _cameras = new List<PublishedCamera>();
    private ConcurrentDictionary<CameraPublishDestination, DateTime> _timeToAlive = new ConcurrentDictionary<CameraPublishDestination, DateTime>(new CameraPublishDestinationEqualityComparer());
    private static System.Threading.Timer _keepAliveTimer;
    private static readonly int _aliveSeconds = 15;

    public StreamingManager()
    {
      _keepAliveTimer = new System.Threading.Timer(CheckCameraKeepAlive, null,
        TimeSpan.FromSeconds(0),
        TimeSpan.FromSeconds(15));
    }

    #region IStreamManager Members

    public byte[] GetCameraSnapshot(string cameraId)
    {
      byte[] snapshot;

      PublishedCamera camera = GetPublishedCamera(cameraId);
      if (camera != null)
      {
        do
        {
          snapshot = camera.LatestFrameBytes;
          if (snapshot == null) Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }
        while (snapshot == null);
      }
      else
      {
        using (camera = new PublishedCamera(Locator.Get<ICameraManager>().GetCamera(cameraId)))
        {
          camera.Start();

          do
          {
            snapshot = camera.LatestFrameBytes;
            if (snapshot == null) Thread.Sleep(TimeSpan.FromMilliseconds(100));
          }
          while (snapshot == null);

          camera.Stop();
        }
      }

      return snapshot;
    }

    public bool IsCameraPublished(string cameraId)
    {
      return GetPublishedCamera(cameraId) != null;
    }

    public PublishedCamera GetPublishedCamera(string cameraId)
    {
      return _cameras.Find(c => c.Id == cameraId);
    }

    public ReadOnlyCollection<PublishedCamera> GetPublishedCameras()
    {
      return new ReadOnlyCollection<PublishedCamera>(_cameras);
    }

    public PublishedCamera PublishCamera(string cameraId, PublishDestination destination)
    {
      lock (_accessLock)
      {
        if (destination == null)
          throw new ArgumentNullException("destination");

        PublishedCamera camera = GetPublishedCamera(cameraId);

        if (camera != null)
        {
          camera.AddDestination(destination);
        }
        else
        {
          camera = new PublishedCamera(Locator.Get<ICameraManager>().GetCamera(cameraId)).AddDestination(destination);
          camera.Sender = StreamSenderFactory.GetSender().Start();
          camera.CameraErrorOccurred += new EventHandler<CameraErrorOccurredEventArgs>(OnCameraErrorOccurred);
          camera.Start();
          _cameras.Add(camera);

          _timeToAlive.AddOrUpdate(new CameraPublishDestination(cameraId, destination),
            DateTime.Now, (k, v) => { return DateTime.Now; });
        }

        return camera;
      }
    }

    public PublishedCamera PublishCamera(string cameraId, ICollection<PublishDestination> destinations)
    {
      lock (_accessLock)
      {
        if (destinations == null)
          throw new ArgumentNullException("destinations");

        PublishedCamera camera = null;
        foreach (var dest in destinations)
        {
          camera = PublishCamera(cameraId, dest);
        }
        return camera;
      }
    }

    public void UnpublishCamera(string cameraId, PublishDestination destination)
    {
      lock (_accessLock)
      {
        if (destination == null)
          throw new ArgumentNullException("destination");

        PublishedCamera camera = GetPublishedCamera(cameraId);
        if (camera != null)
        {
          camera.RemoveDestination(destination);

          DateTime aliveTime;
          _timeToAlive.TryRemove(
            new CameraPublishDestination(cameraId, destination), out aliveTime);

          if (camera.Destinations.Count <= 0)
          {
            camera.CameraErrorOccurred -= new EventHandler<CameraErrorOccurredEventArgs>(OnCameraErrorOccurred);
            camera.Stop();
            _cameras.Remove(camera);
            camera.Dispose();
          }
        }
      }
    }

    public void UnpublishCamera(string cameraId, ICollection<PublishDestination> destinations)
    {
      lock (_accessLock)
      {
        if (destinations == null)
          throw new ArgumentNullException("destinations");

        PublishedCamera camera = GetPublishedCamera(cameraId);
        if (camera != null)
        {
          foreach (var dest in destinations)
          {
            camera.RemoveDestination(dest);

            DateTime aliveTime;
            _timeToAlive.TryRemove(
              new CameraPublishDestination(cameraId, dest), out aliveTime);
          }
          if (camera.Destinations.Count <= 0)
          {
            camera.CameraErrorOccurred -= new EventHandler<CameraErrorOccurredEventArgs>(OnCameraErrorOccurred);
            camera.Stop();
            _cameras.Remove(camera);
            camera.Dispose();
          }
        }
      }
    }

    public void UnpublishCamera(string cameraId)
    {
      lock (_accessLock)
      {
        PublishedCamera camera = GetPublishedCamera(cameraId);
        if (camera != null)
        {
          foreach (var dest in camera.Destinations)
          {
            DateTime aliveTime;
            _timeToAlive.TryRemove(
              new CameraPublishDestination(cameraId, dest), out aliveTime);
          }
          camera.ClearDestination();
          camera.CameraErrorOccurred -= new EventHandler<CameraErrorOccurredEventArgs>(OnCameraErrorOccurred);
          camera.Stop();
          _cameras.Remove(camera);
          camera.Dispose();
        }
      }
    }

    public void KeepAlive(string cameraId, PublishDestination destination)
    {
      if (destination == null)
        throw new ArgumentNullException("destination");

      CameraPublishDestination dest = new CameraPublishDestination(cameraId, destination);
      if (_timeToAlive.ContainsKey(dest))
      {
        _timeToAlive[dest] = DateTime.Now;
      }
    }

    #endregion

    #region Private Methods

    private void OnCameraErrorOccurred(object sender, CameraErrorOccurredEventArgs e)
    {
      // send error notification to client side
    }

    private void CheckCameraKeepAlive(object state)
    {
      try
      {
        foreach (var item in _timeToAlive)
        {
          // 如果摄像机的保活时间超过一定时间，则自动取消发布视频流
          if ((DateTime.Now - item.Value).TotalSeconds > _aliveSeconds)
          {
            UnpublishCamera(item.Key.CameraId, item.Key.Destination);
          }
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    #endregion
  }
}
