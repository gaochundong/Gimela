using System;
using System.Runtime.InteropServices;

namespace Gimela.Media.Utilities
{
  /// <summary>
  /// 非托管内存帮助类
  /// </summary>
  public static class UnmanagedMemoryHelper
  {
    /// <summary>
    /// 拷贝非托管内存块
    /// </summary>
    /// <param name="dst">目的地址指针</param>
    /// <param name="src">源地址指针</param>
    /// <param name="count">内存块长度</param>
    /// <returns>返回目的地址指针</returns>
    public static IntPtr CopyUnmanagedMemory(IntPtr dst, IntPtr src, int count)
    {
      unsafe
      {
        CopyUnmanagedMemory((byte*)dst.ToPointer(), (byte*)src.ToPointer(), count);
      }
      return dst;
    }

    /// <summary>
    /// 拷贝非托管内存块
    /// </summary>
    /// <param name="dst">目的地址指针</param>
    /// <param name="src">源地址指针</param>
    /// <param name="count">内存块长度</param>
    /// <returns>返回目的地址指针</returns>
    public static unsafe byte* CopyUnmanagedMemory(byte* dst, byte* src, int count)
    {
#if !MONO
      return memcpy(dst, src, count);
#else
      int countUint = count >> 2;
      int countByte = count & 3;

      uint* dstUint = (uint*) dst;
      uint* srcUint = (uint*) src;

      while ( countUint-- != 0 )
      {
        *dstUint++ = *srcUint++;
      }

      byte* dstByte = (byte*) dstUint;
      byte* srcByte = (byte*) srcUint;

      while ( countByte-- != 0 )
      {
        *dstByte++ = *srcByte++;
      }
      return dst;
#endif
    }

    /// <summary>
    /// 使用指定的值填充非托管内存
    /// </summary>
    /// <param name="dst">目的地址指针</param>
    /// <param name="filler">填充的值</param>
    /// <param name="count">填充的内存块长度</param>
    /// <returns>返回目的地址指针</returns>
    public static IntPtr SetUnmanagedMemory(IntPtr dst, int filler, int count)
    {
      unsafe
      {
        SetUnmanagedMemory((byte*)dst.ToPointer(), filler, count);
      }
      return dst;
    }

    /// <summary>
    /// 使用指定的值填充非托管内存
    /// </summary>
    /// <param name="dst">目的地址指针</param>
    /// <param name="filler">填充的值</param>
    /// <param name="count">填充的内存块长度</param>
    /// <returns>返回目的地址指针</returns>
    public static unsafe byte* SetUnmanagedMemory(byte* dst, int filler, int count)
    {
#if !MONO
      return memset(dst, filler, count);
#else
      int countUint = count >> 2;
      int countByte = count & 3;

      byte fillerByte = (byte) filler;
      uint fiilerUint = (uint) filler | ( (uint) filler << 8 ) |
                                        ( (uint) filler << 16 );// |
                                        //( (uint) filler << 24 );

      uint* dstUint = (uint*) dst;

      while ( countUint-- != 0 )
      {
        *dstUint++ = fiilerUint;
      }

      byte* dstByte = (byte*) dstUint;

      while ( countByte-- != 0 )
      {
        *dstByte++ = fillerByte;
      }
      return dst;
#endif
    }


#if !MONO
    /// <summary>
    /// Win32内存拷贝函数
    /// </summary>
    /// <param name="dst"></param>
    /// <param name="src"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [DllImport("ntdll.dll", CallingConvention = CallingConvention.Cdecl)]
    private static unsafe extern byte* memcpy(byte* dst, byte* src, int count);
    /// <summary>
    /// Win32内存设置函数
    /// </summary>
    /// <param name="dst"></param>
    /// <param name="filler"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [DllImport("ntdll.dll", CallingConvention = CallingConvention.Cdecl)]
    private static unsafe extern byte* memset(byte* dst, int filler, int count);
#endif
  }
}
