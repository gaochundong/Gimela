using System;
using System.Windows.Data;

namespace Gimela.Presentation.Controls.Converters
{
  public class StringListToStringMultiConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values == null || values.Length <= 0)
      {
        return string.Empty;
      }

      string target = string.Empty;
      foreach (var item in values)
      {
        if (item == null) continue;

        target = target + "[ " + item.ToString() + " ], ";
      }

      return target.TrimEnd(new char[] { ',', ' ' });
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      return new object[] { };
    }
  }
}
