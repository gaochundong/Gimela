using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.MPS.Contracts.FaultContracts
{
  /// <summary>
  /// 媒体发布服务故障
  /// </summary>
  [DataContract]
  public class MediaPublisherServiceFault
  {
    /// <summary>
    /// 媒体发布服务故障
    /// </summary>
    /// <param name="faultMessage">故障信息</param>
    public MediaPublisherServiceFault(string faultMessage)
    {
      FaultMessage = faultMessage;
    }

    /// <summary>
    /// 媒体发布服务故障
    /// </summary>
    /// <param name="faultMessage">故障信息</param>
    /// <param name="innerException">内部异常</param>
    public MediaPublisherServiceFault(string faultMessage, Exception innerException)
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
