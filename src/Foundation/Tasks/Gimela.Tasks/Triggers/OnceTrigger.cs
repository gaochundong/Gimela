using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Gimela.Common.Logging;

namespace Gimela.Tasks.Triggers
{
  /// <summary>
  /// 仅触发一次的触发器
  /// </summary>
  [Serializable]
  [DataContract]
  public class OnceTrigger : Trigger
  {
    /// <summary>
    /// 仅触发一次的触发器
    /// </summary>
    public OnceTrigger()
      : base()
    {
    }

    /// <summary>
    /// 仅触发一次的触发器
    /// </summary>
    /// <param name="fireTime">指定触发时间</param>
    /// <param name="job">触发的作业</param>
    public OnceTrigger(DateTime fireTime, IJob job)
      : this()
    {
      FireTime = fireTime;
      TargetJob = job;
    }

    /// <summary>
    /// 触发时间
    /// </summary>
    [XmlAttribute]
    [DataMember]
    public DateTime? FireTime { get; set; }

    /// <summary>
    /// 运行触发器之前的准备工作
    /// </summary>
    protected override void PrepareRun()
    {
    }

    /// <summary>
    /// 检测触发时间
    /// </summary>
    /// <returns></returns>
    protected override bool CheckFireTime()
    {
      if (FireTime.HasValue)
      {
        Logger.Debug(string.Format(CultureInfo.InvariantCulture, @"Next Fire Time : {0}",
          FireTime.Value.ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture)));
        if (DateTime.Now >= FireTime.Value)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// 检测是否继续
    /// </summary>
    /// <returns></returns>
    protected override bool CheckContinue()
    {
      return false;
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

        if (now > FireTime.Value)
        {
          return 0;
        }
        else
        {
          int sleep = (int)(Math.Round((FireTime.Value - now).TotalMilliseconds));
          Logger.Debug(string.Format(CultureInfo.InvariantCulture, @"Waiting : {0} Milliseconds until {1}.",
            sleep, FireTime.Value.ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture)));
          return sleep;
        }
      }
    }
  }
}
