using System.Runtime.Serialization;

namespace Gimela.ServiceModel.ChannelManagement.ServiceIdentity
{
  /// <summary>
  /// 定制化的消息头
  /// </summary>
  [DataContract]
  public class CustomizedMessageHeaderData
  {
    /// <summary>
    /// 定制化的消息头
    /// </summary>
    public CustomizedMessageHeaderData()
    {
    }

    /// <summary>
    /// 会话Id
    /// </summary>
    [DataMember(Name = "SessionId", IsRequired = true, Order = 0)]
    public string SessionId { get; set; }

    /// <summary>
    /// 客户端用户名
    /// </summary>
    [DataMember(Name = "ClientUserName", IsRequired = true, Order = 10)]
    public string ClientUserName { get; set; }

    /// <summary>
    /// 客户端主机身份标识符
    /// </summary>
    [DataMember(Name = "ClientWorkstationIdentifier", IsRequired = true, Order = 20)]
    public string ClientWorkstationIdentifier { get; set; }

    /// <summary>
    /// 服务端主机身份标识符
    /// </summary>
    [DataMember(Name = "ServiceWorkstationIdentifier", IsRequired = true, Order = 30)]
    public string ServiceWorkstationIdentifier { get; set; }
  }
}
