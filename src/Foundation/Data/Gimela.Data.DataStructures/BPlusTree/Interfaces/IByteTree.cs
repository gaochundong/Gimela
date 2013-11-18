
namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 树接口，存储Byte数组值
  /// </summary>
  public interface IByteTree : IIndexTree
  {
    /// <summary>
    /// Gets or sets the byte array with the specified key.
    /// </summary>
    byte[] this[string key] { get; set; }
  }
}
