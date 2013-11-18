using System;
using System.Diagnostics;

namespace Gimela.Infrastructure.Patterns
{
  public static class BitConverterExtensions
  {
    public static string BytesToString(byte[] data)
    {
      return BitConverter.ToString(data).Replace("-", string.Empty);
    }
  }
}
