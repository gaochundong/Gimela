using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace Gimela.Presentation.Controls.Converters
{
  public class ByteToColorMultiConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == null)
      {
        throw new ArgumentNullException("values");
      }

      if (values.Length != 3)
      {
        throw new ArgumentException("need three values");
      }

      byte red = (byte)values[0];
      byte green = (byte)values[1];
      byte blue = (byte)values[2];

      return Color.FromRgb(red, green, blue);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      Color color = (Color)value;

      return new object[] { color.R, color.G, color.B };
    }
  }
}
