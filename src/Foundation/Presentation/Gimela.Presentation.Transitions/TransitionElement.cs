using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace Gimela.Presentation.Transitions
{
    /// <summary>
    /// 过渡器
    /// </summary>
    [ContentProperty("Content")]
    public class TransitionElement : FrameworkElement
    {
        #region Variables

        /// <summary>
        /// 包含的UIElement
        /// </summary>
        private UIElementCollection _children;

        /// <summary>
        /// 当前装饰器 为可视化树中其下面的元素提供一个装饰器层。
        /// </summary>
        private AdornerDecorator _currentHost;

        /// <summary>
        /// 前一个装饰器 为可视化树中其下面的元素提供一个装饰器层。
        /// </summary>
        private AdornerDecorator _previousHost;

        /// <summary>
        /// IsTransitioning 活动使用的转换器
        /// </summary>
        private Transition _activeTransition;

        #endregion

        #region Constructors

        public TransitionElement()
        {
            _children = new UIElementCollection(this, null);

            ContentPresenter currentContent = new ContentPresenter();
            _currentHost = new AdornerDecorator();
            _currentHost.Child = currentContent;

            _children.Add(_currentHost);

            ContentPresenter previousContent = new ContentPresenter();
            _previousHost = new AdornerDecorator();
            _previousHost.Child = previousContent;
        }

        static TransitionElement()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof(TransitionElement), new FrameworkPropertyMetadata(null, CoerceClipToBounds));
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Force clip to be true if the active Transition requires it
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceClipToBounds(object element, object value)
        {
            TransitionElement transitionElement = (TransitionElement)element;
            bool clip = (bool)value;
            if (!clip && transitionElement.IsTransitioning)
            {
                Transition transition = transitionElement.Transition;
                if (transition.ClipToBounds)
                    return true;
            }
            return value;
        }

        /// <summary>
        /// TransitionElement 内容，它是一个 Object.
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content",
                typeof(object),
                typeof(TransitionElement),
                new UIPropertyMetadata(null, OnContentChanged, CoerceContent));

        private static object CoerceContent(object element, object value)
        {
            //Don't update content until done transitioning
            TransitionElement te = (TransitionElement)element;
            if (te.IsTransitioning)
                return te.CurrentContentPresenter.Content;
            return value;
        }

        private static void OnContentChanged(object element, DependencyPropertyChangedEventArgs e)
        {
            TransitionElement te = (TransitionElement)element;
            te.BeginTransition();
        }

        /// <summary>
        /// 内容模板
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate",
                typeof(DataTemplate),
                typeof(TransitionElement),
                new UIPropertyMetadata(null, OnContentTemplateChanged));

        private static void OnContentTemplateChanged(object element, DependencyPropertyChangedEventArgs e)
        {
            TransitionElement te = (TransitionElement)element;
            te.CurrentContentPresenter.ContentTemplate = (DataTemplate)e.NewValue;
        }

        /// <summary>
        /// 根据数据对象和绑定进行内容选择
        /// </summary>
        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register("ContentTemplateSelector",
                typeof(DataTemplateSelector),
                typeof(TransitionElement),
                new UIPropertyMetadata(null, OnContentTemplateSelectorChanged));

        private static void OnContentTemplateSelectorChanged(object element, DependencyPropertyChangedEventArgs e)
        {
            TransitionElement te = (TransitionElement)element;
            te.CurrentContentPresenter.ContentTemplateSelector = (DataTemplateSelector)e.NewValue;
        }

        /// <summary>
        /// 是否正在进行转换
        /// </summary>
        public bool IsTransitioning
        {
            get { return (bool)GetValue(IsTransitioningProperty); }
            private set { SetValue(IsTransitioningPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsTransitioningPropertyKey =
            DependencyProperty.RegisterReadOnly("IsTransitioning",
                typeof(bool),
                typeof(TransitionElement),
                new UIPropertyMetadata(false));

        public static readonly DependencyProperty IsTransitioningProperty =
            IsTransitioningPropertyKey.DependencyProperty;

        /// <summary>
        /// 当前使用的转换器
        /// </summary>
        public Transition Transition
        {
            get { return (Transition)GetValue(TransitionProperty); }
            set { SetValue(TransitionProperty, value); }
        }

        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.Register("Transition", typeof(Transition), typeof(TransitionElement), 
            new UIPropertyMetadata(null, null, CoerceTransition));

        private static object CoerceTransition(object element, object value)
        {
            TransitionElement te = (TransitionElement)element;
            if (te.IsTransitioning) return te._activeTransition;
            return value;
        }

        /// <summary>
        /// 转换器选择装置
        /// </summary>
        public TransitionSelector TransitionSelector
        {
            get { return (TransitionSelector)GetValue(TransitionSelectorProperty); }
            set { SetValue(TransitionSelectorProperty, value); }
        }

        public static readonly DependencyProperty TransitionSelectorProperty =
            DependencyProperty.Register("TransitionSelector", typeof(TransitionSelector), typeof(TransitionElement), new UIPropertyMetadata(null));

        #endregion

        #region Methods

        /// <summary>
        /// 开始进行转换
        /// </summary>
        private void BeginTransition()
        {
            TransitionSelector selector = TransitionSelector;

            Transition transition = selector != null ?
                selector.SelectTransition(CurrentContentPresenter.Content, Content) :
                Transition;

            if (transition != null)
            {
                // Swap content presenters
                AdornerDecorator temp = _previousHost;
                _previousHost = _currentHost;
                _currentHost = temp;
            }

            ContentPresenter currentContent = CurrentContentPresenter;
            // Set the current content
            currentContent.Content = Content;
            currentContent.ContentTemplate = ContentTemplate;
            currentContent.ContentTemplateSelector = ContentTemplateSelector;

            if (transition != null)
            {
                ContentPresenter previousContent = PreviousContentPresenter;

                if (transition.IsNewContentTopmost)
                    Children.Add(_currentHost);
                else
                    Children.Insert(0, _currentHost);

                IsTransitioning = true;
                _activeTransition = transition;

                CoerceValue(TransitionProperty);
                CoerceValue(ClipToBoundsProperty);

                //使用指定转换器进行转换
                transition.BeginTransition(this, previousContent, currentContent);
            }
        }

        /// <summary>
        /// Clean up after the transition is complete
        /// </summary>
        /// <param name="transition"></param>
        internal void OnTransitionCompleted(Transition transition)
        {
            _children.Clear();
            _children.Add(_currentHost);
            _currentHost.Visibility = Visibility.Visible;
            _previousHost.Visibility = Visibility.Visible;
            ((ContentPresenter)_previousHost.Child).Content = null;

            IsTransitioning = false;
            _activeTransition = null;
            CoerceValue(TransitionProperty);
            CoerceValue(ClipToBoundsProperty);
            CoerceValue(ContentProperty);
        }

        /// <summary>
        /// 包含的UIElement
        /// </summary>
        internal UIElementCollection Children
        {
            get { return _children; }
        }

        /// <summary>
        /// 获取前一个装饰器的Content
        /// </summary>
        private ContentPresenter PreviousContentPresenter
        {
            get { return ((ContentPresenter)_previousHost.Child); }
        }

        /// <summary>
        /// 获取当前装饰器的Content
        /// </summary>
        private ContentPresenter CurrentContentPresenter
        {
            get { return ((ContentPresenter)_currentHost.Child); }
        }

        #endregion

        #region Override Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            _currentHost.Measure(availableSize);
            return _currentHost.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement uie in _children)
                uie.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
                throw new ArgumentException("Bad index.", "index");
            return _children[index];
        }

        #endregion
    }
}
