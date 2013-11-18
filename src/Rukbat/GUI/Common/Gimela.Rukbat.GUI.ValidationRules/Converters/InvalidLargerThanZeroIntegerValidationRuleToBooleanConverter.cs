using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Gimela.Rukbat.GUI.ValidationRules.Converters
{
  public class InvalidGreaterThanZeroIntegerValidationRuleToBooleanConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ValidationRuleHelper.Validate<InvalidGreaterThanZeroIntegerValidationRule>(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }

    #endregion
  }
}
