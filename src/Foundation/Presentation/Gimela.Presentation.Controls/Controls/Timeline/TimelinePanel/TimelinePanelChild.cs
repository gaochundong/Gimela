using System;
using System.Windows;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 时间线面板子元素，包含至Layer中。
    /// </summary>
    internal class TimelinePanelChild
    {
        /// <summary>
        /// 时间线面板子元素，包含至Layer中。
        /// </summary>
        /// <param name="visual">UIElement元素</param>
        public TimelinePanelChild(UIElement visual)
        {
            Visual = visual;
            Start = DateTime.MinValue;
            End = null;
            ZOrder = 1;
            IsWrapping = true;
            LayoutRect = new Rect();
            Alignment = HorizontalAlignment.Stretch;
        }

        /// <summary>
        /// 元素布局位置
        /// </summary>
        public Rect LayoutRect;

        /// <summary>
        /// 元素代表的开始时间
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// 元素代表的结束时间，可能为空。
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// 空间顺序
        /// </summary>
        public int ZOrder { get; set; }
        /// <summary>
        /// 指定所呈现控件的换行方式
        /// </summary>
        public bool IsWrapping { get; set; }
        /// <summary>
        /// 包含的控件
        /// </summary>
        public UIElement Visual { get; set; }

        /// <summary>
        /// 指定排列方向
        /// </summary>
        public HorizontalAlignment Alignment { get; set; }

        /// <summary>
        /// 如果结束时间不为空，则描述为持续时间，而不是时间点。
        /// </summary>
        public bool IsDuration
        {
            get { return End.HasValue; }
        }
    }
}
