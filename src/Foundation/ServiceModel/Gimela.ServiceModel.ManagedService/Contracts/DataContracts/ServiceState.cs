using System.Runtime.Serialization;

namespace Gimela.ServiceModel.ManagedService.Contracts.DataContracts
{
  /// <summary>
  /// 服务状态
  /// </summary>
  [DataContract]
  public enum ServiceState
  {
    /// <summary>
    /// 状态不可知
    /// </summary>
    [EnumMember]
    Unknown = 0,

    /// <summary>
    /// 正在启动
    /// </summary>
    [EnumMember]
    Starting = 1,

    /// <summary>
    /// 启动失败
    /// </summary>
    [EnumMember]
    StartFailed = 2,

    /// <summary>
    /// 已启动
    /// </summary>
    [EnumMember]
    Started = 3,

    /// <summary>
    /// 正在停止
    /// </summary>
    [EnumMember]
    Stopping = 4,

    /// <summary>
    /// 停止失败
    /// </summary>
    [EnumMember]
    StopFailed = 5,

    /// <summary>
    /// 已停止
    /// </summary>
    [EnumMember]
    Stopped = 6,

    /// <summary>
    /// 正在重启
    /// </summary>
    [EnumMember]
    Restarting = 7,
  }
}
