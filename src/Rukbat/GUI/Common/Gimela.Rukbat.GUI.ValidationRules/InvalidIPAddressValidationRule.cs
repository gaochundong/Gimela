using System.Globalization;
using System.Windows.Controls;
using Gimela.Common.Cultures;
using Gimela.Text.Validation;

namespace Gimela.Rukbat.GUI.ValidationRules
{
  public class InvalidIPAddressValidationRule : ValidationRule
  {
    private string INVALID_DESCRIPTION = LanguageString.Find("Framework_ValidationRules_InvalidIPAddress");

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      try
      {
        if (value == null)
          return new ValidationResult(false, INVALID_DESCRIPTION);
        if (string.IsNullOrEmpty(value.ToString()))
          return new ValidationResult(false, INVALID_DESCRIPTION);

        bool valid = IPAddressValidator.IsIPAddress(value.ToString());

        if (valid)
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
