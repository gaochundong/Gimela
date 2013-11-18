using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Gimela.Crust;
using Gimela.Rukbat.DomainModels;

namespace Gimela.Rukbat.GUI.Controls
{
  /// <summary>
  /// Interaction logic for CameraPresenter.xaml
  /// </summary>
  public partial class CameraPresenter : UserControl, ICameraControl
  {
    #region Ctors

    public CameraPresenter()
    {
      InitializeComponent();
    }

    #endregion

    #region ICameraControl Members

    public ICamera BindingCamera { get; private set; }

    public void AddCameraBinding(ICamera camera)
    {
      RemoveCameraBinding();

      BindingCamera = camera;
      if (BindingCamera != null)
      {
        BindingCamera.NewFrameEvent += new EventHandler(OnBindingCameraNewFrameEvent);
      }
    }

    public void RemoveCameraBinding()
    {
      if (BindingCamera != null)
      {
        BindingCamera.NewFrameEvent -= new EventHandler(OnBindingCameraNewFrameEvent);
      }
      BindingCamera = null;
    }

    #endregion

    #region Invalidate

    private void OnBindingCameraNewFrameEvent(object sender, EventArgs e)
    {
      Invalidate();
    }

    private void Invalidate()
    {
      Monitor.Enter(this);

      if (BindingCamera != null)
      {
        Bitmap origin = BindingCamera.LastFrame;

        if (origin != null)
        {
          Bitmap frame = origin.Clone() as Bitmap;

          if (frame != null)
          {
            DispatcherHelper.InvokeOnUI(() =>
            {
              PresentImage(frame);
            });
          }
        }
      }

      Monitor.Exit(this);
    }

    private void PresentImage(Bitmap frame)
    {
      MemoryStream ms = new MemoryStream();
      frame.Save(ms, ImageFormat.Bmp);
      ms.Position = 0;

      BitmapImage bi = new BitmapImage();
      bi.BeginInit();
      bi.StreamSource = ms;
      bi.EndInit();

      BitmapSource bs = ProcessImage(bi);

      this.surface.Source = bs;
    }

    #endregion

    #region Process Image

    private BitmapSource ProcessImage(BitmapSource bi)
    {
      // 显示前图像处理
      //TransformedBitmap tb = new TransformedBitmap(bi, new RotateTransform(90));

      return bi;
    }

    #endregion

    #region Dependency Property

    public static readonly DependencyProperty TargetCameraProperty =
        DependencyProperty.Register("TargetCamera", typeof(Camera), typeof(CameraPresenter),
        new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTargetCameraValueChanged)));

    public Camera TargetCamera
    {
      get { return (Camera)GetValue(TargetCameraProperty); }
      set { SetValue(TargetCameraProperty, value); }
    }

    private static void OnTargetCameraValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      CameraPresenter presenter = (CameraPresenter)sender;
      presenter.RemoveCameraBinding();

      if (e.OldValue != null)
      {
        Camera camera = e.OldValue as Camera;
        if (camera != null)
        {
          camera.Stop();
        }
      }

      if (e.NewValue != null)
      {
        Camera camera = e.NewValue as Camera;
        if (camera != null)
        {
          camera.Start();
          presenter.AddCameraBinding(camera);
        }
      }
    }

    #endregion
  }
}
