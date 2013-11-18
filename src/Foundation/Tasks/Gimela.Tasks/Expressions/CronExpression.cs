using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Gimela.Tasks.Expressions
{
  /// <summary>
  /// 时程表达式
  /// </summary>
  [Serializable]
  public class CronExpression
  {
    /// <summary>
    /// 时程表达式字符串
    /// </summary>
    private string _cronExpressionString = null;

    /// <summary>
    /// 时程表达式
    /// </summary>
    public CronExpression()
    {
    }

    /// <summary>
    /// 时程表达式
    /// </summary>
    /// <param name="cronExpressionString">时程表达式字符串</param>
    public CronExpression(string cronExpressionString)
      : this()
    {
      CronExpressionString = cronExpressionString;
    }

    /// <summary>
    /// 时程表达式字符串
    /// </summary>
    [XmlAttribute]
    public string CronExpressionString
    {
      get
      {
        return _cronExpressionString;
      }
      set
      {
        if (string.IsNullOrEmpty(value))
        {
          throw new FormatException("CronExpressionString bad format.");
        }

        _cronExpressionString = value.ToUpperInvariant();

        BuildExpression();
      }
    }

    /// <summary>
    /// 秒数 列表
    /// </summary>
    [XmlIgnore]
    public CronExpressionCollection SecondsList { get; private set; }

    /// <summary>
    /// 分钟 列表
    /// </summary>
    [XmlIgnore]
    public CronExpressionCollection MinutesList { get; private set; }

    /// <summary>
    /// 小时 列表
    /// </summary>
    [XmlIgnore]
    public CronExpressionCollection HoursList { get; private set; }

    /// <summary>
    /// 日期 列表
    /// </summary>
    [XmlIgnore]
    public CronExpressionCollection DaysList { get; private set; }

    /// <summary>
    /// 月份 列表
    /// </summary>
    [XmlIgnore]
    public CronExpressionCollection MonthsList { get; private set; }

    /// <summary>
    /// 星期 列表
    /// </summary>
    [XmlIgnore]
    public CronExpressionCollection WeekdaysList { get; private set; }

    /// <summary>
    /// 年份 列表
    /// </summary>
    [XmlIgnore]
    public CronExpressionCollection YearsList { get; private set; }

    /// <summary>
    /// 编译分析时程表达式
    /// </summary>
    private void BuildExpression()
    {
      // Format : "* * * * * * *"
      // 第1列表示秒数0～59      每  秒用*或者*/1表示
      // 第2列表示分钟0～59      每分钟用*或者*/1表示
      // 第3列表示小时0～23      每小时用*或者*/1表示
      // 第4列表示日期1～31      每  天用*或者*/1表示
      // 第5列表示月份1～12      每  月用*或者*/1表示
      // 第6列表示星期0～6       每  天用*表示*/1表示 0表示星期天
      // 第7列表示月份2000～2099 每年用*或者*/1表示
      // * 代表任意 *   每个
      // / 代表每隔 /2  每隔2
      // - 代表区间 1-7 从1至7
      // , 表示单独 1,3 1和3
      // 例子： 1-10/2 * * 17 9 * * 描述为 9月17日，在每分钟内，从1-10秒间每隔2秒触发一次

      if (string.IsNullOrEmpty(CronExpressionString))
      {
        throw new FormatException("CronExpressionString bad format.");
      }

      // 分割字符串
      string[] columns = CronExpressionString.Trim().Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

      // 验证表达式字符串
      CronExpressionHelper.ValidateCronExpression(columns);

      // 构造基础数据
      for (int i = 0; i < columns.Length; i++)
      {
        string item = columns[i];

        switch (i)
        {
          case 0:
            // 秒
            SecondsList = CronExpressionHelper.ParseSecondExpression(item);
            break;
          case 1:
            // 分钟
            MinutesList = CronExpressionHelper.ParseMinuteExpression(item);
            break;
          case 2:
            // 小时
            HoursList = CronExpressionHelper.ParseHourExpression(item);
            break;
          case 3:
            // 日期
            DaysList = CronExpressionHelper.ParseDayExpression(item);
            break;
          case 4:
            // 月份
            MonthsList = CronExpressionHelper.ParseMonthExpression(item);
            break;
          case 5:
            // 星期
            WeekdaysList = CronExpressionHelper.ParseWeekdayExpression(item);
            break;
          case 6:
            // 年份
            YearsList = CronExpressionHelper.ParseYearExpression(item);
            break;
          default:
            CronExpressionValidationException.Throw("Maybe validation parsed error.");
            break;
        }
      }

      if (SecondsList == null
          || MinutesList == null
          || HoursList == null
          || DaysList == null
          || MonthsList == null
          || WeekdaysList == null
          || YearsList == null)
      {
        CronExpressionValidationException.Throw("CronExpression parsed collections null.");
      }

      // 列表内数据从小到大排序
      SecondsList.Sort();
      MinutesList.Sort();
      HoursList.Sort();
      DaysList.Sort();
      MonthsList.Sort();
      WeekdaysList.Sort();
      YearsList.Sort();
    }

    /// <summary>
    /// 获取下一个符合时程表达式的时间
    /// </summary>
    /// <returns></returns>
    public DateTime? NextTime
    {
      get
      {
        return ComputeNextTime();
      }
    }

    private DateTime? ComputeNextTime()
    {
      // Test Cases:
      // @"* * * * * * *";
      // @"23-37 * * * * * *";
      // @"10-36/3 * * * * * *";
      // @"10,23,27,33,55 * * * * * *";
      // @"13 * * * * * *";
      // @"* * * * * 1 *";
      // @"1 2 3 5 12 1 *";
      // @"1 2 3 * * 0 *";
      // @"1 2 15 * * 0 *";
      // @"* * * * * 0 *";
      // @"20 * * * * 0 *";
      // @"1 2 3 17 9 * 2011";

      DateTime now = DateTime.Now;
      DateTime? nextTime = null;

      for (int y = YearsList.GetIndex(now.Year); y < YearsList.Count; y++)
      {
        for (int o = MonthsList.GetIndex(now.Month); o < MonthsList.Count; o++)
        {
          for (int d = 0; d < DaysList.Count; d++)
          {
            for (int w = WeekdaysList.GetIndex((int)now.DayOfWeek); w < WeekdaysList.Count; w++)
            {
              DateTime weekday = DateTime.MinValue;
              bool dayResult = DateTime.TryParse(string.Format(CultureInfo.InvariantCulture, @"{0}-{1}-{2} {3}:{4}:{5}", YearsList[y], MonthsList[o], DaysList[d], 0, 0, 0), out weekday);
              if (dayResult && (int)weekday.DayOfWeek == WeekdaysList[w])
              {
                if (WeekdaysList[w] > (int)now.DayOfWeek)
                {
                  DateTime target = DateTime.MinValue;
                  bool parseResult = DateTime.TryParse(string.Format(CultureInfo.InvariantCulture, @"{0}-{1}-{2} {3}:{4}:{5}", YearsList[y], MonthsList[o], DaysList[d], HoursList[0], MinutesList[0], SecondsList[0]), out target);
                  if (parseResult && target > now)
                  {
                    return nextTime = target;
                  }
                }
                else
                {
                  for (int h = HoursList.GetIndex(now.Hour); h < HoursList.Count; h++)
                  {
                    for (int m = MinutesList.GetIndex(now.Minute); m < MinutesList.Count; m++)
                    {
                      for (int s = 0; s < SecondsList.Count; s++)
                      {
                        DateTime target = DateTime.MinValue;
                        bool parseResult = DateTime.TryParse(string.Format(CultureInfo.InvariantCulture, @"{0}-{1}-{2} {3}:{4}:{5}", YearsList[y], MonthsList[o], DaysList[d], HoursList[h], MinutesList[m], SecondsList[s]), out target);
                        if (parseResult && target > now)
                        {
                          return nextTime = target;
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }

      return nextTime;
    }
  }
}
