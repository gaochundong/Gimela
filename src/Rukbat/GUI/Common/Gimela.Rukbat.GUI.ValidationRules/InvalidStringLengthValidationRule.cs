using System.Globalization;
using System.Windows.Controls;
using Gimela.Common.Cultures;

namespace Gimela.Rukbat.GUI.ValidationRules
{
  public class InvalidStringLengthValidationRule : ValidationRule
  {
    private string INVALID_DESCRIPTION = LanguageString.Find("Framework_ValidationRules_StringLength");

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      try
      {
        if (value == null)
          return new ValidationResult(false, INVALID_DESCRIPTION);
        if (string.IsNullOrEmpty(value.ToString()))
          return new ValidationResult(false, INVALID_DESCRIPTION);

        string testString = ((string)value).Replace(" ", "");

        if (string.IsNullOrEmpty(testString))
        {
          return new ValidationResult(false, INVALID_DESCRIPTION);
        }
        else if (testString.Length > 255)
        {
          return new ValidationResult(false, INVALID_DESCRIPTION);
        }
        else
        {
          return new ValidationResult(true, null);
        }
      }
      catch
      {
        return new ValidationResult(false, INVALID_DESCRIPTION);
      }
    }
  }
}
