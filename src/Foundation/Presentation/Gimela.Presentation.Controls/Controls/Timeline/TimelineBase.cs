using System;
using System.Windows;
using System.Windows.Controls;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 时间线基类，继承自UserControl控件。
    /// </summary>
    public class TimelineBase : UserControl
    {
        #region Ctor

        /// <summary>
        /// 时间刻度集合
        /// </summary>
        private readonly TimelineTickCollection _tickCollection;

        /// <summary>
        /// 时间线基类
        /// </summary>
        public TimelineBase()
        {
            _tickCollection = new TimelineTickCollection();

            TickCollection.ViewportOffset = 0;
            TickCollection.ViewportExtent = ActualWidth;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 时间刻度集合
        /// </summary>
        public TimelineTickCollection TickCollection { get { return _tickCollection; } }

        #endregion

        #region Dependency Properties

        #region Now

        /// <summary>
        /// Now Dependency Property
        /// </summary>
        public static readonly DependencyProperty NowProperty =
            DependencyProperty.Register("Now", typeof(DateTime), typeof(TimelineBase),
                new FrameworkPropertyMetadata((DateTime)DateTime.Now,
                    new PropertyChangedCallback(OnNowChanged)));

        /// <summary>
        /// 时间线中的当前时间，这是一个依赖属性。
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
            ((TimelineBase)d).OnNowChanged(e);
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
            DependencyProperty.Register("Start", typeof(DateTime), typeof(TimelineBase),
                new FrameworkPropertyMetadata((DateTime)DateTime.MinValue,
                    new PropertyChangedCallback(OnStartChanged),
                    new CoerceValueCallback(CoerceStartValue)));

        /// <summary>
        /// 时间线的开始时间，这是一个依赖属性。
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
            ((TimelineBase)d).OnStartChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Start property.
        /// </summary>
        protected virtual void OnStartChanged(DependencyPropertyChangedEventArgs e)
        {
            _tickCollection.Start = Start;
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
            DependencyProperty.Register("End", typeof(DateTime), typeof(TimelineBase),
                new FrameworkPropertyMetadata((DateTime)DateTime.MaxValue,
                    new PropertyChangedCallback(OnEndChanged),
                    new CoerceValueCallback(CoerceEndValue)));

        /// <summary>
        /// 时间线的结束时间，这是一个依赖属性。
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
            ((TimelineBase)d).OnEndChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the End property.
        /// </summary>
        protected virtual void OnEndChanged(DependencyPropertyChangedEventArgs e)
        {
            _tickCollection.End = End;
        }

        /// <summary>
        /// Coerces the End value.
        /// </summary>
        private static object CoerceEndValue(DependencyObject d, object value)
        {
            return value;
        }

        #endregion

        #region Zoom

        /// <summary>
        /// Zoom Dependency Property
        /// </summary>
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double), typeof(TimelineBase),
                new FrameworkPropertyMetadata((double)double.PositiveInfinity,
                    new PropertyChangedCallback(OnZoomChanged),
                    new CoerceValueCallback(CoerceZoomValue)));

        /// <summary>
        /// 时间线的缩放，这是一个依赖属性。
        /// </summary>
        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Zoom property.
        /// </summary>
        private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelineBase)d).OnZoomChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Zoom property.
        /// </summary>
        protected virtual void OnZoomChanged(DependencyPropertyChangedEventArgs e)
        {
            _tickCollection.PixelsPerSecond = Zoom;
        }

        /// <summary>
        /// Coerces the Zoom value.
        /// </summary>
        private static object CoerceZoomValue(DependencyObject d, object value)
        {
            return ((TimelineBase)d).OnCoerceZoomValue(value);
        }

        /// <summary>
        /// Coerces the Zoom value.
        /// </summary>
        protected virtual object OnCoerceZoomValue(object value)
        {
            return value;
        }

        #endregion

        #region ZoomPercent

        /// <summary>
        /// ZoomPercent Dependency Property
        /// </summary>
        public static readonly DependencyProperty ZoomPercentProperty =
            DependencyProperty.Register("ZoomPercent", typeof(double), typeof(TimelineBase),
                new FrameworkPropertyMetadata((double)0,
                    new PropertyChangedCallback(OnZoomPercentChanged),
                    new CoerceValueCallback(CoerceZoomPercentValue)));

        /// <summary>
        /// 时间线的缩放百分比，这是一个依赖属性。
        /// </summary>
        public double ZoomPercent
        {
            get { return (double)GetValue(ZoomPercentProperty); }
            set { SetValue(ZoomPercentProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ZoomPercent property.
        /// </summary>
        private static void OnZoomPercentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelineBase)d).OnZoomPercentChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ZoomPercent property.
        /// </summary>
        protected virtual void OnZoomPercentChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Coerces the ZoomPercent value.
        /// </summary>
        private static object CoerceZoomPercentValue(DependencyObject d, object value)
        {
            return ((TimelineBase)d).OnCoerceZoomPercentValue(value);
        }

        protected virtual object OnCoerceZoomPercentValue(object value)
        {
            if (value is double)
            {
                // bounds check
                var doubleValue = (double)value;
                return (doubleValue >= 0 && doubleValue <= 100) ? doubleValue : ZoomPercent;
            }

            return value;
        }

        #endregion

        #region CanBringIntoView

        /// <summary>
        /// CanBringIntoView Dependency Property
        /// </summary>
        public static readonly DependencyProperty CanBringIntoViewProperty =
            DependencyProperty.Register("CanBringIntoView", typeof(bool), typeof(TimelineBase),
                new FrameworkPropertyMetadata((bool)true));

        /// <summary>
        /// 是否能够将此元素放入其所在的任何可滚动区域内的视图中，这是一个依赖属性。
        /// </summary>
        public bool CanBringIntoView
        {
            get { return (bool)GetValue(CanBringIntoViewProperty); }
            set { SetValue(CanBringIntoViewProperty, value); }
        }

        #endregion

        #endregion
    }
}
