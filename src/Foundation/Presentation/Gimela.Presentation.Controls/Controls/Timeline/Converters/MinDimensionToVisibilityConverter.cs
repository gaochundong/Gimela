using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Gimela.Presentation.Controls.Timeline
{
  public class MinDimensionToVisibilityConverter : IValueConverter
  {
    public MinDimensionToVisibilityConverter()
    {
      DefaultVisibility = Visibility.Collapsed;
      TrueVisibility = Visibility.Visible;
      FalseVisibility = Visibility.Collapsed;
    }

    public Visibility DefaultVisibility { get; set; }
    public Visibility TrueVisibility { get; set; }
    public Visibility FalseVisibility { get; set; }

    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      double doubleParameter;
      if (value is double && double.TryParse(parameter.ToString(), out doubleParameter))
      {
        var doubleValue = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);

        return (doubleValue >= doubleParameter) ? TrueVisibility : FalseVisibility;
      }

      return DefaultVisibility;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
