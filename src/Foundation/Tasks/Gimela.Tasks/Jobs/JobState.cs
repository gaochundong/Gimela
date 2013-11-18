using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Gimela.Tasks
{
  /// <summary>
  /// 工作状态
  /// </summary>
  [DataContract(Name = "JobState")]
  public enum JobState
  {
    /// <summary>
    /// 新的Job
    /// </summary>
    [EnumMember(Value = "New")]
    [XmlEnum(Name = "New")]
    New = 0,

    /// <summary>
    /// 正在运行
    /// </summary>
    [EnumMember(Value = "Running")]
    [XmlEnum(Name = "Running")]
    Running = 1,

    /// <summary>
    /// 已结束
    /// </summary>
    [EnumMember(Value = "Terminated")]
    [XmlEnum(Name = "Terminated")]
    Terminated = 2,
  }
}
