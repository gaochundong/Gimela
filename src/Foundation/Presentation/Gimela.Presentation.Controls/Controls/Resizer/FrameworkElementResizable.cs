using System.Windows;

namespace Gimela.Presentation.Controls
{
  public abstract class FrameworkElementResizable : Freezable, IResizable
  {
    protected abstract FrameworkElement ResizableTarget { get; }

    #region IResizable Members

    public abstract double Top { get; set; }

    public abstract double Left { get; set; }

    public double Height
    {
      get { return ResizableTarget.Height; }
      set { ResizableTarget.Height = value; }
    }

    public double Width
    {
      get { return ResizableTarget.Width; }
      set { ResizableTarget.Width = value; }
    }

    public double MinHeight
    {
      get { return ResizableTarget.MinHeight; }
    }

    public double MinWidth
    {
      get { return ResizableTarget.MinWidth; }
    }

    public double MaxHeight
    {
      get { return ResizableTarget.MaxHeight; }
    }

    public double MaxWidth
    {
      get { return ResizableTarget.MaxWidth; }
    }

    public double ActualHeight
    {
      get { return ResizableTarget.ActualHeight; }
    }

    public double ActualWidth
    {
      get { return ResizableTarget.ActualWidth; }
    }

    #endregion
  }
}
