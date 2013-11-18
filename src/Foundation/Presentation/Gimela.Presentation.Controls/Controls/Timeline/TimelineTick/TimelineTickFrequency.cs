using System;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 时间线刻度频度，用于描述刻度时间间隔，包括周期(秒、分钟、小时、天)，频度和范围(TimeSpan)。
    /// </summary>
    public class TimelineTickFrequency : IComparable<TimelineTickFrequency>
    {
        /// <summary>
        /// 刻度周期
        /// </summary>
        public TimelineTickPeriod Period { get; set; }

        /// <summary>
        /// 刻度频度
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// 长度范围
        /// </summary>
        public TimeSpan Extent
        {
            get
            {
                switch (Period)
                {
                    case TimelineTickPeriod.Seconds:
                        return new TimeSpan(0, 0, Frequency);
                    case TimelineTickPeriod.Minutes:
                        return new TimeSpan(0, Frequency, 0);
                    case TimelineTickPeriod.Hours:
                        return new TimeSpan(Frequency, 0, 0);
                    case TimelineTickPeriod.Days:
                        return new TimeSpan(Frequency, 0, 0, 0);
                }

                // something went wacky, just return an empty time span
                return new TimeSpan();
            }
        }

        #region IComparable<TimelineTickFrequency> Members

        public int CompareTo(TimelineTickFrequency other)
        {
            return this.Extent.CompareTo(other.Extent);
        }

        #endregion
    }
}
