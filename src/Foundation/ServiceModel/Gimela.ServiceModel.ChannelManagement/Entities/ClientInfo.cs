using System;
using System.Globalization;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 客户端描述，用于记录回调契约实例描述
  /// </summary>
  public class ClientInfo<TCallbackContract>
  {
    /// <summary>
    /// 客户端描述
    /// </summary>
    /// <param name="clientCallback">客户端回调实例</param>
    public ClientInfo(TCallbackContract clientCallback)
    {
      TransactionId = Guid.NewGuid();
      ClientCallback = clientCallback;
    }

    /// <summary>
    /// 客户端描述
    /// </summary>
    /// <param name="clientCallback">客户端回调实例</param>
    /// <param name="clientHostName">客户端主机名</param>
    /// <param name="clientName">客户端名称</param>
    public ClientInfo(TCallbackContract clientCallback, string clientHostName, string clientName)
      : this(clientCallback)
    {
      ClientHostName = clientHostName;
      ClientName = clientName;
    }

    /// <summary>
    /// 客户端描述
    /// </summary>
    /// <param name="clientCallback">客户端回调实例</param>
    /// <param name="clientData">客户端描述数据</param>
    public ClientInfo(TCallbackContract clientCallback, ClientData clientData)
      : this(clientCallback)
    {
      if (clientData == null)
        throw new ArgumentNullException("clientData");
      ClientHostName = clientData.ClientHostName;
      ClientName = clientData.ClientName;
    }

    /// <summary>
    /// 客户端主机名
    /// </summary>
    public string ClientHostName { get; set; }

    /// <summary>
    /// 客户端名称
    /// </summary>
    public string ClientName { get; set; }

    /// <summary>
    /// 客户端回调实例
    /// </summary>
    public TCallbackContract ClientCallback { get; private set; }

    /// <summary>
    /// 事务Id
    /// </summary>
    public Guid TransactionId { get; private set; }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      string keyFormat = "{0}.{1}.{2}.{3}"; // {ClientHostName}.{ClientName}.{ClientCallback}.{TransactionId}
      return string.Format(CultureInfo.InvariantCulture, keyFormat, ClientHostName, ClientName, ClientCallback.GetType().FullName, TransactionId);
    }
  }
}
