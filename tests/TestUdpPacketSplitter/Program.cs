using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Gimela.Net.Sockets;
using Gimela.Common.Logging;

namespace TestUdpPacketSplitter
{
  class Program
  {
    static void Main(string[] args)
    {
      LogFactory.Assign(new ConsoleLogFactory());

      byte[] datagram = new byte[277777];
      for (int i = 0; i < datagram.Length; i++)
      {
        datagram[i] = 1;
      }

      ICollection<UdpPacket> packets = UdpPacketSplitter.Split(1024, datagram, 65535);
      foreach (var packet in packets)
      {
        Console.WriteLine(packet.ToString());
      }

      foreach (var packet in packets)
      {
        byte[] data = packet.ToArray();
        UdpPacket convert = UdpPacket.FromArray(data);
        Console.WriteLine(convert.ToString());
      }

      Console.ReadKey();
    }
  }
}
