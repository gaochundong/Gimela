
namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 树接口，存储字符串值
  /// </summary>
  public interface IStringTree : IIndexTree
  {
    /// <summary>
    /// Gets or sets the <see cref="System.String"/> with the specified key.
    /// </summary>
    string this[string key] { get; set; }
  }
}
