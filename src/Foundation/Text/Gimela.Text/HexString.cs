using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Text
{
  /// <summary>
  /// 十六进制字符串转换成二进制数组
  /// </summary>
  public static class HexString
  {
    /// <summary>
    /// 十六进制字符串转换成二进制数组
    /// </summary>
    /// <param name="hex">十六进制字符串</param>
    /// <returns>二进制数组</returns>
    public static byte[] ToBytes(string hex)
    {
      if (string.IsNullOrEmpty(hex))
        throw new ArgumentNullException("hex");

      string fixedHex = hex.Replace("-", string.Empty);

      // array to put the result in
      byte[] bytes = new byte[fixedHex.Length / 2];
      // variable to determine shift of high/low nibble
      int shift = 4;
      // offset of the current byte in the array
      int offset = 0;
      // loop the characters in the string
      foreach (char c in fixedHex)
      {
        // get character code in range 0-9, 17-22
        // the % 32 handles lower case characters
        int b = (c - '0') % 32;
        // correction for a-f
        if (b > 9) b -= 7;
        // store nibble (4 bits) in byte array
        bytes[offset] |= (byte)(b << shift);
        // toggle the shift variable between 0 and 4
        shift ^= 4;
        // move to next byte
        if (shift != 0) offset++;
      }

      return bytes;
    }

    /// <summary>
    /// 二进制数组转换成十六进制字符串
    /// </summary>
    /// <param name="value">二进制数组</param>
    /// <returns>十六进制字符串</returns>
    public static string FromBytes(byte[] value)
    {
      return FromBytes(value, false);
    }

    /// <summary>
    /// 二进制数组转换成十六进制字符串
    /// </summary>
    /// <param name="value">二进制数组</param>
    /// <param name="removeDashes">删除横线</param>
    /// <returns>十六进制字符串</returns>
    public static string FromBytes(byte[] value, bool removeDashes)
    {
      string hex = BitConverter.ToString(value);
      if (removeDashes)
        hex = hex.Replace("-", "");

      return hex;
    }
  }
}
