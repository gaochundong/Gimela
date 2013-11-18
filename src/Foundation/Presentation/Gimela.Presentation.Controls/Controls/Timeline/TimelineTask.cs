using System;
using System.ComponentModel;
using System.Globalization;

namespace Gimela.Presentation.Controls.Timeline
{
  /// <summary>
  /// 时间线任务
  /// </summary>
  public class TimelineTask : INotifyPropertyChanged
  {
    private DateTime startTime;
    private DateTime endTime;

    /// <summary>
    /// 任务开始时间
    /// </summary>
    public DateTime StartTime
    {
      get { return startTime; }
      set
      {
        if (value != startTime)
        {
          startTime = value;
          RaisePropertyChanged("StartTime");
        }
      }
    }

    /// <summary>
    /// 任务结束时间
    /// </summary>
    public DateTime EndTime
    {
      get { return endTime; }
      set
      {
        if (value != endTime)
        {
          endTime = value;
          RaisePropertyChanged("EndTime");
        }
      }
    }

    /// <summary>
    /// 任务时长
    /// </summary>
    public TimeSpan TimeSpan
    {
      get
      {
        return EndTime - StartTime;
      }
      set
      {
        // TimeSpan in this case is read only
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// 任务提示文本
    /// </summary>
    public string TipText
    {
      get
      {
        return this.ToString();
      }
      set
      {
        // TipText in this case is read only
        throw new NotSupportedException();
      }
    }

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the property changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected void RaisePropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
        PropertyChanged(this, args);
      }
    }

    #endregion

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, "{0} -> {1}",
          StartTime.ToString(@"HH:mm:ss", CultureInfo.InvariantCulture),
          EndTime.ToString(@"HH:mm:ss", CultureInfo.InvariantCulture));
    }
  }
}
