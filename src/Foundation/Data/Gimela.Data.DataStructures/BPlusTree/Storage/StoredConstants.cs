
namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 用于存储的常量
  /// </summary>
  internal static class StoredConstants
  {
    /// <summary>
    /// 无效块序号(-1)
    /// </summary>
    public const int NullBlockNumber = -1;

    /// <summary>
    /// 默认Short类型存储的长度(2 Bytes)
    /// </summary>
    public const int ShortLength = 2;
    /// <summary>
    /// 默认Int类型存储的长度(4 Bytes)
    /// </summary>
    public const int IntegerLength = 4;
    /// <summary>
    /// 默认Long类型存储的长度(8 Bytes)
    /// </summary>
    public const int LongLength = 8;

    /// <summary>
    /// 块的大小的最小值(Bytes)
    /// </summary>
    public const int MinBlockSize = 20;

    /// <summary>
    /// 块存储头部前缀长度的最小值(Bytes)
    /// </summary>
    public const int MinFileHeaderPrefixLength = 3;

    /// <summary>
    /// 树文件序列化存储的头部前缀
    /// </summary>
    public static readonly byte[] TreeFileHeaderPrefix = { (byte)'g', (byte)'i', (byte)'m', (byte)'e', (byte)'l', (byte)'a' };

    /// <summary>
    /// 块文件序列化存储的头部前缀
    /// </summary>
    public static readonly byte[] BlockFileHeaderPrefix = { (byte)'g', (byte)'b', (byte)'l', (byte)'o', (byte)'c', (byte)'k' };

    /// <summary>
    /// 链式块文件序列化存储的头部前缀
    /// </summary>
    public static readonly byte[] LinkedFileHeaderPrefix = { (byte)'g', (byte)'c', (byte)'h', (byte)'u', (byte)'n', (byte)'k' };

    /// <summary>
    /// 树中节点的最小容量
    /// </summary>
    public const int MinNodeCapacity = 2;

    /// <summary>
    /// 树中键的最小长度(Bytes)
    /// </summary>
    public const int MinKeyLength = 5;
  }
}
