using System;
using System.Globalization;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 服务契约描述
  /// </summary>
  public class ContractInfo
  {
    /// <summary>
    /// 服务契约描述
    /// </summary>
    /// <param name="contractType">服务契约类型</param>
    public ContractInfo(Type contractType)
    {
      ContractType = contractType;
    }

    /// <summary>
    /// 服务契约描述
    /// </summary>
    /// <param name="contractType">服务契约类型</param>
    /// <param name="hostName">主机名称</param>
    public ContractInfo(Type contractType, string hostName)
      : this(contractType)
    {
      HostName = hostName;
    }

    /// <summary>
    /// 服务契约类型
    /// </summary>
    public Type ContractType { get; private set; }

    /// <summary>
    /// 主机名称
    /// </summary>
    public string HostName { get; private set; }

    /// <summary>
    /// 服务契约唯一识别键值
    /// </summary>
    public string Key
    {
      get
      {
        return this.ToString();
      }
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      string keyFormat = "{0}#{1}"; // {ContractTypeName}#{HostName}
      return string.Format(CultureInfo.InvariantCulture, keyFormat, ContractType.FullName, HostName);
    }
  }
}
