
namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 数组帮助类
  /// </summary>
  internal static class StoredHelper
  {
    /// <summary>
    /// 将值存储到指定的数组位置
    /// </summary>
    /// <param name="value">被存储的值</param>
    /// <param name="toArray">指定的数组</param>
    /// <param name="atIndex">值在数组中的位置</param>
    public static void Store(int value, byte[] toArray, int atIndex)
    {
      int limit = StoredConstants.IntegerLength;

      if (atIndex + limit > toArray.Length)
      {
        throw new BPlusTreeException("can't access beyond end of array");
      }

      for (int i = 0; i < limit; i++)
      {
        byte thebyte = (byte)(value & 0xff);
        toArray[atIndex + i] = thebyte;
        value = value >> 8;
      }
    }

    /// <summary>
    /// 将值存储到指定的数组位置
    /// </summary>
    /// <param name="value">被存储的值</param>
    /// <param name="toArray">指定的数组</param>
    /// <param name="atIndex">值在数组中的位置</param>
    public static void Store(long value, byte[] toArray, int atIndex)
    {
      int limit = StoredConstants.LongLength;

      if (atIndex + limit > toArray.Length)
      {
        throw new BPlusTreeException("can't access beyond end of array");
      }

      for (int i = 0; i < limit; i++)
      {
        byte thebyte = (byte)(value & 0xff);
        toArray[atIndex + i] = thebyte;
        value = value >> 8;
      }
    }

    /// <summary>
    /// 将值存储到指定的数组位置
    /// </summary>
    /// <param name="value">被存储的值</param>
    /// <param name="toArray">指定的数组</param>
    /// <param name="atIndex">值在数组中的位置</param>
    public static void Store(short value, byte[] toArray, int atIndex)
    {
      int limit = StoredConstants.ShortLength;

      int theInt = value;
      if (atIndex + limit > toArray.Length)
      {
        throw new BlockFileException("can't access beyond end of array");
      }

      for (int i = 0; i < limit; i++)
      {
        byte thebyte = (byte)(theInt & 0xff);
        toArray[atIndex + i] = thebyte;
        theInt = theInt >> 8;
      }
    }

    /// <summary>
    /// 从指定的数组位置获取值
    /// </summary>
    /// <param name="fromArray">指定的数组</param>
    /// <param name="atIndex">值在数组中的位置</param>
    /// <returns>获取到的值</returns>
    public static int RetrieveInt(byte[] fromArray, int atIndex)
    {
      int limit = StoredConstants.IntegerLength;

      if (atIndex + limit > fromArray.Length)
      {
        throw new BlockFileException("can't access beyond end of array");
      }

      int result = 0;
      for (int i = 0; i < limit; i++)
      {
        byte thebyte = fromArray[atIndex + limit - i - 1];
        result = result << 8;
        result = result | thebyte;
      }

      return result;
    }

    /// <summary>
    /// 从指定的数组位置获取值
    /// </summary>
    /// <param name="fromArray">指定的数组</param>
    /// <param name="atIndex">值在数组中的位置</param>
    /// <returns>获取到的值</returns>
    public static long RetrieveLong(byte[] fromArray, int atIndex)
    {
      int limit = StoredConstants.LongLength;

      if (atIndex + limit > fromArray.Length)
      {
        throw new BlockFileException("can't access beyond end of array");
      }

      long result = 0;
      for (int i = 0; i < limit; i++)
      {
        byte thebyte = fromArray[atIndex + limit - i - 1];
        result = result << 8;
        result = result | thebyte;
      }

      return result;
    }

    /// <summary>
    /// 从指定的数组位置获取值
    /// </summary>
    /// <param name="fromArray">指定的数组</param>
    /// <param name="atIndex">值在数组中的位置</param>
    /// <returns>获取到的值</returns>
    public static short RetrieveShort(byte[] fromArray, int atIndex)
    {
      int limit = StoredConstants.ShortLength;

      if (atIndex + limit > fromArray.Length)
      {
        throw new BlockFileException("can't access beyond end of array");
      }

      int result = 0;
      for (int i = 0; i < limit; i++)
      {
        byte thebyte = fromArray[atIndex + limit - i - 1];
        result = (result << 8);
        result = result | thebyte;
      }

      return (short)result;
    }
  }
}
