using System;
using System.Net;

namespace Gimela.Net.Sockets
{
  /// <summary>
  /// UDP数据报文接收事件参数
  /// </summary>
  /// <typeparam name="T">数据类型</typeparam>
  public class UdpDatagramReceivedEventArgs<T> : EventArgs
  {
    /// <summary>
    /// UDP数据包接收事件参数
    /// </summary>
    /// <param name="remoteEndPoint">远端终结点</param>
    /// <param name="datagram">数据报文</param>
    public UdpDatagramReceivedEventArgs(IPEndPoint remoteEndPoint, T datagram)
    {
      RemoteEndPoint = remoteEndPoint;
      Datagram = datagram;
    }

    /// <summary>
    /// 远端终结点
    /// </summary>
    public IPEndPoint RemoteEndPoint { get; private set; }

    /// <summary>
    /// 数据报文
    /// </summary>
    public T Datagram { get; private set; }
  }
}
