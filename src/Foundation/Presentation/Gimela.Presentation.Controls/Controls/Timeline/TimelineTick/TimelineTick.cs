using System;
using System.Windows;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 时间刻度，继承自依赖对象。
    /// </summary>
    public class TimelineTick : DependencyObject, IComparable, IComparable<TimelineTick>
    {
        private DateTime time;

        /// <summary>
        /// 时间刻度，继承自依赖对象。
        /// </summary>
        public TimelineTick()
        {
        }

        /// <summary>
        /// 时间刻度类型，该属性可用于数据模板选择器
        /// </summary>
        public TimelineTickType TickType { get; set; }

        /// <summary>
        /// 刻度所对应的时间，精确到秒
        /// </summary>
        public DateTime Time
        {
            get { return time; }
            set
            {
                time = value.ToPrecision(DateTimePrecision.Seconds);
            }
        }

        #region IComparable<TimelineTick> Members

        public int CompareTo(TimelineTick other)
        {
            return Time.CompareTo(other.Time);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is TimelineTick)
            {
                return this.CompareTo(obj as TimelineTick);
            }

            throw new NotImplementedException();
        }

        #endregion
    }
}
