using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Gimela.Presentation.Controls
{
  [TemplatePart(Name = TopResizerName, Type = typeof(Thumb))]
  [TemplatePart(Name = LeftResizerName, Type = typeof(Thumb))]
  [TemplatePart(Name = RightResizerName, Type = typeof(Thumb))]
  [TemplatePart(Name = BottomResizerName, Type = typeof(Thumb))]
  [TemplatePart(Name = BottomRightResizerName, Type = typeof(Thumb))]
  [TemplatePart(Name = TopRightResizerName, Type = typeof(Thumb))]
  [TemplatePart(Name = TopLeftResizerName, Type = typeof(Thumb))]
  [TemplatePart(Name = BottomLeftResizerName, Type = typeof(Thumb))]
  public class Resizer : Control
  {
    #region Template Part Name

    private const string TopResizerName = "PART_TopResizer";
    private const string LeftResizerName = "PART_LeftResizer";
    private const string RightResizerName = "PART_RightResizer";
    private const string BottomResizerName = "PART_BottomResizer";
    private const string BottomRightResizerName = "PART_BottomRightResizer";
    private const string TopRightResizerName = "PART_TopRightResizer";
    private const string TopLeftResizerName = "PART_TopLeftResizer";
    private const string BottomLeftResizerName = "PART_BottomLeftResizer";

    #endregion

    #region Dependency Properties

    public static readonly DependencyProperty ResizableProperty =
        DependencyProperty.Register("Resizable", typeof(IResizable), typeof(Resizer), new PropertyMetadata(new EmptyResizable()), ValidateResizable);

    public static readonly DependencyProperty IsShowResizeGripProperty =
        DependencyProperty.Register("IsShowResizeGrip", typeof(bool), typeof(Resizer), new FrameworkPropertyMetadata(false));

    public static readonly DependencyProperty StraightResizeBarSizeProperty =
        DependencyProperty.Register("StraightResizeBarSize", typeof(double), typeof(Resizer), new FrameworkPropertyMetadata(4.0, null, CoerceResizeBarSize));

    public static readonly DependencyProperty SlantResizeBarSizeProperty =
        DependencyProperty.Register("SlantResizeBarSize", typeof(double), typeof(Resizer), new FrameworkPropertyMetadata(6.0, null, CoerceResizeBarSize));

    public double StraightResizeBarSize
    {
      get { return (double)GetValue(StraightResizeBarSizeProperty); }
      set { SetValue(StraightResizeBarSizeProperty, value); }
    }

    public IResizable Resizable
    {
      get { return (IResizable)GetValue(ResizableProperty); }
      set { SetValue(ResizableProperty, value); }
    }

    public bool IsShowResizeGrip
    {
      get { return (bool)GetValue(IsShowResizeGripProperty); }
      set { SetValue(IsShowResizeGripProperty, value); }
    }

    public double SlantResizeBarSize
    {
      get { return (double)GetValue(SlantResizeBarSizeProperty); }
      set { SetValue(SlantResizeBarSizeProperty, value); }
    }

    private static bool ValidateResizable(object resizable)
    {
      return resizable != null;
    }

    private static object CoerceResizeBarSize(DependencyObject d, object size)
    {
      return (double)size <= 1 ? 1 : size;
    }

    #endregion

    static Resizer()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Resizer), new FrameworkPropertyMetadata(typeof(Resizer)));
    }

    protected bool _IsTemplateLoaded = false;
    protected Thumb _TopResizer;
    protected Thumb _LeftResizer;
    protected Thumb _RightResizer;
    protected Thumb _BottomResizer;
    protected Thumb _TopRightResizer;
    protected Thumb _TopLeftResizer;
    protected Thumb _BottomLeftResizer;
    protected Thumb _BottomRightResizer;

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _TopResizer = GetTemplateChild<Thumb>(TopResizerName);
      _TopResizer.DragDelta += new DragDeltaEventHandler(ResizeTop);
      _LeftResizer = GetTemplateChild<Thumb>(LeftResizerName);
      _LeftResizer.DragDelta += new DragDeltaEventHandler(ResizeLeft);
      _RightResizer = GetTemplateChild<Thumb>(RightResizerName);
      _RightResizer.DragDelta += new DragDeltaEventHandler(ResizeRight);
      _BottomResizer = GetTemplateChild<Thumb>(BottomResizerName);
      _BottomResizer.DragDelta += new DragDeltaEventHandler(ResizeBottom);
      _BottomRightResizer = GetTemplateChild<Thumb>(BottomRightResizerName);
      _BottomRightResizer.DragDelta += new DragDeltaEventHandler(ResizeBottomRight);
      _TopRightResizer = GetTemplateChild<Thumb>(TopRightResizerName);
      _TopRightResizer.DragDelta += new DragDeltaEventHandler(ResizeTopRight);
      _TopLeftResizer = GetTemplateChild<Thumb>(TopLeftResizerName);
      _TopLeftResizer.DragDelta += new DragDeltaEventHandler(ResizeTopLeft);
      _BottomLeftResizer = GetTemplateChild<Thumb>(BottomLeftResizerName);
      _BottomLeftResizer.DragDelta += new DragDeltaEventHandler(ResizeBottomLeft);

      _IsTemplateLoaded = true;
    }

    private T GetTemplateChild<T>(string childName) where T : FrameworkElement, new()
    {
      return (GetTemplateChild(childName) as T) ?? new T();
    }

    private void ResizeBottomLeft(object sender, DragDeltaEventArgs e)
    {
      ResizeLeft(sender, e);
      ResizeBottom(sender, e);
    }

    private void ResizeTopLeft(object sender, DragDeltaEventArgs e)
    {
      ResizeTop(sender, e);
      ResizeLeft(sender, e);
    }

    private void ResizeTopRight(object sender, DragDeltaEventArgs e)
    {
      ResizeRight(sender, e);
      ResizeTop(sender, e);
    }

    private void ResizeBottomRight(object sender, DragDeltaEventArgs e)
    {
      ResizeBottom(sender, e);
      ResizeRight(sender, e);
    }

    private void ResizeBottom(object sender, DragDeltaEventArgs e)
    {
      if (Resizable.ActualHeight <= Resizable.MinHeight && e.VerticalChange < 0)
      {
        return;
      }

      if (double.IsNaN(Resizable.Height))
      {
        Resizable.Height = Resizable.ActualHeight;
      }

      if (Resizable.Height + e.VerticalChange > MinHeight)
      {
        Resizable.Height += e.VerticalChange;
      }
    }

    private void ResizeRight(object sender, DragDeltaEventArgs e)
    {
      if (Resizable.ActualWidth <= Resizable.MinWidth && e.HorizontalChange < 0)
      {
        return;
      }

      if (double.IsNaN(Resizable.Width))
      {
        Resizable.Width = Resizable.ActualWidth;
      }

      if (Resizable.Width + e.HorizontalChange > MinWidth)
      {
        Resizable.Width += e.HorizontalChange;
      }
    }

    private void ResizeLeft(object sender, DragDeltaEventArgs e)
    {
      if (Resizable.ActualWidth <= Resizable.MinWidth && e.HorizontalChange > 0)
      {
        return;
      }

      if (double.IsNaN(Resizable.Width))
      {
        Resizable.Width = Resizable.ActualWidth;
      }

      if (Resizable.Width - e.HorizontalChange > MinWidth)
      {
        Resizable.Width -= e.HorizontalChange;
        Resizable.Left += e.HorizontalChange;
      }
    }

    private void ResizeTop(object sender, DragDeltaEventArgs e)
    {
      if (Resizable.ActualHeight <= Resizable.MinHeight && e.VerticalChange > 0)
      {
        return;
      }

      if (double.IsNaN(Resizable.Height))
      {
        Resizable.Height = Resizable.ActualHeight;
      }

      if (Resizable.Height - e.VerticalChange > MinHeight)
      {
        Resizable.Height -= e.VerticalChange;
        Resizable.Top += e.VerticalChange;
      }
    }
  }
}
