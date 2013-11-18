using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Gimela.Rukbat.GUI.ValidationRules.Converters
{
  public class CheckPortAvailableResultTypeToVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        string resultType = value.ToString();

        switch (resultType)
        {
          case "Available":
            return Visibility.Visible;
          case "Unavailable":
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
