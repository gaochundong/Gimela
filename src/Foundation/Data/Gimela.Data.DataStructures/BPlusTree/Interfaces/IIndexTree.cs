
namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 各种树索引的实现接口。每种实现同时支持一个this[key]索引方式。
  /// This is the shared interface among the various tree index implementations.  Each
  /// implementation also supports an indexing notation this[key] which is not included
  /// here because of type incompatibilities.
  /// </summary>
  public interface IIndexTree
  {
    /// <summary>
    /// 在结构中映射键值对。
    /// Map the key to the item in the structure.
    /// </summary>
    /// <param name="key">the key</param>
    /// <param name="value">the item	(must coerce to the appropriate item for the tree instance).</param>
    void Set(string key, object value);

    /// <summary>
    /// 检索键关联的对象。
    /// Get the object associated with the key.
    /// </summary>
    /// <param name="key">key to retrieve.</param>
    /// <returns>the mapped item boxed as an object</returns>
    object Get(string key);

    /// <summary>
    /// 由树决定键的比较规则
    /// </summary>
    /// <param name="left">节点的键</param>
    /// <param name="right">节点的键</param>
    /// <returns>节点的键比较结果</returns>
    int Compare(string left, string right);

    /// <summary>
    /// 获取结构中最小的键。
    /// Get the least key in the structure.
    /// </summary>
    /// <returns>least key item or null if the tree is empty.</returns>
    string FirstKey();

    /// <summary>
    /// 获取比指定的键稍大的键。如果无此类键则返回空。
    /// Get the least key in the structure strictly "larger" than the argument.  Return null if there is no such key.
    /// </summary>
    /// <param name="afterThisKey">The "lower limit" for the item to return</param>
    /// <returns>Least key greater than argument or null</returns>
    string NextKey(string afterThisKey);

    /// <summary>
    /// 判断结构中是否存在此键。
    /// Return true if the key is present in the structure.
    /// </summary>
    /// <param name="key">Key to test</param>
    /// <returns>true if present, otherwise false.</returns>
    bool ContainsKey(string key);

    /// <summary>
    /// 弃用指定的键和关联的值。如果键未找到则抛出异常。
    /// Dispose of the key and its associated item.  Throw an exception if the key is missing.
    /// </summary>
    /// <param name="key">Key to erase.</param>
    void RemoveKey(string key);

    /// <summary>
    /// 设置一个参数，用于决定何时释放内存映射的块。
    /// Set a parameter used to decide when to release memory mapped blocks.
    /// Larger values mean that more memory is used but accesses may be faster
    /// especially if there is locality of reference.  5 is too small and 1000
    /// may be too big.
    /// </summary>
    /// <param name="limit">maximum number of leaves with no materialized children to keep in memory.</param>
    void SetFootprintLimit(int limit);

    /// <summary>
    /// 提交更改，将上次提交后的所有更改持久化。
    /// Make changes since the last commit permanent.
    /// </summary>
    void Commit();

    /// <summary>
    /// 丢弃自上次提交后的所有更改。
    /// Discard changes since the last commit and return to the state at the last commit point.
    /// </summary>
    void Abort();

    /// <summary>
    /// 关闭并刷新流，而未进行Commit或Abort。
    /// Close and flush the streams without committing or aborting.
    /// (This is equivalent to abort, except unused space in the streams may be left unreachable).
    /// </summary>
    void Shutdown();

    /// <summary>
    /// 检测并尝试回收再利用不可达的空间。当一块空间被修改后未进行Commit或Abort操作可能导致空间不可达。
    /// Examine the structure and optionally try to reclaim unreachable space.  A structure which was modified without a
    /// concluding commit or abort may contain unreachable space.
    /// </summary>
    /// <param name="correctErrors">如果为真则确认异常为可预知的，如果为假则当发生错误是抛出异常</param>
    void Recover(bool correctErrors);
  }
}
