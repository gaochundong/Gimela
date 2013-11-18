
namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// B+树节点指示符
  /// </summary>
  internal enum BPlusTreeNodeIndicator : byte
  {
    /// <summary>
    /// 内部节点
    /// </summary>
    Internal = 0,
    /// <summary>
    /// 叶子节点
    /// </summary>
    Leaf = 1,
    /// <summary>
    /// 空闲节点
    /// </summary>
    Free = 2
  }
}
