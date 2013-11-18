using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gimela.Rukbat.GUI.Controls
{
  public class CameraSurface : Image
  {
    #region Ctor

    public CameraSurface()
    {
    }

    #endregion

    #region Properties

    public double Rotation
    {
      get { return (double)GetValue(RotationProperty); }
      set { SetValue(RotationProperty, value); }
    }

    public static readonly DependencyProperty RotationProperty =
        DependencyProperty.Register("Rotation", typeof(double), typeof(CameraSurface),
        new UIPropertyMetadata(0d, new PropertyChangedCallback(RotationProperty_Changed)));

    private static void RotationProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      CameraSurface obj = sender as CameraSurface;
      if (obj != null)
      {
        obj.LayoutTransform = new RotateTransform((double)e.NewValue);
      }
    }

    public float Framerate
    {
      get { return (float)GetValue(FramerateProperty); }
      set { SetValue(FramerateProperty, value); }
    }

    public static readonly DependencyProperty FramerateProperty =
        DependencyProperty.Register("Framerate", typeof(float), typeof(CameraSurface),
        new UIPropertyMetadata(default(float)));

    #endregion
  }
}
