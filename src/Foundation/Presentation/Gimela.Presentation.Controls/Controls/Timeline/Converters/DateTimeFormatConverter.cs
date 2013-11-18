using System;
using System.Globalization;
using System.Windows.Data;

namespace Gimela.Presentation.Controls.Timeline
{
  public class DateTimeFormatConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string output = null;
      if (value is DateTime)
      {
        string formatString = (parameter == null) ? "g" : parameter.ToString().Trim().ToLowerInvariant();

        DateTime date = (DateTime)value;

        if (formatString == "f")
        {
          output = date.ToString(@"HH:mm:ss", CultureInfo.InvariantCulture);
        }
        else if (formatString == "h")
        {
          output = date.ToString(@"HH", CultureInfo.InvariantCulture);
        }
        else
        {
          output = date.ToString(formatString, culture.DateTimeFormat);
        }
      }

      return output;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (targetType != typeof(DateTime))
      {
        return null;
      }

      string dateString = value.ToString();

      DateTime parsed;
      DateTime.TryParse(dateString, culture, DateTimeStyles.None, out parsed);

      return parsed;
    }

    #endregion
  }
}
