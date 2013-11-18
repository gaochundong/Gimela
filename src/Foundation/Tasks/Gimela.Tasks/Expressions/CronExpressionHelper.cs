using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gimela.Tasks.Expressions
{
  /// <summary>
  /// CronExpression帮助类
  /// </summary>
  public static class CronExpressionHelper
  {
    /// <summary>
    /// 表达式内允许的特殊字符
    /// </summary>
    public static readonly char[] ValidSpecialChars = @"*/-,".ToCharArray();

    /// <summary>
    /// 表达式内允许的数字字符
    /// </summary>
    public static readonly char[] ValidNumberChars = @"0123456789".ToCharArray();

    /// <summary>
    /// 表达式内允许的字符
    /// </summary>
    public static readonly char[] ValidChars = @"0123456789*/-,".ToCharArray();

    /// <summary>
    /// 验证表达式字符串
    /// </summary>
    /// <param name="columns">The columns.</param>
    public static void ValidateCronExpression(string[] columns)
    {
      if (columns == null)
        throw new ArgumentNullException("columns");

      // 表达式字符串必须7列
      if (columns.Length != 7)
      {
        CronExpressionValidationException.Throw("CronExpression must be 7 columns.");
      }

      // 每列字符必须为合法字符
      foreach (string item in columns)
      {
        char[] charList = item.ToCharArray();

        if (charList.Length <= 0)
        {
          CronExpressionValidationException.Throw("Every column length cannot be zero.");
        }

        // 每列字符必须为合法字符
        foreach (char c in charList)
        {
          if (!ValidChars.Contains(c))
          {
            CronExpressionValidationException.Throw("CronExpression chars are illegal.");
          }
        }

        // 某特殊字符在一列中只能出现一次
        foreach (char special in ValidSpecialChars)
        {
          if (special == ',')
          {
            // ','不能连续出现
            if (new String(charList).Contains(@",,"))
            {
              CronExpressionValidationException.Throw("CronExpression commas cannot be consecutive.");
            }

            // ','不能开始和结尾
            if (new String(charList).StartsWith(@",", StringComparison.CurrentCulture) || new String(charList).EndsWith(@",", StringComparison.CurrentCulture))
            {
              CronExpressionValidationException.Throw("CronExpression cannot start with or end with a comma.");
            }
          }
          else
          {
            // 特殊字符在一列中只能出现一次
            if (charList.Where(p => p == special).Count() > 1)
            {
              CronExpressionValidationException.Throw("The special char must only appear once in CronExpression");
            }

            // 特殊字符不能和','同时出现
            if (charList.Where(p => p == special).Count() > 0 && charList.Where(p => p == ',').Count() > 0)
            {
              CronExpressionValidationException.Throw("The special char cannot appear with comma in CronExpression");
            }

            if (special == '*')
            {
              // '*' '-' 不能同时出现
              if (charList.Where(p => p == special).Count() > 0 && charList.Where(p => p == '-').Count() > 0)
              {
                CronExpressionValidationException.Throw("The special char '*' and '-' cannot appear simultaneously in CronExpression");
              }
            }
          }
        }
      }

      // 每列中的值必须在合理范围内
      for (int j = 0; j < columns.Length; j++)
      {
        char[] charList = columns[j].ToCharArray();

        List<string> strList = new List<string>(charList.Length);
        for (int i = 0; i < charList.Length; i++)
        {
          strList.Add(new String(new char[] { charList[i] }));
        }

        string special = new String(ValidSpecialChars);
        for (int i = 0; i < strList.Count; i++)
        {
          if (!special.Contains(strList[i]))
          {
            while (true)
            {
              if ((i + 1 < strList.Count)
                  && !special.Contains(strList[i + 1]))
              {
                strList[i] += strList[i + 1];
                strList.RemoveAt(i + 1);
              }
              else
              {
                break;
              }
            }
          }
        }

        for (int i = 0; i < strList.Count; i++)
        {
          if (strList[i].Length == 1 && special.Contains(strList[i]))
          {
            // 纯特殊字符
          }
          else
          {
            int parser = int.Parse(strList[i], CultureInfo.InvariantCulture);

            // 数字在范围内
            switch (j)
            {
              case 0:
                // 秒
                if (parser < 0 || parser > 59)
                {
                  CronExpressionValidationException.Throw("Seconds", string.Format(CultureInfo.InvariantCulture, @"Must in range {0}-{1}.", 0, 59));
                }
                break;
              case 1:
                // 分钟
                if (parser < 0 || parser > 59)
                {
                  CronExpressionValidationException.Throw("Minutes", string.Format(CultureInfo.InvariantCulture, @"Must in range {0}-{1}.", 0, 59));
                }
                break;
              case 2:
                // 小时
                if (parser < 0 || parser > 23)
                {
                  CronExpressionValidationException.Throw("Hours", string.Format(CultureInfo.InvariantCulture, @"Must in range {0}-{1}.", 0, 23));
                }
                break;
              case 3:
                // 日期
                if (parser < 1 || parser > 31)
                {
                  CronExpressionValidationException.Throw("Days", string.Format(CultureInfo.InvariantCulture, @"Must in range {0}-{1}.", 1, 31));
                }
                break;
              case 4:
                // 月份
                if (parser < 1 || parser > 12)
                {
                  CronExpressionValidationException.Throw("Months", string.Format(CultureInfo.InvariantCulture, @"Must in range {0}-{1}.", 1, 12));
                }
                break;
              case 5:
                // 星期
                if (parser < 0 || parser > 6)
                {
                  CronExpressionValidationException.Throw("Weekdays", string.Format(CultureInfo.InvariantCulture, @"Must in range {0}-{1}.", 0, 6));
                }
                break;
              case 6:
                // 年份
                if (parser < DateTime.Now.Year || parser > 2099)
                {
                  CronExpressionValidationException.Throw("Years", string.Format(CultureInfo.InvariantCulture, @"Must in range {0}-{1}.", DateTime.Now.Year, 2099));
                }
                break;
              default:
                CronExpressionValidationException.Throw("Must not reach here.");
                break;
            }
          }
        }
      }
    }

    /// <summary>
    /// 解析时程表达式中的秒表达式
    /// </summary>
    /// <param name="item">表达式字符串</param>
    /// <returns>表达式集合</returns>
    public static CronExpressionCollection ParseSecondExpression(string item)
    {
      return ParseExpression(item, "Seconds", 0, 59);
    }

    /// <summary>
    /// 解析时程表达式中的分钟表达式
    /// </summary>
    /// <param name="item">表达式字符串</param>
    /// <returns>表达式集合</returns>
    public static CronExpressionCollection ParseMinuteExpression(string item)
    {
      return ParseExpression(item, "Minutes", 0, 59);
    }

    /// <summary>
    /// 解析时程表达式中的小时表达式
    /// </summary>
    /// <param name="item">表达式字符串</param>
    /// <returns>表达式集合</returns>
    public static CronExpressionCollection ParseHourExpression(string item)
    {
      return ParseExpression(item, "Hours", 0, 23);
    }

    /// <summary>
    /// 解析时程表达式中的日表达式
    /// </summary>
    /// <param name="item">表达式字符串</param>
    /// <returns>表达式集合</returns>
    public static CronExpressionCollection ParseDayExpression(string item)
    {
      return ParseExpression(item, "Days", 1, 31);
    }

    /// <summary>
    /// 解析时程表达式中的月表达式
    /// </summary>
    /// <param name="item">表达式字符串</param>
    /// <returns>表达式集合</returns>
    public static CronExpressionCollection ParseMonthExpression(string item)
    {
      return ParseExpression(item, "Months", 1, 12);
    }

    /// <summary>
    /// 解析时程表达式中的周表达式
    /// </summary>
    /// <param name="item">表达式字符串</param>
    /// <returns>表达式集合</returns>
    public static CronExpressionCollection ParseWeekdayExpression(string item)
    {
      return ParseExpression(item, "Weekdays", 0, 6);
    }

    /// <summary>
    /// 解析时程表达式中的年表达式
    /// </summary>
    /// <param name="item">表达式字符串</param>
    /// <returns>表达式集合</returns>
    public static CronExpressionCollection ParseYearExpression(string item)
    {
      return ParseExpression(item, "Years", DateTime.Now.Year, 2099);
    }

    private static CronExpressionCollection ParseExpression(string item, string errorParam, int rangeMin, int rangeMax)
    {
      Regex regex1 = new Regex(@"^\*$");                           // *
      Regex regex2 = new Regex(@"^\*\/([0-9]+)$");                 // */1
      Regex regex3 = new Regex(@"^([0-9]+)\-([0-9]+)$");           // 1-10
      Regex regex4 = new Regex(@"^([0-9]+)\-([0-9]+)\/([0-9]+)$"); // 1-10/1
      Regex regex5 = new Regex(@"^([0-9]+)\,.*([0-9]+)$");         // 1,2,3,4,5
      Regex regex6 = new Regex(@"^([0-9]+)$");                     // 0
      Match m;

      CronExpressionCollection list = new CronExpressionCollection();

      if ((m = regex1.Match(item)).Success)
      {
        for (int t = rangeMin; t <= rangeMax; t++)
        {
          list.Add(t);
        }
      }
      else if ((m = regex2.Match(item)).Success)
      {
        int period = int.Parse(m.Groups[1].ToString(), CultureInfo.InvariantCulture);
        if (period < rangeMin || period > rangeMax)
        {
          CronExpressionValidationException.Throw(errorParam, string.Format(CultureInfo.InvariantCulture, @"The period value must in range [{0}-{1}].", rangeMin, rangeMax));
        }

        for (int t = rangeMin; t <= rangeMax; t++)
        {
          if (t % period == 0)
          {
            list.Add(t);
          }
        }
      }
      else if ((m = regex3.Match(item)).Success)
      {
        int begin = int.Parse(m.Groups[1].ToString(), CultureInfo.InvariantCulture);
        int end = int.Parse(m.Groups[2].ToString(), CultureInfo.InvariantCulture);
        if (begin < rangeMin || begin > rangeMax || end < rangeMin || end > rangeMax)
        {
          CronExpressionValidationException.Throw(errorParam, string.Format(CultureInfo.InvariantCulture, @"The begin and end value must in range [{0}-{1}].", rangeMin, rangeMax));
        }

        if (begin <= end)
        {
          for (int t = begin; t <= end; t++)
          {
            list.Add(t);
          }
        }
        else
        {
          for (int t = begin; t <= rangeMax; t++)
          {
            list.Add(t);
          }
          for (int t = rangeMin; t <= end; t++)
          {
            if (!list.Contains(t))
            {
              list.Add(t);
            }
          }
        }
      }
      else if ((m = regex4.Match(item)).Success)
      {
        int begin = int.Parse(m.Groups[1].ToString(), CultureInfo.InvariantCulture);
        int end = int.Parse(m.Groups[2].ToString(), CultureInfo.InvariantCulture);
        if (begin < rangeMin || begin > rangeMax || end < rangeMin || end > rangeMax)
        {
          CronExpressionValidationException.Throw(errorParam, string.Format(CultureInfo.InvariantCulture, @"The begin and end value must in range [{0}-{1}].", rangeMin, rangeMax));
        }

        int period = int.Parse(m.Groups[3].ToString(), CultureInfo.InvariantCulture);
        if (period < rangeMin || period > rangeMax)
        {
          CronExpressionValidationException.Throw(errorParam, string.Format(CultureInfo.InvariantCulture, @"The period value must in range [{0}-{1}].", rangeMin, rangeMax));
        }

        if (begin <= end)
        {
          for (int t = begin; t <= end; t++)
          {
            if (t % period == 0)
            {
              list.Add(t);
            }
          }
        }
        else
        {
          for (int t = begin; t <= rangeMax; t++)
          {
            if (t % period == 0)
            {
              list.Add(t);
            }
          }
          for (int t = rangeMin; t <= end; t++)
          {
            if (t % period == 0)
            {
              if (!list.Contains(t))
              {
                list.Add(t);
              }
            }
          }
        }
      }
      else if ((m = regex5.Match(item)).Success)
      {
        // ','特殊处理
        string[] splitsComma = item.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var t in splitsComma)
        {
          int p = int.Parse(t, CultureInfo.InvariantCulture);

          if (p < rangeMin || p > rangeMax)
          {
            CronExpressionValidationException.Throw(errorParam, string.Format(CultureInfo.InvariantCulture, @"The comma splitted value must in range [{0}-{1}].", rangeMin, rangeMax));
          }

          if (!list.Contains(p))
          {
            list.Add(p);
          }
        }
      }
      else if ((m = regex6.Match(item)).Success)
      {
        int specified = int.Parse(m.Groups[1].ToString(), CultureInfo.InvariantCulture);
        if (specified < rangeMin || specified > rangeMax)
        {
          CronExpressionValidationException.Throw(errorParam, string.Format(CultureInfo.InvariantCulture, @"The specified value must in range [{0}-{1}].", rangeMin, rangeMax));
        }

        list.Add(specified);
      }
      else
      {
        CronExpressionValidationException.Throw(errorParam, "Regex match error.");
      }

      return list;
    }
  }
}
