using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Gimela.Common.Logging;
using Gimela.Tasks.Expressions;

namespace Gimela.Tasks.Triggers
{
  /// <summary>
  /// 时程触发器
  /// </summary>
  [Serializable]
  [DataContract]
  public class CronTrigger : Trigger
  {
    /// <summary>
    /// 时程触发器
    /// </summary>
    public CronTrigger()
      : base()
    {
    }

    /// <summary>
    /// 时程触发器
    /// </summary>
    /// <param name="cronExpressionString">时程表达式</param>
    /// <param name="job">触发的作业</param>
    public CronTrigger(string cronExpressionString, IJob job)
      : this()
    {
      CronExpressionString = cronExpressionString;
      TargetJob = job;
    }

    /// <summary>
    /// 时程表达式字符串
    /// </summary>
    [XmlAttribute]
    [DataMember]
    public string CronExpressionString
    {
      get
      {
        return this.CronExpression == null ? null : this.CronExpression.CronExpressionString;
      }
      set
      {
        this.CronExpression = new CronExpression(value);
      }
    }

    /// <summary>
    /// 时程表达式
    /// </summary>
    [XmlIgnore]
    [IgnoreDataMember]
    public CronExpression CronExpression { get; protected set; }

    /// <summary>
    /// 下一次触发时间
    /// </summary>
    [XmlIgnore]
    [IgnoreDataMember]
    public DateTime? NextFireTime { get; protected set; }

    /// <summary>
    /// 运行触发器之前的准备工作
    /// </summary>
    protected override void PrepareRun()
    {
      ComputeNextFireTime();
    }

    /// <summary>
    /// 检测触发时间
    /// </summary>
    /// <returns></returns>
    protected override bool CheckFireTime()
    {
      if (NextFireTime == null || !NextFireTime.HasValue)
      {
        return false;
      }

      if (DateTime.Now >= NextFireTime.Value)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// 检测是否继续
    /// </summary>
    /// <returns></returns>
    protected override bool CheckContinue()
    {
      ComputeNextFireTime();

      if (NextFireTime == null || !NextFireTime.HasValue)
      {
        Logger.Debug(string.Format(CultureInfo.InvariantCulture, @"Is Continue : {0}", "False"));
        return false;
      }

      Logger.Debug(string.Format(CultureInfo.InvariantCulture, @"Is Continue : {0}", "True"));
      return true;
    }

    /// <summary>
    /// 获取等待时长
    /// </summary>
    /// <returns></returns>
    protected override int WaitingMilliseconds
    {
      get
      {
        DateTime now = DateTime.Now;

        if (NextFireTime == null || !NextFireTime.HasValue)
        {
          Logger.Debug(string.Format(CultureInfo.InvariantCulture, @"Waiting : 0 Millisecond when null."));
          return 0;
        }

        if (now > NextFireTime.Value)
        {
          Logger.Debug(string.Format(CultureInfo.InvariantCulture, @"Waiting : 0 Millisecond when over time."));
          return 0;
        }
        else
        {
          int sleep = (int)(Math.Round((NextFireTime.Value - now).TotalMilliseconds));
          Logger.Debug(string.Format(CultureInfo.InvariantCulture, @"Waiting : {0} Milliseconds until {1}.", sleep,
              NextFireTime.HasValue ? NextFireTime.Value.ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture) : "null"));
          return sleep;
        }
      }
    }

    /// <summary>
    /// 计算下一次触发时间
    /// </summary>
    private void ComputeNextFireTime()
    {
      NextFireTime = this.CronExpression.NextTime;

      Logger.Debug(string.Format(CultureInfo.InvariantCulture, @"Next Fire Time : {0} ", NextFireTime.HasValue ?
          NextFireTime.Value.ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture) : "null"));
    }
  }
}
