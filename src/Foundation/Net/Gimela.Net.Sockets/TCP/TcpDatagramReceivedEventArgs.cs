using System;
using System.Net.Sockets;

namespace Gimela.Net.Sockets
{
  /// <summary>
  /// 接收到数据报文事件参数
  /// </summary>
  /// <typeparam name="T">报文类型</typeparam>
  public class TcpDatagramReceivedEventArgs<T> : EventArgs
  {
    /// <summary>
    /// 接收到数据报文事件参数
    /// </summary>
    /// <param name="tcpClient">客户端</param>
    /// <param name="datagram">报文</param>
    public TcpDatagramReceivedEventArgs(TcpClient tcpClient, T datagram)
    {
      TcpClient = tcpClient;
      Datagram = datagram;
    }

    /// <summary>
    /// 客户端
    /// </summary>
    public TcpClient TcpClient { get; private set; }
    /// <summary>
    /// 报文
    /// </summary>
    public T Datagram { get; private set; }
  }
}
