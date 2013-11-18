using System;
using System.Windows.Data;

namespace Gimela.Presentation.Controls.Converters
{
  public class ByteToDoubleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return (double)(byte)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return (byte)(double)value;
    }
  }
}
