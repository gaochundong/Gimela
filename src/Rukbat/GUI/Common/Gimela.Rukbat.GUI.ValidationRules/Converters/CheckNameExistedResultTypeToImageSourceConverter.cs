using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Gimela.Rukbat.GUI.ValidationRules.Converters
{
  public class CheckNameExistedResultTypeToImageSourceConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string resultType = value.ToString();

      BitmapImage image = new BitmapImage();
      image.BeginInit();

      switch (resultType)
      {
        case "IsExisted":
          image.UriSource = new Uri(@"pack://application:,,,/Gimela.Rukbat.GUI.ValidationRules;component/Resources/Images/CheckNameExistedResultImages/isexisted.png", UriKind.Absolute);
          break;
        case "NotExisted":
        case "UnsetValue":
        default:
          image.UriSource = new Uri(@"pack://application:,,,/Gimela.Rukbat.GUI.ValidationRules;component/Resources/Images/CheckNameExistedResultImages/notexisted.png", UriKind.Absolute);
          break;
      }

      image.EndInit();

      return image;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }

    #endregion
  }
}
