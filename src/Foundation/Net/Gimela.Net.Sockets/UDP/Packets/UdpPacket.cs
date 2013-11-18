using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Net.Sockets
{
  /// <summary>
  /// UDP数据包
  /// </summary>
  public class UdpPacket
  {
    /// <summary>
    /// UDP数据包
    /// </summary>
    /// <param name="sequence">UDP数据包所在的序号</param>
    /// <param name="total">共拥有UDP数据包的数量</param>
    /// <param name="order">UDP数据包所在的顺序</param>
    /// <param name="payload">UDP数据包有效载荷</param>
    /// <param name="payloadSize">UDP数据包有效载荷长度</param>
    public UdpPacket(long sequence, int total, int order, byte[] payload, int payloadSize)
    {
      this.Sequence = sequence;
      this.Total = total;
      this.Order = order;
      this._payloadSize = payloadSize;
      this._payload = payload;

      _header = new byte[HeaderSize];
      for (int i = 0; i < 4; i++)
      {
        _header[3 - i] = (byte)(Sequence >> (8 * i));
      }
      for (int i = 0; i < 4; i++)
      {
        _header[7 - i] = (byte)(Total >> (8 * i));
      }
      for (int i = 0; i < 4; i++)
      {
        _header[11 - i] = (byte)(Order >> (8 * i));
      }
    }

    /// <summary>
    /// UDP数据包
    /// </summary>
    /// <param name="packet">UDP数据包</param>
    /// <param name="packetSize">UDP数据包长度</param>
    public UdpPacket(byte[] packet, int packetSize)
    {
      if (packet == null)
        throw new ArgumentNullException("packet");

      _header = new byte[HeaderSize];
      for (int i = 0; i < HeaderSize; i++)
      {
        _header[i] = packet[i];
      }
      Sequence = UnsignedInt(_header[3]) + 256 * UnsignedInt(_header[2]) + 65536 * UnsignedInt(_header[1]) + 16777216 * UnsignedInt(_header[0]);
      Total = UnsignedInt(_header[7]) + 256 * UnsignedInt(_header[6]) + 65536 * UnsignedInt(_header[5]) + 16777216 * UnsignedInt(_header[4]);
      Order = UnsignedInt(_header[11]) + 256 * UnsignedInt(_header[10]) + 65536 * UnsignedInt(_header[9]) + 16777216 * UnsignedInt(_header[8]);

      _payloadSize = packetSize - HeaderSize;
      _payload = new byte[_payloadSize];
      for (int i = HeaderSize; i < packetSize; i++)
      {
        _payload[i - HeaderSize] = packet[i];
      }
    }

    #region Properties

    /// <summary>
    /// UDP数据包头字段长度
    /// </summary>
    public const int HeaderSize = 12;
    /// <summary>
    /// UDP数据包头
    /// </summary>
    private byte[] _header;
    /// <summary>
    /// UDP数据包头
    /// </summary>
    public byte[] Header { get { return _header; } }
    /// <summary>
    /// UDP数据包所在的序号
    /// </summary>
    public long Sequence { get; private set; }
    /// <summary>
    /// Sequence共拥有UDP数据包的数量
    /// </summary>
    public int Total { get; private set; }
    /// <summary>
    /// UDP数据包所在Sequence的顺序
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// UDP数据包有效载荷长度
    /// </summary>
    private int _payloadSize;
    /// <summary>
    /// UDP数据包有效载荷长度
    /// </summary>
    public int PayloadSize { get { return _payloadSize; } }
    /// <summary>
    /// UDP数据包有效载荷
    /// </summary>
    private byte[] _payload;
    /// <summary>
    /// UDP数据包有效载荷
    /// </summary>
    public byte[] Payload { get { return _payload; } }

    /// <summary>
    /// UDP数据包总长度，包括Header和Payload
    /// </summary>
    public int Length { get { return HeaderSize + PayloadSize; } }

    #endregion

    #region Methods

    /// <summary>
    /// 将UDP数据包转化成二进制数组
    /// </summary>
    /// <returns>二进制数组</returns>
    public byte[] ToArray()
    {
      byte[] packet = new byte[Length];

      Array.Copy(_header, 0, packet, 0, HeaderSize);
      Array.Copy(_payload, 0, packet, HeaderSize, PayloadSize);

      return packet;
    }

    /// <summary>
    /// 将二进制数组转化成UDP数据包
    /// </summary>
    /// <param name="packet">二进制数组</param>
    /// <returns>UDP数据包</returns>
    public static UdpPacket FromArray(byte[] packet)
    {
      if (packet == null)
        throw new ArgumentNullException("packet");

      return new UdpPacket(packet, packet.Length);
    }

    /// <summary>
    /// return the unsigned value of 8-bit integer nb
    /// </summary>
    /// <param name="nb"></param>
    /// <returns></returns>
    private static int UnsignedInt(int nb)
    {
      if (nb >= 0)
        return (nb);
      else
        return (256 + nb);
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture,
        "Sequence[{0}], Total[{1}], Order[{2}], Length[{3}], HeaderSize[{4}], PayloadSize[{5}]",
        Sequence, Total, Order, Length, HeaderSize, PayloadSize);
    }

    #endregion
  }
}
