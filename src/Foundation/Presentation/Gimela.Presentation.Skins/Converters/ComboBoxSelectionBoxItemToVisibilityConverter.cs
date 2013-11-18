using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Gimela.Presentation.Skins.Converters
{
    public class ComboBoxSelectionBoxItemToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }

        #endregion
    }
}
