using System.Configuration;
using System.Linq;

namespace Gimela.Common.Configuration
{
  /// <summary>
  /// 配置文件管理器
  /// </summary>
  public static class ConfigurationMaster
  {
    private static System.Configuration.Configuration config;
    private const string sectionName = "appSettings";
    private static AppSettingsSection appSettingSection;

    static ConfigurationMaster()
    {
      config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      appSettingSection = (AppSettingsSection)config.GetSection(sectionName);
    }

    /// <summary>
    /// 配置文件中是否包含指定的键
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>是否包含指定的键</returns>
    public static bool ContainsKey(string key)
    {
      return appSettingSection.Settings.AllKeys.Contains(key);
    }

    /// <summary>
    /// 在配置文件中获取指定的键值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>键对应的值</returns>
    public static string Get(string key)
    {
      return appSettingSection.Settings[key].Value;
    }

    /// <summary>
    /// 在配置文件中设置指定的键值对
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void Set(string key, string value)
    {
      appSettingSection.Settings.Remove(key);
      appSettingSection.Settings.Add(key, value);

      // Save the configuration file.
      config.Save(ConfigurationSaveMode.Modified);

      // Force a reload of the changed section. This 
      // makes the new values available for reading.
      ConfigurationManager.RefreshSection(sectionName);
    } 
  }
}
