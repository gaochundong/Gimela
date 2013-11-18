using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 时间线刻度集合，继承自DependencyObject对象
    /// </summary>
    public class TimelineTickCollection : DependencyObject, INotifyPropertyChanged
    {
        /// <summary>
        /// 刻度集合
        /// </summary>
        private readonly ObservableCollection<TimelineTick> _ticks;
        /// <summary>
        /// 刻度查询字典，时间：刻度
        /// </summary>
        private readonly Dictionary<DateTime, TimelineTick> _ticksLookup;
        /// <summary>
        /// 刻度频度
        /// </summary>
        private readonly List<TimelineTickFrequency> _validFrequencies;

        /// <summary>
        /// 时间线刻度集合，继承自DependencyObject对象
        /// </summary>
        public TimelineTickCollection()
        {
            _ticks = new ObservableCollection<TimelineTick>();
            _ticksLookup = new Dictionary<DateTime, TimelineTick>();
            _validFrequencies = new List<TimelineTickFrequency>();

            //_validFrequencies.Add(new TimelineTickFrequency() { Frequency = 1, Period = TimelineTickPeriod.Seconds });
            //_validFrequencies.Add(new TimelineTickFrequency() { Frequency = 5, Period = TimelineTickPeriod.Seconds });
            //_validFrequencies.Add(new TimelineTickFrequency() { Frequency = 10, Period = TimelineTickPeriod.Seconds });
            //_validFrequencies.Add(new TimelineTickFrequency() { Frequency = 15, Period = TimelineTickPeriod.Seconds });
            //_validFrequencies.Add(new TimelineTickFrequency() { Frequency = 30, Period = TimelineTickPeriod.Seconds });

            _validFrequencies.Add(new TimelineTickFrequency() { Frequency = 1, Period = TimelineTickPeriod.Minutes });
            _validFrequencies.Add(new TimelineTickFrequency() { Frequency = 5, Period = TimelineTickPeriod.Minutes });
            _validFrequencies.Add(new TimelineTickFrequency() { Frequency = 10, Period = TimelineTickPeriod.Minutes });
            _validFrequencies.Add(new TimelineTickFrequency() { Frequency = 15, Period = TimelineTickPeriod.Minutes });
            _validFrequencies.Add(new TimelineTickFrequency() { Frequency = 30, Period = TimelineTickPeriod.Minutes });

            _validFrequencies.Add(new TimelineTickFrequency() { Frequency = 1, Period = TimelineTickPeriod.Hours });
            _validFrequencies.Add(new TimelineTickFrequency() { Frequency = 2, Period = TimelineTickPeriod.Hours });
            _validFrequencies.Add(new TimelineTickFrequency() { Frequency = 4, Period = TimelineTickPeriod.Hours });
            _validFrequencies.Add(new TimelineTickFrequency() { Frequency = 6, Period = TimelineTickPeriod.Hours });

            _validFrequencies.Sort();

            BigTickMinimumSize = 100;
            MediumTickMinimumSize = 20;
            SmallTickMinimumSize = 4;
        }

        #region Properties

        /// <summary>
        /// 大刻度递归列举
        /// </summary>
        public IEnumerable<TimelineTick> BigTicks
        {
            get
            {
                foreach (var tick in Ticks)
                {
                    if (tick.TickType == TimelineTickType.Big)
                        yield return tick;
                }
            }
        }

        /// <summary>
        /// 中刻度递归列举
        /// </summary>
        public IEnumerable<TimelineTick> MediumTicks
        {
            get
            {
                foreach (var tick in Ticks)
                {
                    if (tick.TickType == TimelineTickType.Medium)
                        yield return tick;
                }
            }
        }

        /// <summary>
        /// 小刻度递归列举
        /// </summary>
        public IEnumerable<TimelineTick> SmallTicks
        {
            get
            {
                foreach (var tick in Ticks)
                {
                    if (tick.TickType == TimelineTickType.Small)
                        yield return tick;
                }
            }
        }

        /// <summary>
        /// 刻度集合
        /// </summary>
        public ObservableCollection<TimelineTick> Ticks { get { return _ticks; } }

        /// <summary>
        /// 是否自动生成刻度集合
        /// </summary>
        public bool AutoGenerateTicks { get; set; }

        /// <summary>
        /// 大刻度 刻度频度
        /// </summary>
        public TimelineTickFrequency BigTickFrequency { get; set; }

        /// <summary>
        /// 中刻度 刻度频度
        /// </summary>
        public TimelineTickFrequency MediumTickFrequency { get; set; }

        /// <summary>
        /// 小刻度 刻度频度
        /// </summary>
        public TimelineTickFrequency SmallTickFrequency { get; set; }

        /// <summary>
        /// 大刻度 最小值 可能值为100
        /// </summary>
        public double BigTickMinimumSize { get; set; }

        /// <summary>
        /// 中刻度 最小值 可能值为20
        /// </summary>
        public double MediumTickMinimumSize { get; set; }

        /// <summary>
        /// 小刻度 最小值 可能值为4/6
        /// </summary>
        public double SmallTickMinimumSize { get; set; }

        #endregion

        #region Dependency Properties

        #region ViewportOffset

        /// <summary>
        /// ViewportOffset Dependency Property
        /// </summary>
        public static readonly DependencyProperty ViewportOffsetProperty =
            DependencyProperty.Register("ViewportOffset", typeof(double), typeof(TimelineTickCollection),
                new FrameworkPropertyMetadata((double)0.0,
                    new PropertyChangedCallback(OnViewportOffsetChanged)));

        /// <summary>
        /// 视口偏移，用于秒数开始时间与视口时间间的长度，这是一个依赖属性。
        /// </summary>
        public double ViewportOffset
        {
            get { return (double)GetValue(ViewportOffsetProperty); }
            set { SetValue(ViewportOffsetProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ViewportOffset property.
        /// </summary>
        private static void OnViewportOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelineTickCollection)d).OnViewportOffsetChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ViewportOffset property.
        /// </summary>
        protected virtual void OnViewportOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!double.IsInfinity(ViewportExtent))
            {
                ReCalculateTimelineTicks();
            }
        }

        #endregion

        #region ViewportExtent

        /// <summary>
        /// ViewportExtent Dependency Property
        /// </summary>
        public static readonly DependencyProperty ViewportExtentProperty =
            DependencyProperty.Register("ViewportExtent", typeof(double), typeof(TimelineTickCollection),
                new FrameworkPropertyMetadata((double)double.PositiveInfinity,
                    new PropertyChangedCallback(OnViewportExtentChanged)));

        /// <summary>
        /// 视口范围，用于在开始和结束时间中描述一个范围，这是一个依赖属性。
        /// </summary>
        public double ViewportExtent
        {
            get { return (double)GetValue(ViewportExtentProperty); }
            set { SetValue(ViewportExtentProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ViewportExtent property.
        /// </summary>
        private static void OnViewportExtentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelineTickCollection)d).OnViewportExtentChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ViewportExtent property.
        /// </summary>
        protected virtual void OnViewportExtentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!double.IsInfinity(ViewportExtent))
            {
                ReCalculateTimelineTicks();
            }
        }

        #endregion

        #region PixelsPerSecond

        /// <summary>
        /// PixelsPerSecond Dependency Property
        /// </summary>
        public static readonly DependencyProperty PixelsPerSecondProperty =
            DependencyProperty.Register("PixelsPerSecond", typeof(double), typeof(TimelineTickCollection),
                new FrameworkPropertyMetadata((double)double.PositiveInfinity,
                    new PropertyChangedCallback(OnPixelsPerSecondChanged)));

        /// <summary>
        /// 每秒像素值，可与Zoom范围关联，这是一个依赖属性。
        /// </summary>
        public double PixelsPerSecond
        {
            get { return (double)GetValue(PixelsPerSecondProperty); }
            set { SetValue(PixelsPerSecondProperty, value); }
        }

        /// <summary>
        /// Handles changes to the PixelsPerSecond property.
        /// </summary>
        private static void OnPixelsPerSecondChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimelineTickCollection)d).OnPixelsPerSecondChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the PixelsPerSecond property.
        /// </summary>
        protected virtual void OnPixelsPerSecondChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!double.IsInfinity(PixelsPerSecond))
            {
                // 如果需要定制粒度则在这里过滤

                ReCalculateTimelineTicks();
            }
        }

        #endregion

        #region Start

        /// <summary>
        /// Start Dependency Property
        /// </summary>
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(DateTime), typeof(TimelineTickCollection),
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
            ((TimelineTickCollection)d).OnStartChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Start property.
        /// </summary>
        protected virtual void OnStartChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Start != DateTime.MinValue)
            {
                ReCalculateTimelineTicks();
            }
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
            DependencyProperty.Register("End", typeof(DateTime), typeof(TimelineTickCollection),
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
            ((TimelineTickCollection)d).OnEndChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the End property.
        /// </summary>
        protected virtual void OnEndChanged(DependencyPropertyChangedEventArgs e)
        {
            if (End != DateTime.MaxValue)
            {
                ReCalculateTimelineTicks();
            }
        }

        /// <summary>
        /// Coerces the End value.
        /// </summary>
        private static object CoerceEndValue(DependencyObject d, object value)
        {
            return value;
        }

        #endregion

        #endregion

        /// <summary>
        /// 重新计算时间线刻度集合
        /// </summary>
        public void ReCalculateTimelineTicks()
        {
            if (!double.IsInfinity(PixelsPerSecond) && PixelsPerSecond > 0 && !double.IsInfinity(ViewportExtent))
            {
                ClearTicks();

                var currentFrequency = ReCalculateTimelineTickFrequency(TimelineTickType.Big);
                if (currentFrequency != null)
                {
                    CalculateTicks(currentFrequency, TimelineTickType.Big);
                    OnPropertyChanged("BigTicks");

                    currentFrequency = ReCalculateTimelineTickFrequency(TimelineTickType.Medium);
                    if (currentFrequency != null)
                    {
                        CalculateTicks(currentFrequency, TimelineTickType.Medium);
                        OnPropertyChanged("MediumTicks");

                        currentFrequency = ReCalculateTimelineTickFrequency(TimelineTickType.Small);
                        if (currentFrequency != null)
                        {
                            CalculateTicks(currentFrequency, TimelineTickType.Small);
                            OnPropertyChanged("SmallTicks");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据刻度频度和刻度类型计算刻度集合
        /// </summary>
        /// <param name="freq">刻度频度</param>
        /// <param name="tickType">刻度类型</param>
        private void CalculateTicks(TimelineTickFrequency freq, TimelineTickType tickType)
        {
            // if there is a viewport defined, then just generate
            double spanExtentInSeconds; // 跨度范围 秒数
            DateTime spanStart; // 跨度范围 开始时间

            if (double.IsInfinity(ViewportExtent))
            {
                spanExtentInSeconds = (End - Start).TotalSeconds;
                spanStart = Start;
            }
            else
            {
                spanStart = Start.AddSeconds(ViewportOffset * (1 / PixelsPerSecond));
                spanExtentInSeconds = ViewportExtent * (1 / PixelsPerSecond);
            }

            double tickCount = spanExtentInSeconds / freq.Extent.TotalSeconds;

            // calculate the first tick
            var tick =
              DateTime.MinValue.AddSeconds(
                Math.Ceiling((spanStart - DateTime.MinValue).TotalSeconds / freq.Extent.TotalSeconds) * freq.Extent.TotalSeconds
              );

            for (int i = 0; i < tickCount; i++)
            {
                AddTick(
                    new TimelineTick()
                    {
                        Time = tick,
                        TickType = tickType
                    }
                  );

                tick = tick.AddSeconds(freq.Extent.TotalSeconds);
            }

        }

        /// <summary>
        /// 根据刻度类型重新计算时间线刻度频度
        /// </summary>
        /// <param name="tickType">刻度类型</param>
        /// <returns>刻度频度</returns>
        private TimelineTickFrequency ReCalculateTimelineTickFrequency(TimelineTickType tickType)
        {
            // make separate passes for each tick size
            var tickPixelExtentInSeconds = GetSpecifiedMinimumSizeByTickType(tickType) * (1 / PixelsPerSecond);

            // find a tick frequency that fits with this size
            foreach (var validFrequency in _validFrequencies)
            {
                if (validFrequency.Extent.TotalSeconds >= tickPixelExtentInSeconds)
                {
                    return validFrequency;
                }
            }

            // couldn't find a tick frequency, could switch to N number of days here
            return null;
        }

        /// <summary>
        /// 获取指定刻度类型的最小值
        /// </summary>
        /// <param name="tickType">刻度类型</param>
        /// <returns>指定刻度类型的最小值</returns>
        private double GetSpecifiedMinimumSizeByTickType(TimelineTickType tickType)
        {
            switch (tickType)
            {
                case TimelineTickType.Big:
                    return BigTickMinimumSize;
                case TimelineTickType.Medium:
                    return MediumTickMinimumSize;
                case TimelineTickType.Small:
                    return SmallTickMinimumSize;
            }

            return double.NaN;
        }

        /// <summary>
        /// 新增一刻度至集合中
        /// </summary>
        /// <param name="tick">刻度</param>
        private void AddTick(TimelineTick tick)
        {
            // make sure we don't already have a tick with a bigger size at that time
            // separate aggregated lists would be better down the road
            if (!_ticksLookup.ContainsKey(tick.Time))
            {
                _ticks.Add(tick);
                _ticksLookup.Add(tick.Time, tick);
            }
        }

        /// <summary>
        /// 清除所有刻度
        /// </summary>
        private void ClearTicks()
        {
            _ticks.Clear();
            _ticksLookup.Clear();
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a property value changes.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
