using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.Contracts.FaultContracts
{
  /// <summary>
  /// 设备连接服务故障
  /// </summary>
  [DataContract]
  public class DeviceConnectorServiceFault
  {
    /// <summary>
    /// 设备连接服务故障
    /// </summary>
    /// <param name="faultMessage">故障信息</param>
    public DeviceConnectorServiceFault(string faultMessage)
    {
      FaultMessage = faultMessage;
    }

    /// <summary>
    /// 设备连接服务故障
    /// </summary>
    /// <param name="faultMessage">故障信息</param>
    /// <param name="innerException">内部异常</param>
    public DeviceConnectorServiceFault(string faultMessage, Exception innerException)
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
