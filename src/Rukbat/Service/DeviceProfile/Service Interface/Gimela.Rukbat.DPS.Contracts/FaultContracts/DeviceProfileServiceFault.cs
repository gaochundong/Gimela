using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.DPS.Contracts.FaultContracts
{
  /// <summary>
  /// 设备信息服务故障
  /// </summary>
  [DataContract]
  public class DeviceProfileServiceFault
  {
    /// <summary>
    /// 设备信息服务故障
    /// </summary>
    /// <param name="faultMessage">故障信息</param>
    public DeviceProfileServiceFault(string faultMessage)
    {
      FaultMessage = faultMessage;
    }

    /// <summary>
    /// 设备信息服务故障
    /// </summary>
    /// <param name="faultMessage">故障信息</param>
    /// <param name="innerException">内部异常</param>
    public DeviceProfileServiceFault(string faultMessage, Exception innerException)
    {
      FaultMessage = faultMessage;
      InnerException = innerException;
    }

    /// <summary>
    /// 故障信息
    /// </summary>
    [DataMember]
    public string FaultMessage { get; private set; }

    /// <summary>
    /// 内部异常
    /// </summary>
    [DataMember]
    public Exception InnerException { get; private set; }
  }
}
