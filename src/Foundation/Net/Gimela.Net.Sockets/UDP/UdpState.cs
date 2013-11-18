using System.Net;
using System.Net.Sockets;

namespace Gimela.Net.Sockets
{
  internal class UdpState
  {
    public UdpState(UdpClient udpClient, IPEndPoint remoteEndPoint)
    {
      UdpClient = udpClient;
      RemoteEndPoint = remoteEndPoint;
    }

    public UdpClient UdpClient { get; private set; }
    public IPEndPoint RemoteEndPoint { get; private set; }
  }
}
