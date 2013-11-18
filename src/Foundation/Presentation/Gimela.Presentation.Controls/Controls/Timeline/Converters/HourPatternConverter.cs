using System;
using System.Globalization;
using System.Windows.Data;

namespace Gimela.Presentation.Controls.Timeline
{
  public class HourPatternConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      CultureInfo current = CultureInfo.InvariantCulture;

      DateTime local = DateTime.MinValue;
      if (value == null)
      {
        return string.Empty;
      }
      else
      {
        if (value is DateTime)
        {
          DateTime dt = (DateTime)value;

          if (dt.Kind != DateTimeKind.Local)
          {
            local = DateTime.SpecifyKind(dt, DateTimeKind.Local);
          }
          else if (dt.Kind == DateTimeKind.Unspecified)
          {
            local = DateTime.SpecifyKind(dt, DateTimeKind.Local);
          }
          else
          {
            local = dt;
          }
        }
      }

      string pattern = current.DateTimeFormat.ShortTimePattern.Replace(
          current.DateTimeFormat.TimeSeparator + "m", string.Empty).Replace("m", string.Empty);

      if (pattern.Length == 1)
      {
        pattern = "%" + pattern;
      }

      return local.ToString(pattern, CultureInfo.InvariantCulture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
