using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml.Serialization;

namespace Gimela.Tasks.Triggers
{
  /// <summary>
  /// 触发器，这是一个抽象类。
  /// </summary>
  [Serializable]
  [DataContract]
  [XmlInclude(typeof(OnceTrigger)), XmlInclude(typeof(CronTrigger))]
  [KnownType(typeof(OnceTrigger))]
  [KnownType(typeof(CronTrigger))]
  public abstract class Trigger : ITrigger
  {
    /// <summary>
    /// 触发器构造函数
    /// </summary>
    protected Trigger()
    {
    }

    /// <summary>
    /// 触发器Id
    /// </summary>
    [XmlAttribute]
    [DataMember]
    public string Id { get; set; }

    /// <summary>
    /// 触发器名称
    /// </summary>
    [XmlAttribute]
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// 被触发执行的作业
    /// </summary>
    [XmlIgnore]
    [IgnoreDataMember]
    public IJob TargetJob { get; set; }

    /// <summary>
    /// 触发器已终结事件
    /// </summary>
    public event EventHandler<TriggerTerminatedEventArgs> TriggerTerminatedEvent;

    /// <summary>
    /// 触发器是否已终结
    /// </summary>
    [XmlIgnore]
    [IgnoreDataMember]
    public bool IsTerminated { get; set; }

    /// <summary>
    /// 终结触发器
    /// </summary>
    public void Terminate()
    {
      IsTerminated = true;

      if (TriggerTerminatedEvent != null)
      {
        TriggerTerminatedEvent(this, new TriggerTerminatedEventArgs(this.Id));
      }
    }

    /// <summary>
    /// 运行触发器之前的准备工作
    /// </summary>
    protected virtual void PrepareRun()
    {

    }

    /// <summary>
    /// 运行触发器
    /// </summary>
    /// <param name="job">被触发作业</param>
    public void Run(IJob job)
    {
      this.TargetJob = job;
      Run();
    }

    /// <summary>
    /// 运行触发器
    /// </summary>
    public void Run()
    {
      PrepareRun();

      while (!IsTerminated)
      {
        if (CheckFireTime())
        {
          try
          {
            ThreadPool.QueueUserWorkItem((state) =>
            {
              Fire();
            },
            null);
          }
          catch { }
        }

        if (CheckContinue())
        {
          Thread.Sleep(WaitingMilliseconds);
        }
        else
        {
          Terminate();
        }
      }
    }

    /// <summary>
    /// 触发触发器
    /// </summary>
    protected void Fire()
    {
      if (TargetJob != null)
      {
        TargetJob.Run();
      }
    }

    /// <summary>
    /// 检测触发时间
    /// </summary>
    /// <returns></returns>
    protected abstract bool CheckFireTime();

    /// <summary>
    /// 检测是否继续
    /// </summary>
    /// <returns></returns>
    protected abstract bool CheckContinue();

    /// <summary>
    /// 获取等待时长 微秒
    /// </summary>
    /// <returns></returns>
    protected abstract int WaitingMilliseconds { get; }

    #region ToString

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, @"Id[{0}], Name[{1}]", Id, Name);
    }

    #endregion
  }
}
