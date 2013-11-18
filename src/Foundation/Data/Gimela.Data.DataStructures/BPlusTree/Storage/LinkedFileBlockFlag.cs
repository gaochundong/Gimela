
namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 链式文件块标记
  /// </summary>
  internal enum LinkedFileBlockFlag : byte
  {
    /// <summary>
    /// 块空闲
    /// </summary>
    Free = 0,
    /// <summary>
    /// 该块为链式文件的头块
    /// </summary>
    Head = 1,
    /// <summary>
    /// 该块为链式文件的体块
    /// </summary>
    Body = 2
  }
}
