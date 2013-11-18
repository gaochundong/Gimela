using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Gimela.Presentation.Controls.Timeline
{
    public class TimelineTickStrip : Control
    {
        #region Constructors

        public TimelineTickStrip()
        {
            this.Initialized += new EventHandler(TimelineTickStrip_Initialized);
        }

        private void TimelineTickStrip_Initialized(object sender, EventArgs e)
        {
            //ReCalculateTimelineTicks();
        }

        #endregion

        #region Dependency Properties

        #region Ticks

        private static readonly DependencyPropertyKey TicksPropertyKey =
            DependencyProperty.RegisterReadOnly(
              "Ticks",
              typeof(ObservableCollection<TimelineTick>),
              typeof(TimelineTick),
              new FrameworkPropertyMetadata(new ObservableCollection<TimelineTick>())
            );
        public static readonly DependencyProperty TicksProperty =
            TicksPropertyKey.DependencyProperty;

        public ObservableCollection<TimelineTick> Ticks
        {
            get { return (ObservableCollection<TimelineTick>)GetValue(TicksProperty); }
        }

        #endregion

        #region TickStartDateTime

        /// <summary>
        /// TickStartDateTime Dependency Property
        /// </summary>
        public static readonly DependencyProperty TickStartDateTimeProperty =
            DependencyProperty.Register("TickStartDateTime", typeof(DateTime), typeof(TimelineTickStrip),
                new FrameworkPropertyMetadata((DateTime)DateTime.MinValue));

        /// <summary>
        /// Gets or sets the TickStartDateTime property.  This dependency property 
        /// indicates ....
        /// </summary>
        public DateTime TickStartDateTime
        {
            get { return (DateTime)GetValue(TickStartDateTimeProperty); }
            set { SetValue(TickStartDateTimeProperty, value); }
        }

        #endregion

        #region TickEndDateTime

        #region TickEndDateTime

        /// <summary>
        /// TickEndDateTime Dependency Property
        /// </summary>
        public static readonly DependencyProperty TickEndDateTimeProperty =
            DependencyProperty.Register("TickEndDateTime", typeof(DateTime), typeof(TimelineTickStrip),
                new FrameworkPropertyMetadata((DateTime)DateTime.MaxValue));

        /// <summary>
        /// Gets or sets the TickEndDateTime property.  This dependency property 
        /// indicates ....
        /// </summary>
        public DateTime TickEndDateTime
        {
            get { return (DateTime)GetValue(TickEndDateTimeProperty); }
            set { SetValue(TickEndDateTimeProperty, value); }
        }

        #endregion

        #endregion


        #endregion

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return base.ArrangeOverride(arrangeBounds);
        }

        private void ReCalculateTimelineTicks()
        {
            // for now
            Ticks.Clear();

            // do a tick for each hour
            var hours = (int)(TickEndDateTime - TickStartDateTime).TotalHours;

            // round up to the nearest hour
            // need to get the tick based rounding code
            DateTime baseHourTick = TickStartDateTime.AddHours(1).AddMinutes(TickStartDateTime.Minute * -1);
            for (int i = 0; i < hours; i++)
            {
                Ticks.Add(
                  new TimelineTick()
                  {
                      Time = baseHourTick.AddHours(i)
                  }
                  );
            }
        }
    }
}
