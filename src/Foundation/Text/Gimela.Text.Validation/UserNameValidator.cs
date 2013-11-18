using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Gimela.Text.Validation
{
  /// <summary>
  /// 用户名验证器
  /// </summary>
  public static class UserNameValidator
  {
    /// <summary>
    /// 验证用户名格式
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <returns></returns>
    public static bool IsUserName(string userName)
    {
      if (string.IsNullOrWhiteSpace(userName))
      {
        return false;
      }

      if (userName.Length <= 0 || userName.Length > 255)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// 验证用户密码格式
    /// </summary>
    /// <param name="password">用户密码</param>
    /// <returns></returns>
    public static bool IsPassword(string password)
    {
      if (string.IsNullOrWhiteSpace(password))
      {
        return false;
      }

      if (password.Length <= 0 || password.Length > 255)
      {
        return false;
      }

      return true;
    }
  }
}
