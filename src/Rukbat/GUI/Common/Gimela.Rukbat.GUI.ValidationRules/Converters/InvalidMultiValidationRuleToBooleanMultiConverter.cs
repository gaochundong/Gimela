using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Gimela.Rukbat.GUI.ValidationRules.Converters
{
  public class InvalidMultiValidationRuleToBooleanMultiConverter : IMultiValueConverter
  {
    #region IMultiValueConverter Members

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      string[] paramlist = ((string)parameter).Split('|');
      if (paramlist == null || paramlist.Length <= 0)
      {
        throw new ArgumentNullException("parameter");
      }
      int length = paramlist.Length;

      IList<bool> boollist = new List<bool>();

      for (int i = 0; i < paramlist.Length; i++)
      {
        switch (paramlist[i].ToLowerInvariant())
        {
          case "checknameexisted":
            boollist.Add(ValidationRuleHelper.Validate<InvalidCheckNameExistedValidationRule>(values[i]));
            break;
          case "checkportavailable":
            boollist.Add(ValidationRuleHelper.Validate<InvalidCheckPortAvailableValidationRule>(values[i]));
            break;
          case "directoryandfileexist":
            boollist.Add(ValidationRuleHelper.Validate<InvalidDirectoryAndFileExistValidationRule>(values[i]));
            break;
          case "greaterthanzerointeger":
            boollist.Add(ValidationRuleHelper.Validate<InvalidGreaterThanZeroIntegerValidationRule>(values[i]));
            break;
          case "numericnull":
            boollist.Add(ValidationRuleHelper.Validate<InvalidNumericNullValidationRule>(values[i]));
            break;
          case "stringlength":
            boollist.Add(ValidationRuleHelper.Validate<InvalidStringLengthValidationRule>(values[i]));
            break;
          case "stringnullorempty":
            boollist.Add(ValidationRuleHelper.Validate<InvalidStringNullOrEmptyValidationRule>(values[i]));
            break;
          case "ipaddress":
            boollist.Add(ValidationRuleHelper.Validate<InvalidIPAddressValidationRule>(values[i]));
            break;
          case "objectnull":
          default:
            boollist.Add(ValidationRuleHelper.Validate<InvalidObjectNullValidationRule>(values[i]));
            break;
        }
      }

      bool result = boollist[0];
      for (int i = 1; i < boollist.Count; i++)
      {
        result = result & boollist[i];
      }

      return result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return null;
    }

    #endregion
  }
}
