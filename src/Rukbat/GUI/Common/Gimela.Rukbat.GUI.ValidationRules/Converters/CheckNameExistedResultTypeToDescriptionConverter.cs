using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Gimela.Common.Cultures;

namespace Gimela.Rukbat.GUI.ValidationRules.Converters
{
  public class CheckNameExistedResultTypeToDescriptionConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string resultType = value.ToString();

      string description = string.Empty;

      switch (resultType)
      {
        case "IsExisted":
          description = string.Empty; // 默认不显示
          break;
        case "NotExisted":
          description = string.Empty; // 默认不显示
          break;
        case "UnsetValue":
        default:
          description = string.Empty;
          break;
      }

      return description;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }

    #endregion
  }
}
