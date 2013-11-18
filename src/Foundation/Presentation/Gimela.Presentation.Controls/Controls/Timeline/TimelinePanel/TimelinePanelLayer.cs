using System;
using System.Collections.Generic;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 面板中的层，在同一面板中包含多个层，根据ZOrder布局。该对象为TimelinePanel的直接子元素。
    /// </summary>
    internal class TimelinePanelLayer : IComparable<TimelinePanelLayer>
    {
        /// <summary>
        /// 面板中的层，在同一面板中包含多个层，根据ZOrder布局。
        /// </summary>
        /// <param name="zOrder">空间顺序</param>
        public TimelinePanelLayer(int zOrder)
        {
            ZOrder = zOrder;
            Children = new List<TimelinePanelChild>();
        }

        /// <summary>
        /// 空间顺序
        /// </summary>
        public int ZOrder { get; set; }

        /// <summary>
        /// 子元素列表
        /// </summary>
        public List<TimelinePanelChild> Children { get; set; }

        #region IComparable<TimelinePanelLayer> Members

        public int CompareTo(TimelinePanelLayer other)
        {
            return this.ZOrder.CompareTo(other.ZOrder);
        }

        #endregion
    }
}
