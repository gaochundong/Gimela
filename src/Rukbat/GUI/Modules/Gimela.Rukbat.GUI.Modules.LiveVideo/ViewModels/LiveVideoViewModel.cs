using System;
using System.Threading;
using Gimela.Crust.Tectosphere;
using Gimela.Common.ExceptionHandling;
using Gimela.Management;
using Gimela.Infrastructure.Messaging;
using Gimela.Media.Vision.Motion;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.GUI.Modules.LiveVideo.Models;
using Gimela.Rukbat.GUI.Modules.UIMessage;

namespace Gimela.Rukbat.GUI.Modules.LiveVideo.ViewModels
{
  public class LiveVideoViewModel : ViewModelResponsive
  {
    #region Ctors

    private System.Threading.Timer _keepCameraAliveTimer;

    public LiveVideoViewModel(CameraModel model)
    {
      if (model == null)
        throw new ArgumentNullException("model");

      _keepCameraAliveTimer = new System.Threading.Timer(KeepCameraAlive, null,
        TimeSpan.FromSeconds(0),
        TimeSpan.FromSeconds(4));

      Model = model;

      Refresh();
    }

    #endregion

    #region Model

    public CameraModel Model { get; private set; }

    #endregion

    #region Cleanup

    public override void Cleanup()
    {
      base.Cleanup();

      TargetCamera = null;

      if (_keepCameraAliveTimer != null)
      {
        _keepCameraAliveTimer.Change(Timeout.Infinite, Timeout.Infinite);
        _keepCameraAliveTimer.Dispose();
        _keepCameraAliveTimer = null;
      }
    }

    #endregion

    #region Properties

    private Camera _targetCamera;
    public Camera TargetCamera
    {
      get
      {
        return _targetCamera;
      }
      set
      {
        if (_targetCamera == value) return;

        if (_targetCamera != null)
        {
          _targetCamera.CameraAlarmEvent -= OnCameraAlarmEvent;
          _targetCamera.CameraStartedEvent -= OnCameraStartedEvent;
          _targetCamera.CameraStoppedEvent -= OnCameraStoppedEvent;

          // after the camera stopped, we cannot receive the stoppped event, so fire it here
          OnCameraStoppedEvent(this, new CameraStoppedEventArgs(_targetCamera.Id));

          CleanCameraDecorators(_targetCamera);
        }

        _targetCamera = value;

        if (_targetCamera != null)
        {
          _targetCamera.CameraAlarmEvent += OnCameraAlarmEvent;
          _targetCamera.CameraStartedEvent += OnCameraStartedEvent;
          _targetCamera.CameraStoppedEvent += OnCameraStoppedEvent;

          SetCameraDecorators(_targetCamera);
        }

        RaisePropertyChanged("TargetCamera");
      }
    }

    #endregion

    #region Bindings

    protected override void BindCommands()
    {

    }

    protected override void UnbindCommands()
    {

    }

    protected override void SubscribeMessages()
    {
      Messenger.Default.Register<NotificationMessage<Camera>>(this, message =>
      {
        // 收到窗口正在关闭通知
        if (message.Notification == UIMessageType.LiveVideo_LiveVideoClosingEvent)
        {
          if (this.TargetCamera != null && this.TargetCamera.Id == message.Content.Id)
          {
            // 属性中处理了关闭摄像机操作
            this.TargetCamera = null;
          }

          Cleanup();
        }
      });
    }

    protected override void UnsubscribeMessages()
    {
      Messenger.Default.Unregister<NotificationMessage<Camera>>(this);
    }

    #endregion

    #region Private Methods

    public void SetObject(object target)
    {
      if (target == null)
        throw new ArgumentNullException("target");

      Camera camera = target as Camera;
      if (camera == null)
        throw new ArgumentNullException("camera");

      TargetCamera = camera;
    }

    private void OnCameraAlarmEvent(object sender, CameraAlarmEventArgs e)
    {

    }

    private void OnCameraStartedEvent(object sender, CameraStartedEventArgs e)
    {
      try
      {
        if (e.CameraId == TargetCamera.Id)
        {
          string address = LocalMachine.HostName;
          int port = int.Parse(TargetCamera.VideoSource.Source);
          Model.PublishCamera(TargetCamera, address, port);
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    private void OnCameraStoppedEvent(object sender, CameraStoppedEventArgs e)
    {
      try
      {
        if (e.CameraId == TargetCamera.Id)
        {
          string address = LocalMachine.HostName;
          int port = int.Parse(TargetCamera.VideoSource.Source);
          Model.UnpublishCamera(TargetCamera, address, port);
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    private void KeepCameraAlive(object state)
    {
      try
      {
        Camera camera = TargetCamera;
        if (camera != null)
        {
          // 发送保活消息
          string address = LocalMachine.HostName;
          int port = int.Parse(camera.VideoSource.Source);
          Model.KeepPublishedCameraAlive(camera, address, port);
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    #endregion

    #region Video Processing

    private void SetCameraDecorators(Camera camera)
    {
      SetCameraRotation(camera);
      SetCameraTimestamp(camera);
      SetCameraOnScreenDisplay(camera);
      SetCameraDisplayLogo(camera);
      SetCameraDisplayProtectionMask(camera);
      SetCameraMotionDetection(camera);
    }

    private void CleanCameraDecorators(Camera camera)
    {
      CleanCameraRotation(camera);
      CleanCameraTimestamp(camera);
      CleanCameraOnScreenDisplay(camera);
      CleanCameraDisplayLogo(camera);
      CleanCameraDisplayProtectionMask(camera);
      CleanCameraMotionDetection(camera);
    }

    private void SetCameraRotation(Camera camera)
    {
      camera.IsFlipX = false;
      camera.IsFlipY = false;
    }

    private void SetCameraTimestamp(Camera camera)
    {
      camera.IsDisplayTimestamp = true;
    }

    private void SetCameraOnScreenDisplay(Camera camera)
    {
      camera.IsDisplayOnScreenDisplay = true;
    }

    private void SetCameraDisplayLogo(Camera camera)
    {
      camera.IsDisplayLogo = false;
    }

    private void SetCameraDisplayProtectionMask(Camera camera)
    {
      camera.IsDisplayProtectionMask = false;
    }

    private void SetCameraMotionDetection(Camera camera)
    {
      camera.IsMotionDetection = false;

      MotionDetectorType detectorType = MotionDetectorType.TwoFramesDifference;

      // 设置移动侦测方式
      switch (detectorType)
      {
        case MotionDetectorType.TwoFramesDifference:
          camera.MotionDetector = new MotionDetector(new TwoFramesDifferenceDetector(true));
          SetCameraMotionDetectorProcessor(camera);
          break;
        case MotionDetectorType.CustomFrameDifference:
          camera.MotionDetector = new MotionDetector(new CustomFrameDifferenceDetector(true, true));
          SetCameraMotionDetectorProcessor(camera);
          break;
        case MotionDetectorType.SimpleBackgroundModeling:
          camera.MotionDetector = new MotionDetector(new SimpleBackgroundModelingDetector(true, true));
          SetCameraMotionDetectorProcessor(camera);
          break;
        default:
          break;
      }
    }

    private void SetCameraMotionDetectorProcessor(Camera camera)
    {
      MotionProcessorType motionProcessorAlgorithm = MotionProcessorType.BlobCountingObjects;

      // 设置移动侦测处理算法
      switch (motionProcessorAlgorithm)
      {
        case MotionProcessorType.GridMotionArea:
          camera.MotionDetector.MotionProcessingAlgorithm = new GridMotionAreaProcessing();
          ((GridMotionAreaProcessing)camera.MotionDetector.MotionProcessingAlgorithm).HighlightColor = System.Drawing.Color.Blue;
          break;
        case MotionProcessorType.BlobCountingObjects:
          camera.MotionDetector.MotionProcessingAlgorithm = new BlobCountingObjectsProcessing();
          ((BlobCountingObjectsProcessing)camera.MotionDetector.MotionProcessingAlgorithm).HighlightColor = System.Drawing.Color.Blue;
          break;
        case MotionProcessorType.MotionBorderHighlighting:
          camera.MotionDetector.MotionProcessingAlgorithm = new MotionBorderHighlighting();
          ((MotionBorderHighlighting)camera.MotionDetector.MotionProcessingAlgorithm).HighlightColor = System.Drawing.Color.Blue;
          break;
        case MotionProcessorType.MotionAreaHighlighting:
          camera.MotionDetector.MotionProcessingAlgorithm = new MotionAreaHighlighting();
          ((MotionAreaHighlighting)camera.MotionDetector.MotionProcessingAlgorithm).HighlightColor = System.Drawing.Color.Blue;
          break;
        default:
          break;
      }
    }

    private void CleanCameraRotation(Camera camera)
    {
      camera.IsFlipX = false;
      camera.IsFlipY = false;
    }

    private void CleanCameraTimestamp(Camera camera)
    {
      camera.IsDisplayTimestamp = false;
    }

    private void CleanCameraOnScreenDisplay(Camera camera)
    {
      camera.IsDisplayOnScreenDisplay = false;
    }

    private void CleanCameraDisplayLogo(Camera camera)
    {
      camera.IsDisplayLogo = false;
    }

    private void CleanCameraDisplayProtectionMask(Camera camera)
    {
      camera.IsDisplayProtectionMask = false;
    }

    private void CleanCameraMotionDetection(Camera camera)
    {
      camera.IsMotionDetection = false;
      camera.MotionDetector = null;
    }

    #endregion
  }
}
