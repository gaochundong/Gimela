using System;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Gimela.Presentation.Controls
{
  [ValueConversion(typeof(ObservableCollection<DateTextItem>), typeof(ObservableCollection<DateTextItem>))]
  public class DateTextItemsMultiConverter : IMultiValueConverter
  {
    #region IMultiValueConverter Members

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      DateTime date = (DateTime)values[1];
      ObservableCollection<DateTextItem> dis = new ObservableCollection<DateTextItem>();

      foreach (DateTextItem item in (ObservableCollection<DateTextItem>)values[0])
      {
        if (item.Date.Date == date)
        {
          dis.Add(item);
        }
      }

      return dis;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
