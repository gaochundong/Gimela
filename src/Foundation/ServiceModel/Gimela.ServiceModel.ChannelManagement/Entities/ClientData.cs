using System.Runtime.Serialization;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 客户端描述数据，改数据通常由服务请求端在请求消息中携带
  /// </summary>
  [DataContract]
  public class ClientData
  {
    /// <summary>
    /// 客户端主机名
    /// </summary>
    [DataMember]
    public string ClientHostName { get; set; }

    /// <summary>
    /// 客户端名称
    /// </summary>
    [DataMember]
    public string ClientName { get; set; }
  }
}
