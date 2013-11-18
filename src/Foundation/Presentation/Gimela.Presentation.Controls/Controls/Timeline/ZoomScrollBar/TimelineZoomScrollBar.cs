using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Gimela.Presentation.Controls.Timeline
{
  public class TimelineZoomScrollBar : ScrollBar, INotifyPropertyChanged
  {
    private readonly TimelineTickCollection tickCollection;

    private Track _zoomTrack;
    private EventHandler actualWidthChangedHandler;

    public TimelineZoomScrollBar()
      : base()
    {
      ChangeZoom = new DelegateCommand<object>(ChangeZoomExecute);

      tickCollection =
        new TimelineTickCollection
        {
          BigTickMinimumSize = 50,
          MediumTickMinimumSize = 10
        };

      Unloaded += (ZoomScrollBarUnloaded);
      actualWidthChangedHandler = ActualWidthChanged;
    }

    public TimelineTickCollection TickCollection { get { return tickCollection; } }

    public double Zoom
    {
      get { return _zoomTrack.ActualWidth / (End - Start).TotalSeconds; }
    }

    public ICommand ChangeZoom { get; set; }

    #region Dependency Properties

    #region ZoomMode

    /// <summary>
    /// ZoomMode Dependency Property
    /// </summary>
    public static readonly DependencyProperty ZoomModeProperty =
        DependencyProperty.Register("ZoomMode", typeof(bool), typeof(TimelineZoomScrollBar),
            new FrameworkPropertyMetadata((bool)false));

    /// <summary>
    /// 是否缩放，这是一个依赖属性。
    /// </summary>
    public bool ZoomMode
    {
      get { return (bool)GetValue(ZoomModeProperty); }
      set { SetValue(ZoomModeProperty, value); }
    }

    #endregion

    #region Now

    /// <summary>
    /// Now Dependency Property
    /// </summary>
    public static readonly DependencyProperty NowProperty =
        DependencyProperty.Register("Now", typeof(DateTime), typeof(TimelineZoomScrollBar),
            new FrameworkPropertyMetadata((DateTime)DateTime.Now,
                new PropertyChangedCallback(OnNowChanged)));

    /// <summary>
    /// 当前时间，这是一个依赖属性。
    /// </summary>
    public DateTime Now
    {
      get { return (DateTime)GetValue(NowProperty); }
      set { SetValue(NowProperty, value); }
    }

    /// <summary>
    /// Handles changes to the Now property.
    /// </summary>
    private static void OnNowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimelineZoomScrollBar)d).OnNowChanged(e);
    }

    /// <summary>
    /// Provides derived classes an opportunity to handle changes to the Now property.
    /// </summary>
    protected virtual void OnNowChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion

    #region Start

    /// <summary>
    /// Start Dependency Property
    /// </summary>
    public static readonly DependencyProperty StartProperty =
        DependencyProperty.Register("Start", typeof(DateTime), typeof(TimelineZoomScrollBar),
            new FrameworkPropertyMetadata((DateTime)DateTime.MinValue,
                new PropertyChangedCallback(OnStartChanged),
                new CoerceValueCallback(CoerceStartValue)));

    /// <summary>
    /// 开始时间，这是一个依赖属性。
    /// </summary>
    public DateTime Start
    {
      get { return (DateTime)GetValue(StartProperty); }
      set { SetValue(StartProperty, value); }
    }

    /// <summary>
    /// Handles changes to the Start property.
    /// </summary>
    private static void OnStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimelineZoomScrollBar)d).OnStartChanged(e);
    }

    /// <summary>
    /// Provides derived classes an opportunity to handle changes to the Start property.
    /// </summary>
    protected virtual void OnStartChanged(DependencyPropertyChangedEventArgs e)
    {
      tickCollection.Start = Start;
      tickCollection.PixelsPerSecond = _zoomTrack.ActualWidth / (End - Start).TotalSeconds;
      OnPropertyChanged("Zoom");
    }

    /// <summary>
    /// Coerces the Start value.
    /// </summary>
    private static object CoerceStartValue(DependencyObject d, object value)
    {
      return value;
    }

    #endregion

    #region End

    /// <summary>
    /// End Dependency Property
    /// </summary>
    public static readonly DependencyProperty EndProperty =
        DependencyProperty.Register("End", typeof(DateTime), typeof(TimelineZoomScrollBar),
            new FrameworkPropertyMetadata((DateTime)DateTime.MaxValue,
                new PropertyChangedCallback(OnEndChanged),
                new CoerceValueCallback(CoerceEndValue)));

    /// <summary>
    /// 结束时间，这是一个依赖属性。
    /// </summary>
    public DateTime End
    {
      get { return (DateTime)GetValue(EndProperty); }
      set { SetValue(EndProperty, value); }
    }

    /// <summary>
    /// Handles changes to the End property.
    /// </summary>
    private static void OnEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimelineZoomScrollBar)d).OnEndChanged(e);
    }

    /// <summary>
    /// Provides derived classes an opportunity to handle changes to the End property.
    /// </summary>
    protected virtual void OnEndChanged(DependencyPropertyChangedEventArgs e)
    {
      tickCollection.End = End;
      tickCollection.PixelsPerSecond = _zoomTrack.ActualWidth / (End - Start).TotalSeconds;
      OnPropertyChanged("Zoom");
    }

    /// <summary>
    /// Coerces the End value.
    /// </summary>
    private static object CoerceEndValue(DependencyObject d, object value)
    {
      return value;
    }

    #endregion

    #region InnerTrackWidth

    /// <summary>
    /// InnerTrackWidth Dependency Property
    /// </summary>
    public static readonly DependencyProperty InnerTrackWidthProperty =
        DependencyProperty.Register("InnerTrackWidth", typeof(double), typeof(TimelineZoomScrollBar),
            new FrameworkPropertyMetadata((double)0,
                new PropertyChangedCallback(OnInnerTrackWidthChanged)));

    /// <summary>
    /// 内部轨道宽度，这是一个依赖属性。
    /// </summary>
    public double InnerTrackWidth
    {
      get { return (double)GetValue(InnerTrackWidthProperty); }
      set { SetValue(InnerTrackWidthProperty, value); }
    }

    /// <summary>
    /// Handles changes to the InnerTrackWidth property.
    /// </summary>
    private static void OnInnerTrackWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimelineZoomScrollBar)d).OnInnerTrackWidthChanged(e);
    }

    /// <summary>
    /// Provides derived classes an opportunity to handle changes to the InnerTrackWidth property.
    /// </summary>
    protected virtual void OnInnerTrackWidthChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion

    #endregion

    #region RoutedUICommands

    public static RoutedUICommand ZoomViewportCommand
        = new RoutedUICommand("Increase/Decrease the zoom of the Viewport while anchoring to one side",
            "ZoomViewportCommand", typeof(TimelineZoomScrollBar));

    #endregion

    void ZoomScrollBarUnloaded(object sender, RoutedEventArgs e)
    {
      DetachFromVisualTree();
      actualWidthChangedHandler = null;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      AttachToVisualTree();
    }

    private void AttachToVisualTree()
    {
      DetachFromVisualTree();

      _zoomTrack = GetTemplateChild("PART_Track") as Track;
      if (_zoomTrack != null)
      {
        DependencyPropertyDescriptor dpdActualWidth =
          DependencyPropertyDescriptor.FromProperty(ActualWidthProperty, typeof(FrameworkElement));

        dpdActualWidth.AddValueChanged(_zoomTrack, actualWidthChangedHandler);
      }
    }

    private void ActualWidthChanged(object sender, EventArgs e)
    {
      // notify that Zoom has changed
      tickCollection.ViewportExtent = _zoomTrack.ActualWidth;
      tickCollection.ViewportOffset = 0;
      tickCollection.PixelsPerSecond = _zoomTrack.ActualWidth / (End - Start).TotalSeconds;
      OnPropertyChanged("Zoom");
    }

    private void DetachFromVisualTree()
    {
      if (_zoomTrack != null)
      {
        DependencyPropertyDescriptor dpdActualWidth =
          DependencyPropertyDescriptor.FromProperty(ActualWidthProperty, typeof(FrameworkElement));

        dpdActualWidth.RemoveValueChanged(_zoomTrack, actualWidthChangedHandler);

        _zoomTrack = null;
      }
    }

    public void ChangeZoomExecute(object sender)
    {
      ZoomViewportCommand.Execute(
        new TimelineZoomViewportParameters() { ViewportAnchor = TimelineZoomAnchor.Left, ViewportScale = 1.1 },
        this);
    }

    public static void OnViewportThumbLoaded(object sender, RoutedEventArgs e)
    {
      var thumb = sender as Thumb;
      if (thumb != null)
      {
        thumb.DragStarted += ViewportThumbDragStarted;
        thumb.DragDelta += ViewportThumbDragDelta;
        thumb.DragCompleted += ViewportThumbDragCompleted;
        thumb.Unloaded += ViewportThumbUnloaded;
      }
    }

    public static void ViewportThumbUnloaded(object sender, RoutedEventArgs e)
    {
      var thumb = sender as Thumb;
      if (thumb != null)
      {
        thumb.DragStarted -= ViewportThumbDragStarted;
        thumb.DragDelta -= ViewportThumbDragDelta;
        thumb.DragCompleted -= ViewportThumbDragCompleted;
        thumb.Unloaded -= ViewportThumbUnloaded;
      }
    }

    private static double OriginalWidth;

    public static void ViewportThumbDragStarted(object sender, DragStartedEventArgs e)
    {
      var scrollThumb = VisualTreeExtraHelper.FindVisualParent<Thumb>(sender as DependencyObject);
      if (scrollThumb != null)
      {
        OriginalWidth = scrollThumb.ActualWidth;
        e.Handled = true;
      }
    }

    public static void ViewportThumbDragDelta(object sender, DragDeltaEventArgs e)
    {
      var scrollThumb = VisualTreeExtraHelper.FindVisualParent<Thumb>(sender as DependencyObject);
      if (scrollThumb != null)
      {
        var scrollThumbMargin = scrollThumb.Margin;
        scrollThumbMargin.Left += e.HorizontalChange;
        scrollThumb.Margin = scrollThumbMargin;
        e.Handled = true;
      }
    }

    public static void ViewportThumbDragCompleted(object sender, DragCompletedEventArgs e)
    {
      var scrollThumb = VisualTreeExtraHelper.FindVisualParent<Thumb>(sender as DependencyObject);
      if (scrollThumb != null)
      {
        var newMarginLeft = scrollThumb.Margin.Left;
        var change = (newMarginLeft - OriginalWidth) * -1;
        var scale = OriginalWidth / scrollThumb.ActualWidth;

        scrollThumb.Margin = new Thickness(-9, 1, -9, 1);  // reset the margin back to what it was

        ZoomViewportCommand.Execute(
          new TimelineZoomViewportParameters()
          {
            ViewportAnchor = TimelineZoomAnchor.Right,
            ViewportScale = scale
          },
            scrollThumb
          );

        e.Handled = true;
      }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}
