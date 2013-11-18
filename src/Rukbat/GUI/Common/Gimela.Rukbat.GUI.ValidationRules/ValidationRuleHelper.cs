using System.Threading;
using System.Windows.Controls;
using Gimela.Infrastructure.Patterns;

namespace Gimela.Rukbat.GUI.ValidationRules
{
  public static class ValidationRuleHelper
  {
    public static bool Validate<T>(object value) where T : ValidationRule, new()
    {
      return Singleton<T>.Instance.Validate(value, Thread.CurrentThread.CurrentCulture).IsValid;
    }
  }
}
