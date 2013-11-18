using System.Windows;
using Gimela.Common.ExceptionHandling;

namespace Gimela.Common.Cultures
{
  /// <summary>
  /// 语言字符串
  /// </summary>
  public static class LanguageString
  {
    /// <summary>
    /// 查找指定键对应的值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>值</returns>
    public static string Find(string key)
    {
      string s = string.Empty;

      try
      {
        if (Application.Current.Resources.Contains(key))
        {
          s = ((string)Application.Current.FindResource(key)).Replace(@"\r", "\r").Replace(@"\n", "\n");
        }
      }
      catch (ResourceReferenceKeyNotFoundException ex)
      {
        ExceptionHandler.Handle(ex);
      }

      return s;
    }
  }
}
