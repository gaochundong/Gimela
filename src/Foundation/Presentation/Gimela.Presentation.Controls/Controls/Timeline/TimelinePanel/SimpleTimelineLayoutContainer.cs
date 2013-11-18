
namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 简单时间线布局容器，每行仅允许一个子元素。
    /// </summary>
    internal class SimpleTimelineLayoutContainer : TimelineLayoutContainer
    {
        /// <summary>
        /// 简单时间线布局容器，每行仅允许一个子元素。
        /// </summary>
        /// <param name="ordinal">序数</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public SimpleTimelineLayoutContainer(int ordinal, double x, double y)
            : base(ordinal, x, y)
        {
            // only allows one child per row
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
            return true;
        }
    }
}
