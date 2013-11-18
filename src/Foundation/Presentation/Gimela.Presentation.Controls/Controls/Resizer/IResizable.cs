
namespace Gimela.Presentation.Controls
{
  public interface IResizable
  {
    double Top { get; set; }
    double Left { get; set; }
    double Height { get; set; }
    double Width { get; set; }
    double MinHeight { get; }
    double MinWidth { get; }
    double MaxHeight { get; }
    double MaxWidth { get; }
    double ActualHeight { get; }
    double ActualWidth { get; }
  }
}
