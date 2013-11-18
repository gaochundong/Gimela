using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 时间线 面板层 集合
    /// </summary>
    internal class TimelinePanelLayerCollection
    {
        /// <summary>
        /// 时间线 面板层 字典，ID:面板层
        /// </summary>
        private readonly Dictionary<int, TimelinePanelLayer> _layers;
        /// <summary>
        /// 时间线 面板层 排序列表
        /// </summary>
        private List<TimelinePanelLayer> _sortedLayers;
        /// <summary>
        /// 时间线 面板子元素比较器
        /// </summary>
        private readonly IComparer<TimelinePanelChild> _childComparer;

        /// <summary>
        /// 时间线 面板层 集合
        /// </summary>
        public TimelinePanelLayerCollection()
        {
            _layers = new Dictionary<int, TimelinePanelLayer>();
            _sortedLayers = new List<TimelinePanelLayer>();
            _childComparer = new TimelinePanelChildEndTimeComparer();
        }

        /// <summary>
        /// 时间线 面板层 排序列表
        /// </summary>
        public IEnumerable<TimelinePanelLayer> Layers
        {
            get
            {
                if (_sortedLayers == null)
                {
                    SortChildren();
                }

                return _sortedLayers;
            }
        }

        /// <summary>
        /// 重置子元素数据
        /// </summary>
        /// <param name="kids">子元素集合</param>
        public void ResetChildData(UIElementCollection kids)
        {
            _layers.Clear();
            _sortedLayers = null;

            // walk the kids
            foreach (UIElement kid in kids)
            {
                var child = kid.ToTimelinePanelChild();
                if (child != null)
                {
                    AddChildToLayer(child);
                }
            }

            SortChildren();
        }

        private void AddChildToLayer(TimelinePanelChild child)
        {
            if (!_layers.ContainsKey(child.ZOrder))
            {
                // add a new layer
                _layers.Add(child.ZOrder, new TimelinePanelLayer(child.ZOrder));
            }

            // add the child
            _layers[child.ZOrder].Children.Add(child);

            _sortedLayers = null;
        }

        private void SortChildren()
        {
            // first sort the _layers
            _sortedLayers = new List<TimelinePanelLayer>(_layers.Values);
            _sortedLayers.Sort();

            // then sort the children
            foreach (var layer in _sortedLayers)
            {
                layer.Children.Sort(_childComparer);
            }
        }
    }
}
