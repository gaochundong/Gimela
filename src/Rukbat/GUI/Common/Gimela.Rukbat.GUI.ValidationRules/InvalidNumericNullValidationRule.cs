using System.Globalization;
using System.Windows.Controls;
using Gimela.Common.Cultures;

namespace Gimela.Rukbat.GUI.ValidationRules
{
  public class InvalidNumericNullValidationRule : ValidationRule
  {
    private string INVALID_DESCRIPTION = LanguageString.Find("Framework_ValidationRules_NumericNull");

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      try
      {
        if (value == null)
          return new ValidationResult(false, INVALID_DESCRIPTION);
        if (string.IsNullOrEmpty(value.ToString()))
          return new ValidationResult(false, INVALID_DESCRIPTION);

        int result = int.Parse(value.ToString(), NumberStyles.Integer);
        return new ValidationResult(true, null);
      }
      catch
      {
        return new ValidationResult(false, INVALID_DESCRIPTION);
      }
    }
  }
}
