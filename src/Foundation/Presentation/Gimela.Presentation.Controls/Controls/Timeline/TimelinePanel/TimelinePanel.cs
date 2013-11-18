using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 时间线的刻度容器面板，继承自Panel对象。
    /// </summary>
    public partial class TimelinePanel : Panel
    {
        #region Variables

        /// <summary>
        /// 定义无穷大尺寸
        /// </summary>
        private static readonly Size INFINITE_SIZE = new Size(double.PositiveInfinity, double.PositiveInfinity);

        /// <summary>
        /// 默认布局容器
        /// </summary>
        private readonly SimpleTimelineLayoutContainer _defaultLayoutContainer;
        /// <summary>
        /// 时间线布局容器集合
        /// </summary>
        private readonly TimelineLayoutContainerCollection _timelineLayoutContainerCollection;
        /// <summary>
        /// 面板中的层集合, TimelinePanelLayer为TimelinePanel的直接子元素。
        /// </summary>
        private readonly TimelinePanelLayerCollection _timelinePanelLayerCollection;
        
        #endregion

        #region Constructors

        /// <summary>
        /// 时间线的刻度容器面板，继承自Panel对象。
        /// </summary>
        public TimelinePanel()
        {
            _timelineLayoutContainerCollection = new TimelineLayoutContainerCollection(ItemSpacer);
            _timelinePanelLayerCollection = new TimelinePanelLayerCollection();
            _defaultLayoutContainer = new SimpleTimelineLayoutContainer(1, 0, 0);
        }

        #endregion

        #region AttachedProperties

        #region StartDateTime

        /// <summary>
        /// 时间线面板 开始时间，这是一个附加属性。通过附加属性，子元素可继承到该值。
        /// </summary>
        public static readonly DependencyProperty StartDateTimeProperty =
          DependencyProperty.RegisterAttached("StartDateTime", typeof(DateTime), typeof(TimelinePanel), 
          new FrameworkPropertyMetadata(DateTime.MinValue));

        public static DateTime GetStartDateTime(DependencyObject d)
        {
            return (DateTime)d.GetValue(StartDateTimeProperty);
        }

        public static void SetStartDateTime(DependencyObject d, DateTime value)
        {
            d.SetValue(StartDateTimeProperty, value);
        }

        #endregion

        #region EndDateTime

        /// <summary>
        /// 时间线面板 结束时间，这是一个附加属性。通过附加属性，子元素可继承到该值。
        /// </summary>
        public static readonly DependencyProperty EndDateTimeProperty =
          DependencyProperty.RegisterAttached("EndDateTime", typeof(DateTime), typeof(TimelinePanel), 
          new FrameworkPropertyMetadata(DateTime.MaxValue));

        public static DateTime GetEndDateTime(DependencyObject d)
        {
            return (DateTime)d.GetValue(EndDateTimeProperty);
        }

        public static void SetEndDateTime(DependencyObject d, DateTime value)
        {
            d.SetValue(EndDateTimeProperty, value);
        }

        #endregion

        #endregion

        #region DependencyProperties

        #region TimelineStartDateTime

        public static readonly DependencyProperty TimelineStartDateTimeProperty =
            DependencyProperty.Register("TimelineStartDateTime", typeof(DateTime), typeof(TimelinePanel),
                new FrameworkPropertyMetadata(DateTime.MinValue,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnTimelineStartDateTimeChanged,
                    CoerceTimelineStartDateTimeValue));

        /// <summary>
        /// 时间线开始时间，这是一个依赖属性。
        /// </summary>
        public DateTime TimelineStartDateTime
        {
            get { return (DateTime)GetValue(TimelineStartDateTimeProperty); }
            set { SetValue(TimelineStartDateTimeProperty, value); }
        }

        private static void OnTimelineStartDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelinePanel)d).OnTimelineStartDateTimeChanged(e);
        }

        protected virtual void OnTimelineStartDateTimeChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceTimelineStartDateTimeValue(DependencyObject d, object value)
        {
            return value;
        }

        #endregion

        #region TimelineEndDateTime

        public static readonly DependencyProperty TimelineEndDateTimeProperty =
            DependencyProperty.Register("TimelineEndDateTime", typeof(DateTime), typeof(TimelinePanel),
                new FrameworkPropertyMetadata(DateTime.MaxValue,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnTimelineEndDateTimeChanged,
                    CoerceTimelineEndDateTimeValue));

        /// <summary>
        /// 时间线结束时间，这是一个依赖属性。
        /// </summary>
        public DateTime TimelineEndDateTime
        {
            get { return (DateTime)GetValue(TimelineEndDateTimeProperty); }
            set { SetValue(TimelineEndDateTimeProperty, value); }
        }

        private static void OnTimelineEndDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelinePanel)d).OnTimelineEndDateTimeChanged(e);
        }

        protected virtual void OnTimelineEndDateTimeChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceTimelineEndDateTimeValue(DependencyObject d, object value)
        {
            return value;
        }

        #endregion

        #region ItemSpacer

        public static readonly DependencyProperty ItemSpacerProperty =
            DependencyProperty.Register("ItemSpacer", typeof(double), typeof(TimelinePanel),
                new FrameworkPropertyMetadata(5.0,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnItemSpacerChanged));

        /// <summary>
        /// 容器中项的空间值，默认为5.0，这是一个依赖属性。
        /// </summary>
        public double ItemSpacer
        {
            get { return (double)GetValue(ItemSpacerProperty); }
            set { SetValue(ItemSpacerProperty, value); }
        }

        private static void OnItemSpacerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelinePanel)d).OnItemSpacerChanged(e);
        }

        protected virtual void OnItemSpacerChanged(DependencyPropertyChangedEventArgs e)
        {
            _timelineLayoutContainerCollection.ItemSpacerExtent = ItemSpacer;
        }

        #endregion

        #region SortDirection

        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register("SortDirection", typeof(ListSortDirection), typeof(TimelinePanel),
                new FrameworkPropertyMetadata(ListSortDirection.Descending,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 时间线排序方向，这是一个依赖属性。
        /// </summary>
        public ListSortDirection SortDirection
        {
            get { return (ListSortDirection)GetValue(SortDirectionProperty); }
            set { SetValue(SortDirectionProperty, value); }
        }

        #endregion

        #region PixelsPerSecond

        public static readonly DependencyProperty PixelsPerSecondProperty =
            DependencyProperty.Register("PixelsPerSecond", typeof(double), typeof(TimelinePanel),
                new FrameworkPropertyMetadata(double.PositiveInfinity,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnPixelsPerSecondChanged,
                    CoercePixelsPerSecondValue));

        /// <summary>
        /// 时间线每秒像素值，这是一个依赖属性。
        /// </summary>
        public double PixelsPerSecond
        {
            get { return (double)GetValue(PixelsPerSecondProperty); }
            set { SetValue(PixelsPerSecondProperty, value); }
        }

        private static void OnPixelsPerSecondChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelinePanel)d).OnPixelsPerSecondChanged(e);
        }

        protected virtual void OnPixelsPerSecondChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        private static object CoercePixelsPerSecondValue(DependencyObject d, object value)
        {
            return ((TimelinePanel)d).CoercePixelsPerSecondValue(value);
        }

        protected virtual object CoercePixelsPerSecondValue(object value)
        {
            // if we are sizing to fit don't allow the scaling to be adjusted
            //return SizeToFit ? PixelsPerSecond : value;
            return value;
        }

        #endregion

        #region SizeToFit

        public static readonly DependencyProperty SizeToFitProperty =
            DependencyProperty.Register("SizeToFit", typeof(bool), typeof(TimelinePanel),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnSizeToFitChanged,
                    CoerceSizeToFitValue));

        /// <summary>
        /// 控件是否跟随窗体的缩放，这是一个依赖属性。
        /// </summary>
        public bool SizeToFit
        {
            get { return (bool)GetValue(SizeToFitProperty); }
            set { SetValue(SizeToFitProperty, value); }
        }

        private static void OnSizeToFitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelinePanel)d).OnSizeToFitChanged(e);
        }

        protected virtual void OnSizeToFitChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceSizeToFitValue(DependencyObject d, object value)
        {
            return value;
        }

        #endregion

        #endregion

        #region Properties

        #region TimelineRangeSeconds

        /// <summary>
        /// 时间线范围的总秒数
        /// </summary>
        protected double TimelineRangeSeconds
        {
            get { return (TimelineEndDateTime - TimelineStartDateTime).TotalSeconds; }
        }

        #endregion

        #endregion

        #region Override

        /// <summary>
        /// 当在派生类中重写时，请测量子元素在布局中所需的大小，然后确定 FrameworkElement 派生类的大小。
        /// When overridden in a derived class, measures the size in layout required for 
        /// child elements and determines a size for 
        /// the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">
        /// 此元素可以赋给子元素的可用大小。 可以指定无穷大值，这表示元素的大小将调整为内容的可用大小。 
        /// The available size that this element can give to 
        /// child elements. Infinity can be specified as a value to indicate that the element 
        /// will size to whatever content is available.</param>
        /// <returns>
        /// 此元素在布局过程中所需的大小，这是由此元素根据对其子元素大小的计算而确定的。
        /// The size that this element determines it needs during layout, 
        /// based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            // 重写 MeasureOverride，以便在元素参与 WPF 布局系统时实现元素的自定义布局大小行为。
            var measureRect = new Rect(0.0, 0.0, 0.0, 0.0);

            if (SizeToFit)
            {
                // we need to calculate the scaling factor
                // depends on orienation
                PixelsPerSecond =
                  CalculatePixelsPerSecond(
                    TimelineRangeSeconds,
                    double.IsPositiveInfinity(availableSize.Width) ? int.MaxValue : availableSize.Width);
            }
            else
            {
                measureRect.Width = TimelineRangeSeconds * PixelsPerSecond;
            }

            if (!double.IsInfinity(PixelsPerSecond))
            {
                // should have some type of value for scale by this point
                _timelineLayoutContainerCollection.Clear();
                _timelinePanelLayerCollection.ResetChildData(InternalChildren);
                _timelineLayoutContainerCollection.SecondsScalingFactor = PixelsPerSecond;

                // measure each layer seperately
                foreach (TimelinePanelLayer layer in _timelinePanelLayerCollection.Layers)
                {
                    foreach (TimelinePanelChild child in layer.Children)
                    {
                        // this might not be right for durations, we may want to constrain here
                        // so that we can do row layout correctly at this stage
                        if (!child.IsDuration || availableSize == INFINITE_SIZE)
                        {
                            child.Visual.Measure(INFINITE_SIZE);
                        }
                        else
                        {
                            // logic here could be trimmed for just duration cases, keeping the same for now

                            // we can position the child.X based on .Start
                            child.LayoutRect.X =
                              child.Start.ToPosition(
                                TimelineStartDateTime,
                                PixelsPerSecond
                                );

                            // height is always set to what the child wants
                            child.LayoutRect.Height = INFINITE_SIZE.Height; //child.Visual.DesiredSize.Height;

                            if (child.IsDuration)
                            {
                                // it's a duration so set the width based on endTime time
                                child.LayoutRect.Width =
                                  child.End.Value.ToPosition(
                                    TimelineStartDateTime,
                                    PixelsPerSecond
                                    );

                                // Y value may not be 0.0 if we are wrapping children
                                if (child.IsWrapping)
                                {
                                    // duration wrapping layout is done in the layout containers
                                    _timelineLayoutContainerCollection.AddChildToLayoutContainers(child);
                                }
                            }
                            else
                            {
                                // it's not a duration, so let the child tell us what its width should be
                                child.LayoutRect.Width = INFINITE_SIZE.Width; //child.Visual.DesiredSize.Width;
                            }

                            // call the child's measure with the rect we have
                            child.Visual.Measure(child.LayoutRect.Size);
                        }

                        // expand our rect to a big enough size to hold this child
                        //measureRect.Union(new Point(child.Visual.DesiredSize.Height, child.Visual.DesiredSize.Width));
                        measureRect.Height += child.Visual.DesiredSize.Height + ItemSpacer;
                    }
                }

                // expand our rect to a big enough size to hold all of the layout containers
                measureRect.Union(_timelineLayoutContainerCollection.LayoutRect);
            }
            else
            {
                measureRect.Height = measureRect.Width = int.MaxValue;
            }

            return measureRect.Size;
        }

        /// <summary>
        /// 在派生类中重写时，请为 FrameworkElement 派生类定位子元素并确定大小。
        /// When overridden in a derived class, positions child elements and determines 
        /// a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">
        /// 父级中此元素应用来排列自身及其子元素的最终区域。
        /// The final area within the parent that this element 
        /// should use to arrange itself and its children.</param>
        /// <returns>
        /// 所用的实际大小。
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // 要自定义布局处理的排列处理过程的控件作者应重写此方法。

            if (SizeToFit)
            {
                // we need to calculate the scaling factor
                // depends on orienation
                PixelsPerSecond = CalculatePixelsPerSecond(TimelineRangeSeconds, finalSize.Width);
            }
            else
            {
                finalSize.Width = TimelineRangeSeconds * PixelsPerSecond;
            }

            if (!double.IsInfinity(PixelsPerSecond))
            {
                // can we reuse the measure stuff? check UIElementCollection some way?
                _timelineLayoutContainerCollection.Clear();

                // depends on orienation
                _timelineLayoutContainerCollection.SecondsScalingFactor = PixelsPerSecond;
                _timelinePanelLayerCollection.ResetChildData(InternalChildren);

                // arrange each layer seperately
                foreach (TimelinePanelLayer layer in _timelinePanelLayerCollection.Layers)
                {
                    foreach (TimelinePanelChild child in layer.Children)
                    {
                        // we can position the child.X based on .Start and alignment
                        // depends on orientation
                        child.LayoutRect.X = LayoutChild(child);

                        // height is always set to what the child wants
                        child.LayoutRect.Height = child.Visual.DesiredSize.Height;

                        if (child.IsDuration)
                        {
                            if (child.Alignment == HorizontalAlignment.Center)
                            {
                                // child is centered, so we will go with the widht of the visual and not the duration
                                child.LayoutRect.Width = child.Visual.DesiredSize.Width;
                            }
                            else
                            {
                                // it's a duration so set the width based on endTime time
                                double testWidth =
                                  child.End.Value.ToPosition(
                                    TimelineStartDateTime,
                                    PixelsPerSecond
                                    ) - child.LayoutRect.X;

                                // set min width
                                int minWidth = 30;

                                // a negative width here means it is completely outside of clip bounds and we don't have to arrange it
                                //if(testWidth <= 0)
                                //  break;
                                child.LayoutRect.Width = (testWidth > minWidth) ? testWidth : minWidth;
                            }

                            // Y value may not be 0.0 if we are wrapping children
                            if (child.IsWrapping)
                            {
                                // duration wrapping layout is done in the layout containers
                                _timelineLayoutContainerCollection.AddChildToLayoutContainers(child);
                            }
                        }
                        else
                        {
                            // it's not a duration, so let the child tell us what its width should be
                            child.LayoutRect.Width = child.Visual.DesiredSize.Width;

                            // call the child's arrange with the rect we have
                            child.Visual.Arrange(child.LayoutRect);
                        }
                    }
                }

                // now do visuals arrange
                _timelineLayoutContainerCollection.AdjustLayoutContainers();
                foreach (TimelinePanelChild child in _timelineLayoutContainerCollection.Children)
                {
                    child.Visual.Arrange(child.LayoutRect);
                }
            }

            return finalSize;
        }

        #endregion

        #region Methods

        private double LayoutChild(TimelinePanelChild child)
        {
            double returnValue =
              child.Start.ToPosition(
                TimelineStartDateTime,
                PixelsPerSecond
                );


            if (child.Visual is FrameworkElement)
            {
                var visual = child.Visual as FrameworkElement;
                child.Alignment = visual.HorizontalAlignment;
                switch (child.Alignment)
                {
                    case HorizontalAlignment.Center:
                        returnValue -= visual.DesiredSize.Width / 2.0;
                        break;
                    case HorizontalAlignment.Right:
                        returnValue -= visual.DesiredSize.Width;
                        break;
                    case HorizontalAlignment.Stretch:
                    case HorizontalAlignment.Left:
                        break;
                }
            }

            return returnValue;
        }

        private static double DateToPosition(DateTime dateTime, DateTime start, double rangeInSeconds, double range)
        {
            return (dateTime - start).TotalSeconds * CalculatePixelsPerSecond(rangeInSeconds, range);
        }

        /// <summary>
        /// 根据时间线范围总秒数和当前宽度计算每秒的像素数
        /// </summary>
        /// <param name="rangeInSeconds">时间线范围总秒数</param>
        /// <param name="range">当前宽度</param>
        /// <returns>每秒的像素数</returns>
        private static double CalculatePixelsPerSecond(double rangeInSeconds, double range)
        {
            return range / rangeInSeconds;
        }

        #endregion
    }
}
