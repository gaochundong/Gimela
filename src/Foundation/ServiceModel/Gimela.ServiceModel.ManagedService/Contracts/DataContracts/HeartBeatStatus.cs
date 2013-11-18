using System.Runtime.Serialization;

namespace Gimela.ServiceModel.ManagedService.Contracts.DataContracts
{
  /// <summary>
  /// 心跳状态
  /// </summary>
  [DataContract]
  public enum HeartBeatStatus
  {
    /// <summary>
    /// 未知状态
    /// </summary>
    [EnumMember]
    Unknown = 0,

    /// <summary>
    /// 健康
    /// </summary>
    [EnumMember]
    Healthy = 1,

    /// <summary>
    /// 存在问题
    /// </summary>
    [EnumMember]
    Sick = 2,

    /// <summary>
    /// 已不可用
    /// </summary>
    [EnumMember]
    Dead = 3,
  }
}
