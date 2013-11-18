using System;
using System.Globalization;
using System.Net;

namespace Gimela.Net.Sockets
{
  /// <summary>
  /// 与服务器的连接已断开事件参数
  /// </summary>
  public class TcpServerDisconnectedEventArgs : EventArgs
  {
    /// <summary>
    /// 与服务器的连接已断开事件参数
    /// </summary>
    /// <param name="ipAddresses">服务器IP地址列表</param>
    /// <param name="port">服务器端口</param>
    public TcpServerDisconnectedEventArgs(IPAddress[] ipAddresses, int port)
    {
      if (ipAddresses == null)
        throw new ArgumentNullException("ipAddresses");

      this.Addresses = ipAddresses;
      this.Port = port;
    }

    /// <summary>
    /// 服务器IP地址列表
    /// </summary>
    public IPAddress[] Addresses { get; private set; }
    /// <summary>
    /// 服务器端口
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      string s = string.Empty;
      foreach (var item in Addresses)
      {
        s = s + item.ToString() + ',';
      }
      s = s.TrimEnd(',');
      s = s + ":" + Port.ToString(CultureInfo.InvariantCulture);

      return s;
    }
  }
}
