using System;
using System.Globalization;
using System.Windows.Data;

namespace Gimela.Presentation.Controls.Converters
{
  public class ObjectNullToBooleanConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
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
