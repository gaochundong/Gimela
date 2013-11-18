using System;
using System.Windows;

namespace Gimela.Presentation.Controls
{
  public class WindowResizable : FrameworkElementResizable
  {
    public Window TargetWindow
    {
      get { return (Window)GetValue(TargetWindowProperty); }
      set { SetValue(TargetWindowProperty, value); }
    }

    public static readonly DependencyProperty TargetWindowProperty =
        DependencyProperty.Register("TargetWindow", typeof(Window), typeof(WindowResizable));

    protected override FrameworkElement ResizableTarget
    {
      get
      {
        if (TargetWindow == null)
        {
          throw new Exception("TargetWindow should not be null.");
        }

        return TargetWindow;
      }
    }

    public override double Top
    {
      get { return TargetWindow.Top; }
      set { TargetWindow.Top = value; }
    }

    public override double Left
    {
      get { return TargetWindow.Left; }
      set { TargetWindow.Left = value; }
    }

    protected override Freezable CreateInstanceCore()
    {
      return new WindowResizable() { TargetWindow = this.TargetWindow };
    }
  }
}
