using System.Collections.Generic;
using System.Globalization;

namespace Gimela.Presentation.Controls
{
  public static class StandardGregorianCalendarFestivalHelper
  {
    static StandardGregorianCalendarFestivalHelper()
    {
      Festivals = new List<KeyValuePair<string, string>>();

      Festivals.Add(new KeyValuePair<string, string>("0101", "元旦"));
      Festivals.Add(new KeyValuePair<string, string>("0215", "中国12亿人口日"));
      Festivals.Add(new KeyValuePair<string, string>("0305", "中国青年志愿者服务日"));
      Festivals.Add(new KeyValuePair<string, string>("0308", "国际劳动妇女节"));
      Festivals.Add(new KeyValuePair<string, string>("0312", "中国植树节"));
      Festivals.Add(new KeyValuePair<string, string>("0318", "全国科技人才活动日"));
      Festivals.Add(new KeyValuePair<string, string>("0425", "全国预防接种宣传日"));
      Festivals.Add(new KeyValuePair<string, string>("0430", "全国交通安全反思日"));
      Festivals.Add(new KeyValuePair<string, string>("0501", "国际劳动节"));
      Festivals.Add(new KeyValuePair<string, string>("0504", "中国青年节"));
      Festivals.Add(new KeyValuePair<string, string>("0504", "五四运动纪念日"));
      Festivals.Add(new KeyValuePair<string, string>("0505", "全国碘缺乏病防治日"));
      Festivals.Add(new KeyValuePair<string, string>("0520", "全国母乳喂养宣传日"));
      Festivals.Add(new KeyValuePair<string, string>("0601", "国际儿童节"));
      Festivals.Add(new KeyValuePair<string, string>("0606", "全国爱眼日"));
      Festivals.Add(new KeyValuePair<string, string>("0611", "中国人口日"));
      Festivals.Add(new KeyValuePair<string, string>("0622", "中国儿童慈善活动日"));
      Festivals.Add(new KeyValuePair<string, string>("0625", "全国土地日"));
      Festivals.Add(new KeyValuePair<string, string>("0701", "中国共产党诞生日"));
      Festivals.Add(new KeyValuePair<string, string>("0701", "香港回归纪念日"));
      Festivals.Add(new KeyValuePair<string, string>("0707", "中国人民抗日战争纪念日"));
      Festivals.Add(new KeyValuePair<string, string>("0801", "中国人民解放军建军节"));
      Festivals.Add(new KeyValuePair<string, string>("0826", "全国律师咨询日"));
      Festivals.Add(new KeyValuePair<string, string>("0903", "中国抗日战争胜利纪念日"));
      Festivals.Add(new KeyValuePair<string, string>("0910", "中国教师节"));
      Festivals.Add(new KeyValuePair<string, string>("0918", "九一八事变纪念日"));
      Festivals.Add(new KeyValuePair<string, string>("0920", "全国爱牙日"));
      Festivals.Add(new KeyValuePair<string, string>("1001", "国庆节"));
      Festivals.Add(new KeyValuePair<string, string>("1008", "全国高血压日"));
      Festivals.Add(new KeyValuePair<string, string>("1010", "辛亥革命纪念日"));
      Festivals.Add(new KeyValuePair<string, string>("1108", "中国记者节"));
      Festivals.Add(new KeyValuePair<string, string>("1109", "中国消防宣传日"));
      Festivals.Add(new KeyValuePair<string, string>("1110", "世界青年节"));
      Festivals.Add(new KeyValuePair<string, string>("1204", "中国法制宣传日"));
      Festivals.Add(new KeyValuePair<string, string>("1212", "西安事变纪念日"));
      Festivals.Add(new KeyValuePair<string, string>("1213", "南京大屠杀纪念日"));
      Festivals.Add(new KeyValuePair<string, string>("1220", "澳门回归纪念日"));
      Festivals.Add(new KeyValuePair<string, string>("1224", "平安夜"));
      Festivals.Add(new KeyValuePair<string, string>("1225", "圣诞节"));
    }

    /// <summary>
    /// 阳历节日
    /// </summary>
    public static List<KeyValuePair<string, string>> Festivals { get; private set; }

    public static List<string> GetFestivalsOfDay(int year, int month, int day)
    {
      List<string> list = new List<string>();
      string dateString = string.Format(CultureInfo.InvariantCulture, "{0}{1}", month.ToString("00", CultureInfo.InvariantCulture), day.ToString("00", CultureInfo.InvariantCulture));

      var query = Festivals.FindAll(p => p.Key == dateString);
      foreach (var item in query)
      {
        list.Add(item.Value);
      }

      return list;
    }
  }
}
