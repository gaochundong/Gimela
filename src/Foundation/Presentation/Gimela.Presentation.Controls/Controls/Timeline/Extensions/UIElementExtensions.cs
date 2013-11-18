using System;
using System.Windows;

namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// UIElement的扩展方法
    /// </summary>
    internal static class UIElementExtensions
    {
        /// <summary>
        /// 将当前元素UIElement转换成TimelinePanelChild，并赋予开始和结束时间。
        /// </summary>
        /// <param name="uiElement">当前元素UIElement</param>
        /// <returns>TimelinePanelChild元素</returns>
        public static TimelinePanelChild ToTimelinePanelChild(this UIElement uiElement)
        {
            var startValue = uiElement.GetValue(TimelinePanel.StartDateTimeProperty);
            if (startValue == null)
            {
                return null;
            }

            var start = (DateTime)startValue;
            var endValue = uiElement.GetValue(TimelinePanel.EndDateTimeProperty);

            // zorder
            // wrapping

            var returnValue =
              new TimelinePanelChild(uiElement)
              {
                  Start = start,
              };

            if (endValue != null)
            {
                var endValueDateTime = (DateTime)endValue;
                returnValue.End = (endValueDateTime == DateTime.MaxValue) ? (DateTime?)null : endValueDateTime;
            }

            return returnValue;
        }
    }
}
