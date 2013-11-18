using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Gimela.Presentation.Controls.Timeline
{
    public class TaskTimeToGridLengthConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            GridLength returnValue;

            if (values.Length == 2 && values[0] is DateTime && values[1] is TimeSpan)
            {
                var instanceStart = (DateTime)values[0];
                var instanceDuration = (TimeSpan)values[1];

                if (instanceDuration.TotalSeconds > 0)
                {
                    returnValue = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    returnValue = new GridLength(0);
                }

                return returnValue;
            }

            throw new ArgumentException();
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
