using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;

namespace Gimela.Common.Cultures
{
  /// <summary>
  /// 语言包加载器
  /// </summary>
  public static class LanguageLoader
  {
    /// <summary>
    /// 加载语言资源
    /// </summary>
    /// <param name="component">组件名称</param>
    /// <param name="culture">文化</param>
    public static void LoadLanguageResource(string component, string culture)
    {
      ResourceDictionary langDict = new ResourceDictionary();
      langDict.Source = new Uri(
        string.Format(CultureInfo.InvariantCulture,
        @"pack://application:,,,/{0};component/{1}.xaml", component, culture), 
        UriKind.RelativeOrAbsolute);

      for (int i = 0; i < Application.Current.Resources.MergedDictionaries.Count; i++)
      {
        ResourceDictionary rd = Application.Current.Resources.MergedDictionaries[i];

        // {pack://application:,,,/Gimela.Rukbat.GUI.Cultures;component/zh-CN.xaml}
        Match mc = Regex.Match(rd.Source.ToString(), @"^.*\.Cultures;component\/[\w-]+.xaml$", RegexOptions.IgnoreCase);
        if (mc.Success)
        {
          // 保证程序按照默认数据正常运行
          if (rd.Count != langDict.Count)
          {
            throw new InvalidProgramException("Error occurred when load language resource!");
          }

          Application.Current.Resources.MergedDictionaries.Remove(rd);
        }
      }

      Application.Current.Resources.MergedDictionaries.Add(langDict);
    }
  }
}
