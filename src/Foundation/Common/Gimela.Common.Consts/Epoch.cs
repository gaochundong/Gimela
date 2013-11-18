using System;
using System.Globalization;

namespace Gimela.Common.Consts
{
  /// <summary>
  /// 常量纪元时间
  /// </summary>
  public static class Epoch
  {
    /// <summary>
    /// 常量纪元时间, UTC时间, 1970-01-01 00:00:00
    /// </summary>
    public static readonly DateTime Utc = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    /// <summary>
    /// 常量纪元时间, 本地时间, 在UTC时间基础上增加本地TimeZone偏移
    /// </summary>
    public static readonly DateTime Local = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Add(TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now));

    /// <summary>
    /// 常量2013时间, UTC时间, 2013-01-01 00:00:00
    /// </summary>
    public static readonly DateTime Utc2013 = new DateTime(2013, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    /// <summary>
    /// 常量2013时间, 本地时间, 在UTC时间基础上增加本地TimeZone偏移
    /// </summary>
    public static readonly DateTime Local2013 = new DateTime(2013, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Add(TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now));

    /// <summary>
    /// 获取当前时间距今天起点的毫秒数
    /// </summary>
    /// <returns>毫秒数</returns>
    public static double GetNowTotalMillisecondsByToday()
    {
      DateTime now = DateTime.Now;
      DateTime today = DateTime.Parse(now.ToString(@"yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
      today = DateTime.SpecifyKind(today, DateTimeKind.Local);
      return (now - today).TotalMilliseconds;
    }

    /// <summary>
    /// 获取当前时间距昨天起点的毫秒数
    /// </summary>
    /// <returns>毫秒数</returns>
    public static double GetNowTotalMillisecondsByYesterday()
    {
      DateTime now = DateTime.Now;
      DateTime yesterday = DateTime.Parse(now.Subtract(TimeSpan.FromDays(1)).ToString(@"yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
      yesterday = DateTime.SpecifyKind(yesterday, DateTimeKind.Local);
      return (now - yesterday).TotalMilliseconds;
    }

    /// <summary>
    /// 获取指定时间距今天起点的毫秒数
    /// </summary>
    /// <param name="time">指定时间</param>
    /// <returns>毫秒数</returns>
    public static double GetDateTimeTotalMillisecondsByToday(DateTime time)
    {
      DateTime today = DateTime.Parse(time.ToString(@"yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
      today = DateTime.SpecifyKind(today, DateTimeKind.Local);
      return (time - today).TotalMilliseconds;
    }

    /// <summary>
    /// 获取指定时间距昨天起点的毫秒数
    /// </summary>
    /// <param name="time">指定时间</param>
    /// <returns>毫秒数</returns>
    public static double GetDateTimeTotalMillisecondsByYesterday(DateTime time)
    {
      DateTime yesterday = DateTime.Parse(time.Subtract(TimeSpan.FromDays(1)).ToString(@"yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
      yesterday = DateTime.SpecifyKind(yesterday, DateTimeKind.Local);
      return (time - yesterday).TotalMilliseconds;
    }

    /// <summary>
    /// 根据毫秒数和今天的起点获取时间
    /// </summary>
    /// <param name="totalMilliseconds">毫秒数</param>
    /// <returns>时间</returns>
    public static DateTime GetDateTimeByTodayTotalMilliseconds(double totalMilliseconds)
    {
      DateTime now = DateTime.Now;
      DateTime today = DateTime.Parse(now.ToString(@"yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
      today = DateTime.SpecifyKind(today, DateTimeKind.Local);
      return today.AddMilliseconds(totalMilliseconds);
    }

    /// <summary>
    /// 根据毫秒数和昨天的起点获取时间
    /// </summary>
    /// <param name="totalMilliseconds">毫秒数</param>
    /// <returns>时间</returns>
    public static DateTime GetDateTimeByYesterdayTotalMilliseconds(double totalMilliseconds)
    {
      DateTime now = DateTime.Now;
      DateTime yesterday = DateTime.Parse(now.Subtract(TimeSpan.FromDays(1)).ToString(@"yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
      yesterday = DateTime.SpecifyKind(yesterday, DateTimeKind.Local);
      return yesterday.AddMilliseconds(totalMilliseconds);
    }
  }
}
