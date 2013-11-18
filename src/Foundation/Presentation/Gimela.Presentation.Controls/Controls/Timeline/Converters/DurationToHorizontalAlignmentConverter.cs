using System;
using System.Windows;
using System.Windows.Data;

namespace Gimela.Presentation.Controls.Timeline
{
    public class DurationToHorizontalAlignmentConverter : IMultiValueConverter
    {
        private HorizontalAlignment falseHorizontalAlignment = HorizontalAlignment.Center;
        private HorizontalAlignment trueHorizontalAlignment = HorizontalAlignment.Stretch;

        public double MinimumExtent { get; set; }

        public DurationToHorizontalAlignmentConverter()
        {
            MinimumExtent = 100;
        }

        private bool invert;

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            HorizontalAlignment returnValue = trueHorizontalAlignment;

            invert = false;
            if (values.Length == 2)
            {
                if (values[0] is double && values[1] is TimeSpan)
                {
                    var zoom = (double)values[0];
                    var duration = (TimeSpan)values[1];

                    if (parameter is string)
                    {
                        bool parameterInvert;
                        if (bool.TryParse(parameter as string, out parameterInvert))
                        {
                            invert = parameterInvert;
                        }
                    }

                    var extent = duration.TotalSeconds * zoom;

                    if (!invert)
                    {
                        returnValue = (extent <= MinimumExtent) ? falseHorizontalAlignment : trueHorizontalAlignment;
                    }
                    else
                    {
                        returnValue = (extent >= MinimumExtent) ? falseHorizontalAlignment : trueHorizontalAlignment;
                    }
                }
            }

            return returnValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
