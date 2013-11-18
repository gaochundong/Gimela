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
  /// 双工计划任务，指定起始和结束触发器，当结束触发器任务运行完毕时终结。
  /// </summary>
  [Serializable]
  [DataContract]
  public class DuplexScheduleTask : IScheduleTask
  {
    /// <summary>
    /// 双工计划任务
    /// </summary>
    public DuplexScheduleTask()
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
    /// 开始触发器
    /// </summary>
    [XmlElement(Type = typeof(Trigger), ElementName = "BeginTrigger", IsNullable = false)]
    [DataMember(Name = "BeginTrigger", EmitDefaultValue = false, IsRequired = false)]
    public virtual Trigger BeginTrigger { get; set; }

    /// <summary>
    /// 结束触发器
    /// </summary>
    [XmlElement(Type = typeof(Trigger), ElementName = "EndTrigger", IsNullable = false)]
    [DataMember(Name = "EndTrigger", EmitDefaultValue = false, IsRequired = false)]
    public virtual Trigger EndTrigger { get; set; }

    /// <summary>
    /// 开始作业任务
    /// </summary>
    [XmlElement(Type = typeof(Job), ElementName = "BeginJob", IsNullable = false)]
    [DataMember(Name = "BeginJob", EmitDefaultValue = false, IsRequired = false)]
    public virtual Job BeginJob { get; set; }

    /// <summary>
    /// 结束作业任务
    /// </summary>
    [XmlElement(Type = typeof(Job), ElementName = "EndJob", IsNullable = false)]
    [DataMember(Name = "EndJob", EmitDefaultValue = false, IsRequired = false)]
    public virtual Job EndJob { get; set; }

    /// <summary>
    /// 开始工作线程
    /// </summary>
    [XmlIgnore]
    [IgnoreDataMember]
    public Thread BeginWorkerThread { get; private set; }

    /// <summary>
    /// 结束工作线程
    /// </summary>
    [XmlIgnore]
    [IgnoreDataMember]
    public Thread EndWorkerThread { get; private set; }

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

      this.BeginTrigger.TargetJob = this.BeginJob;
      this.EndTrigger.TargetJob = this.EndJob;

      this.EndTrigger.TriggerTerminatedEvent += new EventHandler<TriggerTerminatedEventArgs>(OnEndTriggerTerminatedEvent);

      if (this.BeginWorkerThread == null)
      {
        this.BeginWorkerThread = new Thread((ThreadStart)this.BeginTrigger.Run)
        {
          IsBackground = true,
          Name = string.Format(CultureInfo.InvariantCulture, @"{0}_{1}", this.BeginJob.Id, this.BeginJob.Name).Trim(),
          Priority = ThreadPriority.Normal
        };
      }
      if (this.EndWorkerThread == null)
      {
        this.EndWorkerThread = new Thread((ThreadStart)this.EndTrigger.Run)
        {
          IsBackground = true,
          Name = string.Format(CultureInfo.InvariantCulture, @"{0}_{1}", this.EndJob.Id, this.EndJob.Name).Trim(),
          Priority = ThreadPriority.Normal
        };
      }

      this.BeginWorkerThread.Start();
      this.EndWorkerThread.Start();
    }

    /// <summary>
    /// 停止执行计划任务
    /// </summary>
    public void Stop()
    {
      if (!IsStarted) return;

      this.EndTrigger.TriggerTerminatedEvent -= new EventHandler<TriggerTerminatedEventArgs>(OnEndTriggerTerminatedEvent);

      if (this.BeginWorkerThread != null)
      {
        this.BeginWorkerThread.Abort();
        this.BeginWorkerThread = null;
      }

      if (this.EndWorkerThread != null)
      {
        this.EndWorkerThread.Abort();
        this.EndWorkerThread = null;
      }

      IsStarted = false;
    }

    /// <summary>
    /// 响应触发器终结事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">触发器</param>
    void OnEndTriggerTerminatedEvent(object sender, TriggerTerminatedEventArgs e)
    {
      IsTerminated = true;

      if (ScheduleTaskTerminatedEvent != null)
      {
        ScheduleTaskTerminatedEvent(this, new ScheduleTaskTerminatedEventArgs(this.Id));
      }

      Stop();
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
