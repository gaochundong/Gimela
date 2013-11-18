using System;
using System.Collections.ObjectModel;
using Gimela.Crust;
using Gimela.Presentation.Controls.Timeline;

namespace TestTimeline
{
  public class TimelineViewModel : ViewModelBase
  {
    public TimelineViewModel()
    {
      tasks = new ObservableCollection<TimelineTask>();
    }

    private DateTime now;
    public DateTime Now
    {
      get
      {
        return now;
      }
      set
      {
        if (value != now)
        {
          now = value;
          RaisePropertyChanged("Now");
        }
      }
    }

    private DateTime start;
    public DateTime Start
    {
      get { return start; }
      set
      {
        if (value != start)
        {
          start = value;
          RaisePropertyChanged("Start");
        }
      }
    }

    private DateTime end;
    public DateTime End
    {
      get { return end; }
      set
      {
        if (value != end)
        {
          end = value;
          RaisePropertyChanged("End");
        }
      }
    }

    private double zoom;
    public double Zoom
    {
      get { return zoom; }
      set
      {
        if (value != zoom)
        {
          zoom = value;
          RaisePropertyChanged("Zoom");
        }
      }
    }

    private double zoomPercent;
    public double ZoomPercent
    {
      get { return zoomPercent; }
      set
      {
        if (value != zoomPercent)
        {
          zoomPercent = value;
          RaisePropertyChanged("ZoomPercent");
        }
      }
    }

    private ObservableCollection<TimelineTask> tasks;
    /// <summary>
    /// 时间线任务集合
    /// </summary>
    public ObservableCollection<TimelineTask> Tasks
    {
      get { return tasks; }
      set
      {
        if (value != tasks)
        {
          tasks = value;
          RaisePropertyChanged("Tasks");
        }
      }
    }

    private TimelineTask selectedTask;
    /// <summary>
    /// 被选中的时间线任务
    /// </summary>
    public TimelineTask SelectedTask
    {
      get { return selectedTask; }
      set
      {
        if (value != selectedTask)
        {
          selectedTask = value;
          RaisePropertyChanged("SelectedTask");
        }
      }
    }
  }
}
