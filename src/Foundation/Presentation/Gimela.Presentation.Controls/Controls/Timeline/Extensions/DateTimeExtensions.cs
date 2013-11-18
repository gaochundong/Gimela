using System;
using System.Globalization;

namespace Gimela.Presentation.Controls.Timeline
{
  /// <summary>
  /// 日期时间的扩展方法
  /// </summary>
  public static partial class DateTimeExtensions
  {
    /// <summary>
    /// 将时间转成位置
    /// </summary>
    /// <param name="value">时间值</param>
    /// <param name="zeroDateTime">为零时间</param>
    /// <param name="_secondsScalingFactor">秒数伸缩测量因数</param>
    /// <returns>位置值</returns>
    public static double ToPosition(this DateTime value, DateTime zeroDateTime, double secondsScalingFactor)
    {
      return (value - zeroDateTime).TotalSeconds * secondsScalingFactor;
    }

    /// <summary>
    /// 精确时间
    /// </summary>
    /// <param name="dateTime">时间</param>
    /// <param name="precision">精确度</param>
    /// <returns>精确时间</returns>
    public static DateTime ToPrecision(this DateTime dateTime, DateTimePrecision precision)
    {
      return dateTime.AddTicks(dateTime.Ticks % (Convert.ToInt64(precision, CultureInfo.InvariantCulture)) * (-1));
    }
  }
}
