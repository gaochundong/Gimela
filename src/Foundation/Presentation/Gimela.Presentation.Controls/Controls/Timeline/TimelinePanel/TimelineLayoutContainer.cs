using System;
using System.Collections.Generic;
using System.Windows;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 时间线布局容器，这是一个抽象类。
    /// </summary>
    internal abstract class TimelineLayoutContainer : IComparable<TimelineLayoutContainer>
    {
        /// <summary>
        /// 布局位置
        /// </summary>
        private Rect _layoutRect;

        /// <summary>
        /// 时间线布局容器，这是一个抽象类。
        /// </summary>
        /// <param name="ordinal">序数</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        protected TimelineLayoutContainer(int ordinal, double x, double y)
        {
            Ordinal = ordinal;
            Children = new List<TimelinePanelChild>();
            _layoutRect = new Rect(x, y, 0.0, 0.0);
        }

        /// <summary>
        /// 序数
        /// </summary>
        public int Ordinal { get; set; }
        /// <summary>
        /// 秒数的伸缩因数
        /// </summary>
        public double SecondsScalingFactor { get; set; }
        /// <summary>
        /// 布局位置
        /// </summary>
        public Rect LayoutRect { get { return _layoutRect; } }
        /// <summary>
        /// 子元素集合 面板子元素
        /// </summary>
        public List<TimelinePanelChild> Children { get; set; }

        /// <summary>
        /// 是否能够布局该子项
        /// </summary>
        /// <param name="child">面板子元素</param>
        /// <returns>是否能够布局该子项</returns>
        public abstract bool CanLayoutChild(TimelinePanelChild child);

        /// <summary>
        /// 设置布局顶端，这是一个虚方法。
        /// </summary>
        /// <param name="top">顶端</param>
        public virtual void SetLayoutTop(double top)
        {
            _layoutRect.Y = top;
            foreach (var child in Children)
            {
                child.LayoutRect.Y = top;
            }
        }

        /// <summary>
        /// 布局子元素，这是一个虚方法。
        /// </summary>
        /// <param name="child">子元素</param>
        public virtual void LayoutChild(TimelinePanelChild child)
        {
            Children.Add(child);

            // orientation specific, childs Y is the same as the layout containers
            child.LayoutRect.Y = LayoutRect.Y;

            // expand rect to hold child
            _layoutRect.Union(child.LayoutRect);
        }

        /// <summary>
        /// 清除容器内的子元素
        /// </summary>
        public virtual void Clear()
        {
            Children.Clear();
        }

        #region IComparable<TimelineLayoutContainer> Members

        public int CompareTo(TimelineLayoutContainer other)
        {
            // orientation specific
            return LayoutRect.Y.CompareTo(other.LayoutRect.Y);
        }

        #endregion
    }
}
