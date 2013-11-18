using System;
using System.Windows;
using Gimela.Presentation.Controls.Timeline;

namespace TestTimeline
{
  /// <summary>
  /// Interaction logic for TimelineWindow.xaml
  /// </summary>
  public partial class TimelineWindow : Window
  {
    public TimelineWindow()
    {
      InitializeComponent();
      BindViewModel();
    }

    private void BindViewModel()
    {
      DateTime now = DateTime.Parse(@"2020-10-20 00:00:00");

      TimelineTask task1 = new TimelineTask();
      task1.StartTime = now.AddHours(1);
      task1.EndTime = now.AddHours(6);

      TimelineTask task2 = new TimelineTask();
      task2.StartTime = now.AddHours(5);
      task2.EndTime = now.AddHours(9);

      TimelineTask task3 = new TimelineTask();
      task3.StartTime = now.AddHours(6);
      task3.EndTime = now.AddHours(12);

      TimelineViewModel model = new TimelineViewModel();
      model.Now = now;
      model.Start = now;
      model.End = now.AddDays(1);
      model.Zoom = 0.1;
      model.ZoomPercent = 0.1;
      model.Tasks.Add(task1);
      model.Tasks.Add(task2);
      model.Tasks.Add(task3);

      this.DataContext = model;
    }
  }
}
