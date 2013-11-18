using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Gimela.Presentation.Windows
{
  /// <summary>
  /// 消息通知窗体类型转换成图片源
  /// </summary>
  public class MessageBoxWindowTypeToImageSourceConverter : IValueConverter
  {
    #region IValueConverter Members

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException("value");

      MessageBoxWindowType windowType = (MessageBoxWindowType)Enum.Parse(typeof(MessageBoxWindowType), value.ToString(), true);

      BitmapImage image = new BitmapImage();
      image.BeginInit();

      switch (windowType)
      {
        case MessageBoxWindowType.Question:
          image.UriSource = new Uri(@"pack://application:,,,/Gimela.Presentation.Windows;component/Resources/Images/MessageBoxIcons/question.png", UriKind.Absolute);
          break;
        case MessageBoxWindowType.Warning:
          image.UriSource = new Uri(@"pack://application:,,,/Gimela.Presentation.Windows;component/Resources/Images/MessageBoxIcons/warning.png", UriKind.Absolute);
          break;
        case MessageBoxWindowType.Error:
          image.UriSource = new Uri(@"pack://application:,,,/Gimela.Presentation.Windows;component/Resources/Images/MessageBoxIcons/error.png", UriKind.Absolute);
          break;
        case MessageBoxWindowType.Information:
        default:
          image.UriSource = new Uri(@"pack://application:,,,/Gimela.Presentation.Windows;component/Resources/Images/MessageBoxIcons/information.png", UriKind.Absolute);
          break;
      }

      image.EndInit();

      return image;
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return MessageBoxWindowType.Information;
    }

    #endregion
  }
}
