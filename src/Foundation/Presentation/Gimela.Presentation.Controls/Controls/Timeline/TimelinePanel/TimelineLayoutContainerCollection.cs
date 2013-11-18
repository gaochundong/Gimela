using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 时间线 布局 容器 集合
    /// </summary>
    internal class TimelineLayoutContainerCollection
    {
        /// <summary>
        /// 时间线 布局 容器 列表
        /// </summary>
        private readonly List<TimelineLayoutContainer> _layoutContainers;
        /// <summary>
        /// 子项空间范围
        /// </summary>
        private double _itemSpacerExtent;
        /// <summary>
        /// 秒数的伸缩因数
        /// </summary>
        private double _secondsScalingFactor;

        /// <summary>
        /// 时间线 布局 容器 集合
        /// </summary>
        /// <param name="itemSpacerExtent">子项空间范围</param>
        public TimelineLayoutContainerCollection(double itemSpacerExtent)
        {
            this._itemSpacerExtent = itemSpacerExtent;
            _layoutContainers = new List<TimelineLayoutContainer>();
            _secondsScalingFactor = 0.0;
        }

        /// <summary>
        /// 子项空间范围
        /// </summary>
        public double ItemSpacerExtent
        {
            get { return _itemSpacerExtent; }
            set { _itemSpacerExtent = value; }
        }

        /// <summary>
        /// 时间线布局容器中包含的所有面板子元素
        /// </summary>
        public IEnumerable<TimelinePanelChild> Children
        {
            get
            {
                foreach (var container in _layoutContainers)
                {
                    foreach (var child in container.Children)
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// 清空布局容器集合
        /// </summary>
        public void Clear()
        {
            _layoutContainers.Clear();
        }

        /// <summary>
        /// 布局的位置
        /// </summary>
        public Rect LayoutRect
        {
            get
            {
                AdjustLayoutContainers();
                var returnValue = new Rect();
                foreach (var container in _layoutContainers)
                {
                    returnValue.Union(container.LayoutRect);
                }

                return returnValue;
            }
        }

        /// <summary>
        /// 秒数的伸缩因数
        /// </summary>
        public double SecondsScalingFactor
        {
            get { return _secondsScalingFactor; }
            set
            {
                if (value != _secondsScalingFactor)
                {
                    _secondsScalingFactor = value;
                    foreach (var container in _layoutContainers)
                    {
                        container.SecondsScalingFactor = value;
                    }
                }
            }
        }

        /// <summary>
        /// 添加时间线面板子元素至布局容器中
        /// </summary>
        /// <param name="child">时间线面板子元素</param>
        public void AddChildToLayoutContainers(TimelinePanelChild child)
        {
            bool layedOut = false;

            foreach (var container in _layoutContainers)
            {
                if (container.CanLayoutChild(child))
                {
                    container.LayoutChild(child);
                    layedOut = true;
                    break;
                }

                // depends on orientation
                //nextLayoutContainer = container.LayoutRect.Y + container.LayoutRect.Height + _itemSpacerExtent;
            }

            if (!layedOut)
            {
                // depends on orientation
                int nextLayoutContainerOrdinal = _layoutContainers.Count;
                var nextLayoutContainer =
                  (nextLayoutContainerOrdinal > 0)
                    ?
                      _layoutContainers.Max(container => container.LayoutRect.Bottom)
                    :
                      _itemSpacerExtent;

                _layoutContainers.Add(
                    //new SimpleTimelineLayoutContainer(
                  new PackingTimelineLayoutContainer(
                    nextLayoutContainerOrdinal,
                    0.0,
                    nextLayoutContainer
                    )
                  );

                if (_layoutContainers[nextLayoutContainerOrdinal].CanLayoutChild(child))
                {
                    _layoutContainers[nextLayoutContainerOrdinal].LayoutChild(child);
                }
            }
        }

        /// <summary>
        /// 调整布局
        /// </summary>
        public void AdjustLayoutContainers()
        {
            // adjust any y values for the layout containers, based on a container that was expanded above it
            if (_layoutContainers.Count > 1)
            {
                // get a sorted list of containers from the top
                _layoutContainers.Sort();

                for (int i = 1; i < _layoutContainers.Count; i++)
                {
                    var topContainer = _layoutContainers[i - 1];
                    var bottomContainer = _layoutContainers[i];

                    if (topContainer.LayoutRect.Bottom + ItemSpacerExtent > bottomContainer.LayoutRect.Top)
                    {
                        // give it a new top
                        bottomContainer.SetLayoutTop(topContainer.LayoutRect.Bottom + ItemSpacerExtent);
                    }
                }
            }
        }
    }
}
