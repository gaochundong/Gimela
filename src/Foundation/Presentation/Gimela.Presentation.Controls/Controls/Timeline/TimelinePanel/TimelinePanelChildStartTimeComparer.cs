using System.Collections.Generic;
using System.ComponentModel;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 元素开始时间比较器
    /// </summary>
    internal class TimelinePanelChildStartTimeComparer : IComparer<TimelinePanelChild>
    {
        /// <summary>
        /// 元素开始时间比较器
        /// </summary>
        public TimelinePanelChildStartTimeComparer()
        {
            SortDirection = ListSortDirection.Ascending;
        }

        /// <summary>
        /// 指定排序方向，默认为升序排序。
        /// </summary>
        public ListSortDirection SortDirection { get; set; }

        #region IComparer<TimelinePanelChild> Members

        public int Compare(TimelinePanelChild x, TimelinePanelChild y)
        {
            if (SortDirection == ListSortDirection.Descending)
                return x.Start.CompareTo(y.Start);

            return y.Start.CompareTo(x.Start);
        }

        #endregion
    }
}
