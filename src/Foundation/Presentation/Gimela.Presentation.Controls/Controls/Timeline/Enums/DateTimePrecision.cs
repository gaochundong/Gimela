
namespace Gimela.Presentation.Controls.Timeline
{
    /// <summary>
    /// 时间精确度，包括刻度、毫秒、秒。
    /// </summary>
    public enum DateTimePrecision : long
    {
        //微秒 百万分之一秒（10的负6次方秒）
        //纳秒 十亿分之一秒（10的负7次方秒）

        //TimeSpan( 10, 20, 30, 40, 50 )                 10.20:30:40.0500000
        //Days              10       TotalDays              10.8546302083333
        //Hours             20       TotalHours                   260.511125
        //Minutes           30       TotalMinutes                 15630.6675
        //Seconds           40       TotalSeconds                  937840.05
        //Milliseconds      50       TotalMilliseconds             937840050
        //                           Ticks                     9378400500000

        /// <summary>
        /// 刻度，时间的最小计时周期单位为"刻度"，等于 100 纳秒。刻度可以是负值或正值。
        /// </summary>
        Ticks = 0,
        /// <summary>
        /// 毫秒，当前 TimeSpan 结构的毫秒分量。返回值的范围为 -999 到 999。
        /// </summary>
        Milliseconds = 10000,
        /// <summary>
        /// 秒
        /// </summary>
        Seconds = 10000000,
    }
}
