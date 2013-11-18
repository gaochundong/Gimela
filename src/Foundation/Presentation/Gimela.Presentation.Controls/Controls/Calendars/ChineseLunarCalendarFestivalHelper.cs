using System;
using System.Collections.Generic;
using System.Globalization;

namespace Gimela.Presentation.Controls
{
  public static class ChineseLunarCalendarFestivalHelper
  {
    private static ChineseLunisolarCalendar chineseCalendar = new ChineseLunisolarCalendar();

    private static string[] highOrderDigitOfLunarDate = new string[]
        {
            "初",
            "十",
            "廿",
            "三"
        };

    private static string[] lowOrderDigitOfLunarDate = new string[]
        {
            "一",
            "二",
            "三",
            "四",
            "五",
            "六",
            "七",
            "八",
            "九",
            "十"
        };

    private static string[] lunarMonths = new string[]
        { 
            "正月",
            "二月", 
            "三月",
            "四月", 
            "五月",
            "六月", 
            "七月",
            "八月", 
            "九月",
            "十月", 
            "十一月",
            "十二月" 
        };

    static ChineseLunarCalendarFestivalHelper()
    {
      Festivals = new List<KeyValuePair<string, string>>();

      Festivals.Add(new KeyValuePair<string, string>("0101", "春节"));
      Festivals.Add(new KeyValuePair<string, string>("0115", "元宵节"));
      Festivals.Add(new KeyValuePair<string, string>("0202", "头牙"));
      Festivals.Add(new KeyValuePair<string, string>("0505", "端午节"));
      Festivals.Add(new KeyValuePair<string, string>("0707", "七夕"));
      Festivals.Add(new KeyValuePair<string, string>("0715", "中元节"));
      Festivals.Add(new KeyValuePair<string, string>("0815", "中秋节"));
      Festivals.Add(new KeyValuePair<string, string>("0909", "重阳节"));
      Festivals.Add(new KeyValuePair<string, string>("1208", "腊八节"));
      Festivals.Add(new KeyValuePair<string, string>("1216", "尾牙"));
      Festivals.Add(new KeyValuePair<string, string>("1223", "小年"));
      Festivals.Add(new KeyValuePair<string, string>("1224", "祭灶"));
      Festivals.Add(new KeyValuePair<string, string>("1230", "除夕"));
    }

    /// <summary>
    /// 阴历节日
    /// </summary>
    public static List<KeyValuePair<string, string>> Festivals { get; private set; }

    public static List<string> GetFestivalsOfDay(int year, int month, int day)
    {
      DateTime dateTime = new DateTime(year, month, day);

      int lunarYear = chineseCalendar.GetYear(dateTime);
      int lunarMonth = chineseCalendar.GetMonth(dateTime);
      int lunarDay = chineseCalendar.GetDayOfMonth(dateTime);
      int leapMonth = chineseCalendar.GetLeapMonth(lunarYear);
      int lunarMonthDayNum = chineseCalendar.GetDaysInMonth(lunarYear, lunarMonth);

      if ((leapMonth != 0) && (lunarMonth >= leapMonth))
      {
        lunarMonth--;
      }

      List<string> list = new List<string>();
      string key;

      if (lunarDay == lunarMonthDayNum && lunarMonth == 12)
      {
        key = "1230";
      }
      else
      {
        key = string.Format(CultureInfo.InvariantCulture, "{0}{1}", lunarMonth.ToString("00", CultureInfo.InvariantCulture), lunarDay.ToString("00", CultureInfo.InvariantCulture));
      }

      var query = Festivals.FindAll(p => p.Key == key);
      foreach (var item in query)
      {
        list.Add(item.Value);
      }

      return list;
    }

    public static string GetLunarDayText(int year, int month, int day)
    {
      DateTime dateTime = new DateTime(year, month, day);

      int lunarYear = chineseCalendar.GetYear(dateTime);
      int lunarMonth = chineseCalendar.GetMonth(dateTime);
      int lunarDay = chineseCalendar.GetDayOfMonth(dateTime);
      int leapMonth = chineseCalendar.GetLeapMonth(lunarYear);
      int lunarMonthDayNum = chineseCalendar.GetDaysInMonth(lunarYear, lunarMonth);

      int lunarMonthIndex = lunarMonth;
      int gregorianMonthDayNum = DateTime.DaysInMonth(year, month);

      if ((leapMonth != 0) && (lunarMonth >= leapMonth))
      {
        lunarMonth--;
      }

      string lunarString;
      switch (lunarDay)
      {
        case 1:
          // 每逢农历初一时显示月份
          lunarString = lunarMonths[lunarMonth - 1];
          if (lunarMonthIndex == leapMonth)
          {
            lunarString = "闰" + lunarString;
          }
          break;
        case 10:
          lunarString = "初十";
          break;
        case 20:
          lunarString = "廿十";
          break;
        case 30:
          lunarString = "三十";
          break;
        default:
          lunarString = highOrderDigitOfLunarDate[(lunarDay - 1) / 10] + lowOrderDigitOfLunarDate[(lunarDay - 1) % 10];
          break;
      }

      return lunarString;
    }
  }
}
