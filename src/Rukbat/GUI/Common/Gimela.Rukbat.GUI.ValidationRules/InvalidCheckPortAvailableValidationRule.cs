using System.Globalization;
using System.Windows.Controls;
using Gimela.Common.Cultures;

namespace Gimela.Rukbat.GUI.ValidationRules
{
  public class InvalidCheckPortAvailableValidationRule : ValidationRule
  {
    private string INVALID_DESCRIPTION = LanguageString.Find("Framework_ValidationRules_CheckPortAvailable");

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      try
      {
        if (value == null)
          return new ValidationResult(false, INVALID_DESCRIPTION);
        if (string.IsNullOrEmpty(value.ToString()))
          return new ValidationResult(false, INVALID_DESCRIPTION);

        string resultType = value.ToString();
        switch (resultType)
        {
          case "Available":
            return new ValidationResult(true, null);
          case "Unavailable":
          case "UnsetValue":
          default:
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
