
namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 树接口，存储对象二进制序列化值
  /// </summary>
  public interface IObjectTree : IIndexTree
  {
    /// <summary>
    /// Gets or sets the <see cref="System.Object"/> with the specified key.
    /// </summary>
    object this[string key] { get; set; }
  }
}
