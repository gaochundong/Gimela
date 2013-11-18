using System.Globalization;
using System.Windows.Controls;
using Gimela.Common.Cultures;

namespace Gimela.Rukbat.GUI.ValidationRules
{
  public class InvalidCheckNameExistedValidationRule : ValidationRule
  {
    private string INVALID_DESCRIPTION = LanguageString.Find("Framework_ValidationRules_CheckNameExisted");

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
          case "IsExisted":
            return new ValidationResult(false, INVALID_DESCRIPTION);
          case "NotExisted":
          case "UnsetValue":
          default:
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
