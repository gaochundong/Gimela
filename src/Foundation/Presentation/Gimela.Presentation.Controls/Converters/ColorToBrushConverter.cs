using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Gimela.Presentation.Controls.Converters
{
  [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
  public class ColorToBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      Color color = (Color)value;
      return new SolidColorBrush(color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return null;
    }
  }
}
