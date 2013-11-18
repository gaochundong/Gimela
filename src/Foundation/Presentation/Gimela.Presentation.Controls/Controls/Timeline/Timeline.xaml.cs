using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// Interaction logic for Timeline.xaml
    /// </summary>
    public partial class Timeline : TimelineBase
    {
        #region Variables

        private bool isLoadedImplementation;
        private double targetLeftSeconds;
        private double targetRightSeconds;
        private readonly TimeSpan minZoomTimeSpan = new TimeSpan(0, 10, 0);  // 10 minutes
        private TimeSpan maxZoomTimeSpan = new TimeSpan(24, 0, 0); // 24 hours - temporarily
        private double oldZoomPercent;

        #endregion

        #region Ctor

        public Timeline()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private ScrollViewer topTickScrollViewer;
        /// <summary>
        /// 时间线顶端刻度的滚动条
        /// </summary>
        private ScrollViewer TopTickScrollViewer
        {
            get
            {
                if (topTickScrollViewer == null)
                {
                    var listBox = VisualTreeExtraHelper.FindVisualChildByName<ListBox>(ScrollViewer, "topTicks");
                    topTickScrollViewer = VisualTreeExtraHelper.FindVisualChild<ScrollViewer>(listBox);
                }

                return topTickScrollViewer;
            }
        }

        private ScrollViewer longBigTickScrollViewer;
        /// <summary>
        /// 时间线长大刻度的滚动条
        /// </summary>
        private ScrollViewer LongBigTickScrollViewer
        {
            get
            {
                if (longBigTickScrollViewer == null)
                {
                    var listBox = VisualTreeExtraHelper.FindVisualChildByName<ListBox>(ScrollViewer, "longBigTicks");
                    longBigTickScrollViewer = VisualTreeExtraHelper.FindVisualChild<ScrollViewer>(listBox);
                }

                return longBigTickScrollViewer;
            }
        }

        private ScrollViewer longMediumTickScrollViewer;
        /// <summary>
        /// 时间线长中刻度的滚动条
        /// </summary>
        private ScrollViewer LongMediumTickScrollViewer
        {
            get
            {
                if (longMediumTickScrollViewer == null)
                {
                    var listBox = VisualTreeExtraHelper.FindVisualChildByName<ListBox>(ScrollViewer, "longMediumTicks");
                    longMediumTickScrollViewer = VisualTreeExtraHelper.FindVisualChild<ScrollViewer>(listBox);
                }

                return longMediumTickScrollViewer;
            }
        }

        private ScrollViewer longSmallTickScrollViewer;
        /// <summary>
        /// 时间线长小刻度的滚动条
        /// </summary>
        private ScrollViewer LongSmallTickScrollViewer
        {
            get
            {
                if (longSmallTickScrollViewer == null)
                {
                    var listBox = VisualTreeExtraHelper.FindVisualChildByName<ListBox>(ScrollViewer, "longSmallTicks");
                    longSmallTickScrollViewer = VisualTreeExtraHelper.FindVisualChild<ScrollViewer>(listBox);
                }

                return longSmallTickScrollViewer;
            }
        }

        private ScrollViewer scrollViewer;
        /// <summary>
        /// 时间线主窗体的滚动条
        /// </summary>
        public ScrollViewer ScrollViewer
        {
            get
            {
                if (scrollViewer == null)
                {
                    // 在虚拟树种找到该节点
                    scrollViewer = VisualTreeExtraHelper.FindVisualChild<ScrollViewer>(timelineListBox);

                    if (scrollViewer != null)
                    {
                        scrollViewer.ScrollChanged += new ScrollChangedEventHandler(OnScrollViewerScrollChanged);
                    }
                }

                return scrollViewer;
            }
        }

        #endregion

        #region ScrollViewer

        /// <summary>
        /// 时间线自定义的滚动条，将其变化与时间线顶端内容联动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange != 0)
            {
                OnHorizontalOffsetChange();
            }

            if (e.ViewportWidthChange != 0)
            {
                OnViewportWidthChange();
            }

            if (e.ExtentWidthChange != 0)
            {
                OnExtentWidthChange();
            }
        }

        /// <summary>
        /// 滚动条水平偏移时发生
        /// </summary>
        private void OnHorizontalOffsetChange()
        {
            TopTickScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset);
            LongBigTickScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset);
            LongMediumTickScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset);
            LongSmallTickScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset);
            TickCollection.ViewportOffset = ScrollViewer.HorizontalOffset;
        }

        /// <summary>
        /// 视角宽度变化时发生
        /// </summary>
        private void OnViewportWidthChange()
        {
            TickCollection.ViewportExtent = ScrollViewer.ViewportWidth;

            if (ScrollViewer != null)
            {
                ZoomPercent = Math.Round(ScrollViewer.ViewportWidth / (Zoom * maxZoomTimeSpan.TotalSeconds) * 100, 2);
            }
        }

        /// <summary>
        /// 范围宽度变化时发生
        /// </summary>
        private void OnExtentWidthChange()
        {
            if (oldZoomPercent == 0)
            {
                return;
            }

            if (oldZoomPercent != ZoomPercent)
            {
                //the fumula to keep the previous center point is still on the center of the viewport when the extent is changed.
                double offset = (scrollViewer.HorizontalOffset + scrollViewer.ViewportWidth / 2) * oldZoomPercent / ZoomPercent - scrollViewer.ViewportWidth / 2;
                scrollViewer.ScrollToHorizontalOffset(offset);
                oldZoomPercent = ZoomPercent;

                // 如果要将中间的内容滚动，可能要修改这里
            }
        }

        #endregion

        #region OnRender

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that 
        /// are directed by the layout system. The rendering instructions for this element 
        /// are not used directly when this method is invoked, and are instead preserved 
        /// for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. 
        /// This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (IsLoaded && !isLoadedImplementation)
            {
                LoadedImplementation();
            }

            base.OnRender(drawingContext);

            TickCollection.ViewportExtent = ActualWidth;
        }

        /// <summary>
        /// 加载相关实现
        /// </summary>
        public void LoadedImplementation()
        {
            // check to make sure we have a ScrollViewer
            if (this.ScrollViewer != null)
            {
                isLoadedImplementation = true;
            }
            else
            {
                return;
            }

            // 将滚动条缩放视角绑定到方法
            CommandBindings.Add(new CommandBinding(TimelineZoomScrollBar.ZoomViewportCommand, ZoomViewportCommandExecute));

            // initialize the tick collection
            TickCollection.ViewportOffset = ScrollViewer.HorizontalOffset;
            TickCollection.ViewportExtent = ScrollViewer.ViewportWidth;

            // initialize zoom percent      
            targetRightSeconds = (this.End - this.Start).TotalSeconds;  // try to force it to the right      
        }

        /// <summary>
        /// 滚动条缩放视角缩放方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomViewportCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            var param = e.Parameter as TimelineZoomViewportParameters;
            if (param != null)
            {
                // find the target offset using the anchor parameter
                if (param.ViewportAnchor == TimelineZoomAnchor.Right)
                {
                    targetRightSeconds = (ScrollViewer.HorizontalOffset + ScrollViewer.ViewportWidth) * (1 / Zoom);
                }
                else
                {
                    targetLeftSeconds = ScrollViewer.HorizontalOffset * (1 / Zoom);
                }

                Zoom *= param.ViewportScale;
            }
        }

        #endregion

        #region Overrides

        protected override void OnStartChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnStartChanged(e);

            if (Start < End)
            {
                maxZoomTimeSpan = End - Start;
            }
        }

        protected override void OnEndChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnEndChanged(e);

            if (Start < End)
            {
                maxZoomTimeSpan = End - Start;
            }
        }

        protected override void OnZoomChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnZoomChanged(e);

            if (ScrollViewer != null)
            {
                ZoomPercent = Math.Round(ScrollViewer.ViewportWidth / (Zoom * maxZoomTimeSpan.TotalSeconds) * 100, 2);
            }
        }

        protected override void OnZoomPercentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ScrollViewer != null)
            {
                oldZoomPercent = (double)e.OldValue;
                Zoom = Math.Round(ScrollViewer.ViewportWidth / (maxZoomTimeSpan.TotalSeconds * ZoomPercent / 100), 8);
            }
        }

        #endregion
    }
}
