using System;
using System.Globalization;
using System.ServiceModel.Channels;

namespace Gimela.ServiceModel.ManagedHosting
{
  /// <summary>
  /// 服务寄宿信息
  /// </summary>
  public class ServiceHostInfo
  {
    /// <summary>
    /// 服务名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 服务IP地址
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// 服务端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 服务绑定类型
    /// </summary>
    public Binding Binding { get; set; }

    /// <summary>
    /// 服务契约类型
    /// </summary>
    public Type Contract { get; set; }

    /// <summary>
    /// 服务实例类型
    /// </summary>
    public Type Service { get; set; }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, 
        "Name[{0}], Address[{1}], Port[{2}], Binding[{3}], Contract[{4}], Service[{5}]",
        Name, Address, Port, Binding.Scheme, Contract.Name, Service.Name);
    }
  }
}
