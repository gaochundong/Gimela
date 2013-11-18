using System.Globalization;
using System.Windows.Controls;
using Gimela.Common.Cultures;

namespace Gimela.Rukbat.GUI.ValidationRules
{
  public class InvalidGreaterThanZeroIntegerValidationRule : ValidationRule
  {
    private string INVALID_DESCRIPTION = LanguageString.Find("Framework_ValidationRules_GreaterThanZero");

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      try
      {
        if (value == null)
          return new ValidationResult(false, INVALID_DESCRIPTION);
        if (string.IsNullOrEmpty(value.ToString()))
          return new ValidationResult(false, INVALID_DESCRIPTION);

        int result = int.Parse(value.ToString(), NumberStyles.Integer);
        if (result > 0)
        {
          return new ValidationResult(true, null);
        }
        else
        {
          return new ValidationResult(false, INVALID_DESCRIPTION);
        }
      }
      catch
      {
        return new ValidationResult(false, INVALID_DESCRIPTION);
      }
    }
  }
}
