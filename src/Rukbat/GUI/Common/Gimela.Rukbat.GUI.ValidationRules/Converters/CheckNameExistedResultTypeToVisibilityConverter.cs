using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Gimela.Rukbat.GUI.ValidationRules.Converters
{
  public class CheckNameExistedResultTypeToVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        string resultType = value.ToString();

        switch (resultType)
        {
          case "IsExisted":
            return Visibility.Visible;
          case "NotExisted":
            return Visibility.Visible;
          case "UnsetValue":
          default:
            return Visibility.Collapsed;
        }
      }
      catch (Exception)
      {
        return Visibility.Collapsed;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }

    #endregion
  }
}
