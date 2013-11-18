using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Gimela.Presentation.Controls
{
  /// <summary>
  /// 农历和节假日日历
  /// </summary>
  public class LunarCalendar : Calendar, INotifyPropertyChanged
  {
    public static DependencyProperty DateTextItemsProperty =
        DependencyProperty.Register("DateTextItems", typeof(ObservableCollection<DateTextItem>), typeof(Calendar));

    public ObservableCollection<DateTextItem> DateTextItems
    {
      get { return (ObservableCollection<DateTextItem>)GetValue(DateTextItemsProperty); }
      set { SetValue(DateTextItemsProperty, value); }
    }

    static LunarCalendar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(LunarCalendar), new FrameworkPropertyMetadata(typeof(LunarCalendar)));
    }

    public LunarCalendar()
      : base()
    {
      SetValue(DateTextItemsProperty, new ObservableCollection<DateTextItem>());

      InitializeDateTextItems();
    }

    public void AddDateTextItem(DateTextItem dti)
    {
      DateTextItems.Add(dti);
      RaisePropertyChanged("DateTextItems");
    }

    private void InitializeDateTextItems()
    {
      for (int year = 2000; year <= 2050; year++)
      {
        for (int month = 1; month <= 12; month++)
        {
          int days = DateTime.DaysInMonth(year, month);
          for (int day = 1; day <= days; day++)
          {
            DateTime dt = new DateTime(year, month, day);
            string lunar = ChineseLunarCalendarFestivalHelper.GetLunarDayText(year, month, day);
            this.AddDateTextItem(new DateTextItem()
            {
              Date = dt,
              Text = lunar
            });

            List<string> gregorianFestivals = StandardGregorianCalendarFestivalHelper.GetFestivalsOfDay(year, month, day);
            foreach (var item in gregorianFestivals)
            {
              this.AddDateTextItem(new DateTextItem()
              {
                Date = dt,
                Text = item
              });
            }

            List<string> lunarFestivals = ChineseLunarCalendarFestivalHelper.GetFestivalsOfDay(year, month, day);
            foreach (var item in lunarFestivals)
            {
              this.AddDateTextItem(new DateTextItem()
              {
                Date = dt,
                Text = item
              });
            }
          }
        }
      }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion
  }
}
