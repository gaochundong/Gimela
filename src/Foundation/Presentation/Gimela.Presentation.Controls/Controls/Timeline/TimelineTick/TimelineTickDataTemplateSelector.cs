using System.Windows;
using System.Windows.Controls;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 时间刻度的数据模板选择器
    /// </summary>
    public class TimelineTickDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// 小刻度 数据模板 定义在XAML中
        /// </summary>
        public DataTemplate SmallTickDataTemplate { get; set; }
        /// <summary>
        /// 中刻度 数据模板 定义在XAML中
        /// </summary>
        public DataTemplate MediumTickDataTemplate { get; set; }
        /// <summary>
        /// 大刻度 数据模板 定义在XAML中
        /// </summary>
        public DataTemplate BigTickDataTemplate { get; set; }

        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate"/> based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"/> or null. The default value is null.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var tick = item as TimelineTick;
            if (tick != null)
            {
                switch (tick.TickType)
                {
                    case TimelineTickType.Small:
                        return SmallTickDataTemplate;
                    case TimelineTickType.Medium:
                        return MediumTickDataTemplate;
                    case TimelineTickType.Big:
                        return BigTickDataTemplate;
                }
            }

            return null;
        }
    }
}
