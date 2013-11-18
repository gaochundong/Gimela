using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Gimela.Presentation.Controls.Converters
{
  public class StatusToAnimationVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        string status = value.ToString();

        switch (status)
        {
          case "Initializing":
          case "Loading":
          case "Saving":
            return Visibility.Visible;
          case "Loaded":
          case "Saved":
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
