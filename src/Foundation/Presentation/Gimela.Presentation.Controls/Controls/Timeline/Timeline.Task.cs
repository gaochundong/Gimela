using System.Collections.ObjectModel;
using System.Windows;

namespace Gimela.Presentation.Controls.Timeline
{
    public partial class Timeline
    {
        #region Tasks

        public static readonly DependencyProperty TasksProperty =
          DependencyProperty.Register("Tasks", typeof(ObservableCollection<TimelineTask>), typeof(Timeline));

        /// <summary>
        /// 时间线任务集合，这是一个依赖属性。
        /// </summary>
        public ObservableCollection<TimelineTask> Tasks
        {
            get { return (ObservableCollection<TimelineTask>)GetValue(TasksProperty); }
            set { SetValue(TasksProperty, value); }
        }

        #endregion

        #region SelectedTask

        public static readonly DependencyProperty SelectedTaskProperty =
          DependencyProperty.Register("SelectedTask", typeof(TimelineTask), typeof(Timeline));

        /// <summary>
        /// 选中的时间线任务，这是一个依赖属性。
        /// </summary>
        public TimelineTask SelectedTask
        {
            get { return (TimelineTask)GetValue(SelectedTaskProperty); }
            set { SetValue(SelectedTaskProperty, value); }
        }

        #endregion
    }
}
