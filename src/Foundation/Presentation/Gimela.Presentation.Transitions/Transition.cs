using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gimela.Presentation.Transitions
{
    /// <summary>
    /// 过渡的基类
    /// </summary>
    public class Transition : DependencyObject
    {
        #region Dependency Property

        /// <summary>
        /// 用于表示是否剪裁此元素的内容（或来自此元素的子元素的内容）以适合包含元素的大小。
        /// </summary>
        public bool ClipToBounds
        {
            get { return (bool)GetValue(ClipToBoundsProperty); }
            set { SetValue(ClipToBoundsProperty, value); }
        }

        /// <summary>
        /// 用于表示是否剪裁此元素的内容（或来自此元素的子元素的内容）以适合包含元素的大小。
        /// </summary>
        public static readonly DependencyProperty ClipToBoundsProperty = DependencyProperty.Register("ClipToBounds", typeof(bool), typeof(Transition), new UIPropertyMetadata(false));

        /// <summary>
        /// 是否新内容呈现在最上层，否则在最后面。
        /// </summary>
        public bool IsNewContentTopmost
        {
            get { return (bool)GetValue(IsNewContentTopmostProperty); }
            set { SetValue(IsNewContentTopmostProperty, value); }
        }

        /// <summary>
        /// 是否新内容呈现在最上层
        /// </summary>
        public static readonly DependencyProperty IsNewContentTopmostProperty = DependencyProperty.Register("IsNewContentTopmost", typeof(bool), typeof(Transition), new UIPropertyMetadata(true));

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when an element is Removed from the TransitionElement's visual tree
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected internal virtual void BeginTransition(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
            EndTransition(transitionElement, oldContent, newContent);
        }

        /// <summary>
        /// Transitions should call this method when they are done 
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected void EndTransition(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
            OnTransitionEnded(transitionElement, oldContent, newContent);

            transitionElement.OnTransitionCompleted(this);
        }

        /// <summary>
        /// Transitions can override this to perform cleanup at the end of the transition
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected virtual void OnTransitionEnded(TransitionElement transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
        }

        /// <summary>
        /// Returns a clone of the element and hides it in the main tree
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected static Brush CreateBrush(TransitionElement transitionElement, ContentPresenter content)
        {
            ((Decorator)content.Parent).Visibility = Visibility.Hidden;

            VisualBrush brush = new VisualBrush(content);
            brush.ViewportUnits = BrushMappingMode.Absolute;
            RenderOptions.SetCachingHint(brush, CachingHint.Cache);
            RenderOptions.SetCacheInvalidationThresholdMinimum(brush, 40);
            return brush;
        }

        #endregion
    }
}
