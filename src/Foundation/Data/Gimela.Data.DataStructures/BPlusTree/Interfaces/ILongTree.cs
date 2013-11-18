
namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 树接口，存储长整形值
  /// </summary>
  public interface ILongTree : IIndexTree
  {
    /// <summary>
    /// Gets or sets the <see cref="System.Int64"/> with the specified key.
    /// </summary>
    long this[string key] { get; set; }
  }
}
