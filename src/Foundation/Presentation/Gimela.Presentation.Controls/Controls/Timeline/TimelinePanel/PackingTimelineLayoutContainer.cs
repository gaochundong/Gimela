
namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 填充式时间线布局容器，将子元素填充至可用的空间位置。
    /// </summary>
    internal class PackingTimelineLayoutContainer : TimelineLayoutContainer
    {
        /// <summary>
        /// 填充式时间线布局容器，将子元素填充至可用的空间位置。
        /// </summary>
        /// <param name="ordinal">序数</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public PackingTimelineLayoutContainer(int ordinal, double x, double y)
            : base(ordinal, x, y)
        {
            // capable of packing children into available space
        }

        /// <summary>
        /// 是否能够布局该子项
        /// </summary>
        /// <param name="child">面板子元素</param>
        /// <returns>
        /// 是否能够布局该子项
        /// </returns>
        public override bool CanLayoutChild(TimelinePanelChild child)
        {
            var returnValue = true;
            var testRect = child.LayoutRect;

            // orientation specific... adjust the child Y for this layout container
            testRect.Y = LayoutRect.Y;
            
            foreach (var existingChild in Children)
            {
                if (existingChild.LayoutRect.IntersectsWith(testRect))
                {
                    returnValue = false;
                    break;
                }
            }

            return returnValue;
        }
    }
}
