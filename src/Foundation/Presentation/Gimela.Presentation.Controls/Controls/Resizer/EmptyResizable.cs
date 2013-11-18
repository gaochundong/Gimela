using System.Windows;

namespace Gimela.Presentation.Controls
{
  internal class EmptyResizable : FrameworkElementResizable
  {
    private FrameworkElement element = new FrameworkElement();

    protected override FrameworkElement ResizableTarget
    {
      get { return element; }
    }

    public override double Top { get { return 0; } set { } }
    public override double Left { get { return 0; } set { } }

    protected override Freezable CreateInstanceCore()
    {
      return new EmptyResizable();
    }
  }
}
