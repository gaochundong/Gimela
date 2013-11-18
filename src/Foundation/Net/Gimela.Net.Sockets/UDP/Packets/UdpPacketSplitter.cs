using System;
using System.Collections.Generic;

namespace Gimela.Net.Sockets
{
  /// <summary>
  /// UDP数据包分割器
  /// </summary>
  public static class UdpPacketSplitter
  {
    /// <summary>
    /// 分割UDP数据包
    /// </summary>
    /// <param name="sequence">UDP数据包所持有的序号</param>
    /// <param name="datagram">被分割的UDP数据包</param>
    /// <param name="chunkLength">分割块的长度</param>
    /// <returns>
    /// 分割后的UDP数据包列表
    /// </returns>
    public static ICollection<UdpPacket> Split(long sequence, byte[] datagram, int chunkLength)
    {
      if (datagram == null)
        throw new ArgumentNullException("datagram");

      List<UdpPacket> packets = new List<UdpPacket>();

      int chunks = datagram.Length / chunkLength;
      int remainder = datagram.Length % chunkLength;
      int total = chunks;
      if (remainder > 0) total++;

      for (int i = 1; i <= chunks; i++)
      {
        byte[] chunk = new byte[chunkLength];
        Buffer.BlockCopy(datagram, (i - 1) * chunkLength, chunk, 0, chunkLength);
        packets.Add(new UdpPacket(sequence, total, i, chunk, chunkLength));
      }
      if (remainder > 0)
      {
        int length = datagram.Length - (chunkLength * chunks);
        byte[] chunk = new byte[length];
        Buffer.BlockCopy(datagram, chunkLength * chunks, chunk, 0, length);
        packets.Add(new UdpPacket(sequence, total, total, chunk, length));
      }

      return packets;
    }
  }
}
