using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml.Serialization;
using Gimela.Tasks.Jobs;
using Gimela.Tasks.Triggers;

namespace Gimela.Tasks
{
  /// <summary>
  /// 计划任务
  /// </summary>
  [Serializable]
  [DataContract]
  public class ScheduleTask : IScheduleTask
  {
    /// <summary>
    /// 计划任务
    /// </summary>
    public ScheduleTask()
    {
    }

    /// <summary>
    /// 计划任务Id
    /// </summary>
    [XmlAttribute]
    [DataMember]
    public string Id { get; set; }

    /// <summary>
    /// 计划任务组
    /// </summary>
    [XmlAttribute]
    [DataMember]
    public string Group { get; set; }

    /// <summary>
    /// 计划任务已终结事件
    /// </summary>
    public event EventHandler<ScheduleTaskTerminatedEventArgs> ScheduleTaskTerminatedEvent;

    /// <summary>
    /// 计划任务是否已终结
    /// </summary>
    [XmlAttribute]
    [DataMember]
    public bool IsTerminated { get; set; }

    /// <summary>
    /// 触发器
    /// </summary>
    [XmlElement(Type = typeof(Trigger), ElementName = "Trigger", IsNullable = false)]
    [DataMember(Name = "Trigger", EmitDefaultValue = false, IsRequired = false)]
    public virtual Trigger Trigger { get; set; }

    /// <summary>
    /// 作业任务
    /// </summary>
    [XmlElement(Type = typeof(Job), ElementName = "Job", IsNullable = false)]
    [DataMember(Name = "Job", EmitDefaultValue = false, IsRequired = false)]
    public virtual Job Job { get; set; }

    /// <summary>
    /// 工作线程
    /// </summary>
    [XmlIgnore]
    [IgnoreDataMember]
    public Thread WorkerThread { get; private set; }

    /// <summary>
    /// 是否已经启动执行
    /// </summary>
    [XmlIgnore]
    [IgnoreDataMember]
    public bool IsStarted { get; private set; }

    /// <summary>
    /// 开始执行计划任务
    /// </summary>
    public void Start()
    {
      if (IsStarted) return;

      IsStarted = true;

      this.Trigger.TargetJob = this.Job;
      this.Trigger.TriggerTerminatedEvent += new EventHandler<TriggerTerminatedEventArgs>(OnTriggerTerminatedEvent);

      if (this.WorkerThread == null)
      {
        this.WorkerThread = new Thread((ThreadStart)this.Trigger.Run)
        {
          IsBackground = true,
          Name = string.Format(CultureInfo.InvariantCulture, @"{0}_{1}", this.Job.Id, this.Job.Name).Trim(),
          Priority = ThreadPriority.Normal
        };
      }

      this.WorkerThread.Start();
    }

    /// <summary>
    /// 停止执行计划任务
    /// </summary>
    public void Stop()
    {
      if (!IsStarted) return;

      this.Trigger.TriggerTerminatedEvent -= new EventHandler<TriggerTerminatedEventArgs>(OnTriggerTerminatedEvent);

      if (this.WorkerThread != null)
      {
        this.WorkerThread.Abort();
        this.WorkerThread = null;
      }

      IsStarted = false;
    }

    /// <summary>
    /// 响应触发器终结事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">触发器</param>
    void OnTriggerTerminatedEvent(object sender, TriggerTerminatedEventArgs e)
    {
      Stop();

      IsTerminated = true;

      if (ScheduleTaskTerminatedEvent != null)
      {
        ScheduleTaskTerminatedEvent(this, new ScheduleTaskTerminatedEventArgs(this.Id));
      }
    }

    #region ToString

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, @"Id[{0}], Group[{1}]", Id, Group);
    }

    #endregion
  }
}
