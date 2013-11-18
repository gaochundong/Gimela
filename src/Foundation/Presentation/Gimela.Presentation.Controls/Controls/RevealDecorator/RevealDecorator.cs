using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Gimela.Presentation.Controls
{
  public class RevealDecorator : Decorator
  {
    #region Constructors

    static RevealDecorator()
    {
      ClipToBoundsProperty.OverrideMetadata(typeof(RevealDecorator), new FrameworkPropertyMetadata(true));
    }

    #endregion

    #region Public Properties

    public bool IsExpanded
    {
      get { return (bool)GetValue(IsExpandedProperty); }
      set { SetValue(IsExpandedProperty, value); }
    }

    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RevealDecorator), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsExpandedChanged)));

    private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      RevealDecorator reveal = d as RevealDecorator;

      if (reveal.Child != null && (bool)e.NewValue == true)
        reveal.Child.Visibility = Visibility.Visible;

      ((RevealDecorator)d).SetupAnimation((bool)e.NewValue);
    }

    public double Duration
    {
      get { return (double)GetValue(DurationProperty); }
      set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register("Duration", typeof(double), typeof(RevealDecorator), new UIPropertyMetadata(250.0));

    public HorizontalRevealMode HorizontalReveal
    {
      get { return (HorizontalRevealMode)GetValue(HorizontalRevealProperty); }
      set { SetValue(HorizontalRevealProperty, value); }
    }

    public static readonly DependencyProperty HorizontalRevealProperty =
        DependencyProperty.Register("HorizontalReveal", typeof(HorizontalRevealMode), typeof(RevealDecorator), new UIPropertyMetadata(HorizontalRevealMode.None));

    public VerticalRevealMode VerticalReveal
    {
      get { return (VerticalRevealMode)GetValue(VerticalRevealProperty); }
      set { SetValue(VerticalRevealProperty, value); }
    }

    public static readonly DependencyProperty VerticalRevealProperty =
        DependencyProperty.Register("VerticalReveal", typeof(VerticalRevealMode), typeof(RevealDecorator), new UIPropertyMetadata(VerticalRevealMode.FromTopToBottom));

    public double AnimationProgress
    {
      get { return (double)GetValue(AnimationProgressProperty); }
      set { SetValue(AnimationProgressProperty, value); }
    }

    public static readonly DependencyProperty AnimationProgressProperty =
        DependencyProperty.Register("AnimationProgress", typeof(double), typeof(RevealDecorator), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, null, new CoerceValueCallback(OnCoerceAnimationProgress)));

    private static object OnCoerceAnimationProgress(DependencyObject d, object baseValue)
    {
      double num = (double)baseValue;
      if (num < 0.0)
      {
        return 0.0;
      }
      else if (num > 1.0)
      {
        return 1.0;
      }

      return baseValue;
    }

    #endregion

    #region Implementation

    protected override Size MeasureOverride(Size constraint)
    {
      UIElement child = Child;
      if (child != null)
      {
        child.Measure(constraint);

        double percent = AnimationProgress;
        double width = CalculateWidth(child.DesiredSize.Width, percent, HorizontalReveal);
        double height = CalculateHeight(child.DesiredSize.Height, percent, VerticalReveal);
        return new Size(width, height);
      }

      return new Size();
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
      UIElement child = Child;
      if (child != null)
      {
        double percent = AnimationProgress;
        HorizontalRevealMode horizontalReveal = HorizontalReveal;
        VerticalRevealMode verticalReveal = VerticalReveal;

        double childWidth = this.HorizontalAlignment == HorizontalAlignment.Stretch ? arrangeSize.Width : child.DesiredSize.Width;//child.DesiredSize.Width;
        double childHeight = this.VerticalAlignment == VerticalAlignment.Stretch ? arrangeSize.Height : child.DesiredSize.Height;//child.DesiredSize.Height;
        double x = CalculateLeft(childWidth, percent, horizontalReveal);
        double y = CalculateTop(childHeight, percent, verticalReveal);

        child.Arrange(new Rect(x, y, childWidth, childHeight));

        childWidth = child.RenderSize.Width;
        childHeight = child.RenderSize.Height;
        double width = CalculateWidth(childWidth, percent, horizontalReveal);
        double height = CalculateHeight(childHeight, percent, verticalReveal);
        return new Size(width, height);
      }

      return new Size();
    }

    private static double CalculateLeft(double width, double percent, HorizontalRevealMode reveal)
    {
      if (reveal == HorizontalRevealMode.FromRightToLeft)
      {
        return (percent - 1.0) * width;
      }
      else if (reveal == HorizontalRevealMode.FromCenterToEdge)
      {
        return (percent - 1.0) * width * 0.5;
      }
      else
      {
        return 0.0;
      }
    }

    private static double CalculateTop(double height, double percent, VerticalRevealMode reveal)
    {
      if (reveal == VerticalRevealMode.FromBottomToTop)
      {
        return (percent - 1.0) * height;
      }
      else if (reveal == VerticalRevealMode.FromCenterToEdge)
      {
        return (percent - 1.0) * height * 0.5;
      }
      else
      {
        return 0.0;
      }
    }

    private static double CalculateWidth(double originalWidth, double percent, HorizontalRevealMode reveal)
    {
      if (reveal == HorizontalRevealMode.None)
      {
        return originalWidth;
      }
      else
      {
        return originalWidth * percent;
      }
    }

    private static double CalculateHeight(double originalHeight, double percent, VerticalRevealMode reveal)
    {
      if (reveal == VerticalRevealMode.None)
      {
        return originalHeight;
      }
      else
      {
        return originalHeight * percent;
      }
    }

    private void SetupAnimation(bool isExpanded)
    {
      double currentProgress = AnimationProgress;
      if (isExpanded)
      {
        currentProgress = 1.0 - currentProgress;
      }

      DoubleAnimation animation = new DoubleAnimation();
      animation.To = isExpanded ? 1.0 : 0.0;
      animation.Duration = TimeSpan.FromMilliseconds(Duration * currentProgress);
      animation.FillBehavior = FillBehavior.HoldEnd;
      animation.Completed += new EventHandler(OnAnimationCompleted);


      this.BeginAnimation(AnimationProgressProperty, animation);
    }

    void OnAnimationCompleted(object sender, EventArgs e)
    {
      if (!this.IsExpanded)
        this.Child.Visibility = Visibility.Hidden;
    }

    #endregion
  }
}
