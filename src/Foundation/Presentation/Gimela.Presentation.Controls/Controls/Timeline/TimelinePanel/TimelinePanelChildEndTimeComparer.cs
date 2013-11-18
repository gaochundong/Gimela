using System.Collections.Generic;
using System.ComponentModel;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 元素结束时间比较器
    /// </summary>
    internal class TimelinePanelChildEndTimeComparer : IComparer<TimelinePanelChild>
    {
        /// <summary>
        /// 元素结束时间比较器
        /// </summary>
        public TimelinePanelChildEndTimeComparer()
        {
            SortDirection = ListSortDirection.Descending;
        }

        /// <summary>
        /// 指定排序方向，默认为降序排序。
        /// </summary>
        public ListSortDirection SortDirection { get; set; }

        #region IComparer<TimelinePanelChild> Members

        public int Compare(TimelinePanelChild x, TimelinePanelChild y)
        {
            if (x.End.HasValue)
            {
                if (y.End.HasValue)
                {
                    // two values, we can make a real comparison
                    if (SortDirection == ListSortDirection.Ascending)
                        return x.End.Value.CompareTo(y.End.Value);
                    return y.End.Value.CompareTo(x.End.Value);
                }
                return 1;
            }
            if (y.End.HasValue)
                return -1;

            return 0;
        }

        #endregion
    }
}
