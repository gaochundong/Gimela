using System;
using System.Globalization;
using System.Windows.Data;

namespace Gimela.Presentation.Controls.Converters
{
  public class StringEmptyToBooleanConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string text = value as string;

      if (string.IsNullOrEmpty(text))
      {
        return false;
      }
      else
      {
        return true;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return string.Empty;
    }

    #endregion
  }
}
