using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Gimela.Presentation.Transitions
{
    /// <summary>
    /// 逐渐消失
    /// </summary>
    public class FadeTransition : Transition
    {
        static FadeTransition()
        {
            IsNewContentTopmostProperty.OverrideMetadata(typeof(FadeTransition), new FrameworkPropertyMetadata(false));
        }

        public Duration Duration
        {
            get { return (Duration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(Duration), typeof(FadeTransition), new UIPropertyMetadata(Duration.Automatic));

        protected internal override void BeginTransition(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
            DoubleAnimation da = new DoubleAnimation(0, Duration);
            da.Completed += delegate
            {
                EndTransition(transitionElement, oldContent, newContent);
            };
            oldContent.BeginAnimation(UIElement.OpacityProperty, da);
        }

        protected override void OnTransitionEnded(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
            oldContent.BeginAnimation(UIElement.OpacityProperty, null);
        }
    }
}
