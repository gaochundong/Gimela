using System;
using System.IO;
using System.Text;

namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// B+树，固定长度字符串为键，映射至字符串。
  /// </summary>
  public class BPlusTree : IStringTree
  {
    #region Fields

    /// <summary>
    /// 绑定到的B+树
    /// </summary>
    private BPlusTreeBytes tree;

    #endregion

    #region Ctors

    /// <summary>
    /// B+树，固定长度字符串为键，映射至字符串。
    /// </summary>
    /// <param name="tree">绑定到的B+树</param>
    internal BPlusTree(BPlusTreeBytes tree)
    {
      if (tree == null)
        throw new ArgumentNullException("tree");
      this.tree = tree;
    }

    #endregion

    #region File Stream

    /// <summary>
    /// 创建B+树
    /// </summary>
    /// <param name="treeFileName">指定树文件的文件名</param>
    /// <param name="blockFileName">指定块文件的文件名</param>
    /// <param name="keyLength">树允许的键的长度</param>
    /// <param name="nodeCapacity">树允许的各子节点的最大容量</param>
    /// <param name="blockSize">块大小</param>
    /// <returns>B+树</returns>
    public static BPlusTree Create(string treeFileName, string blockFileName, int keyLength, int nodeCapacity, int blockSize)
    {
      BPlusTreeBytes tree = BPlusTreeBytes.Create(treeFileName, blockFileName, keyLength, nodeCapacity, blockSize);
      return new BPlusTree(tree);
    }

    /// <summary>
    /// 创建B+树
    /// </summary>
    /// <param name="treeFileName">指定树文件的文件名</param>
    /// <param name="blockFileName">指定块文件的文件名</param>
    /// <param name="keyLength">树允许的键的长度</param>
    /// <returns>B+树</returns>
    public static BPlusTree Create(string treeFileName, string blockFileName, int keyLength)
    {
      BPlusTreeBytes tree = BPlusTreeBytes.Create(treeFileName, blockFileName, keyLength);
      return new BPlusTree(tree);
    }

    /// <summary>
    /// 创建B+树
    /// </summary>
    /// <param name="treeFileStream">指定树文件的文件流</param>
    /// <param name="blockFileStream">指定块文件的文件流</param>
    /// <param name="keyLength">树允许的键的长度</param>
    /// <param name="nodeCapacity">树允许的各子节点的最大容量</param>
    /// <param name="blockSize">块大小</param>
    /// <returns>B+树</returns>
    public static BPlusTree Create(Stream treeFileStream, Stream blockFileStream, int keyLength, int nodeCapacity, int blockSize)
    {
      BPlusTreeBytes tree = BPlusTreeBytes.Create(treeFileStream, blockFileStream, keyLength, nodeCapacity, blockSize);
      return new BPlusTree(tree);
    }

    /// <summary>
    /// 创建B+树
    /// </summary>
    /// <param name="treeFileStream">指定树文件的文件流</param>
    /// <param name="blockFileStream">指定块文件的文件流</param>
    /// <param name="keyLength">树允许的键的长度</param>
    /// <returns>B+树</returns>
    public static BPlusTree Create(Stream treeFileStream, Stream blockFileStream, int keyLength)
    {
      BPlusTreeBytes tree = BPlusTreeBytes.Create(treeFileStream, blockFileStream, keyLength);
      return new BPlusTree(tree);
    }

    /// <summary>
    /// 从文件打开B+树
    /// </summary>
    /// <param name="treeFileStream">指定树文件的文件流</param>
    /// <param name="blockFileStream">指定块文件的文件流</param>
    /// <returns>B+树</returns>
    public static BPlusTree Open(Stream treeFileStream, Stream blockFileStream)
    {
      BPlusTreeBytes tree = BPlusTreeBytes.Open(treeFileStream, blockFileStream);
      return new BPlusTree(tree);
    }

    /// <summary>
    /// 从文件打开B+树
    /// </summary>
    /// <param name="treeFileName">指定树文件的文件名</param>
    /// <param name="blockFileName">指定块文件的文件名</param>
    /// <returns>B+树</returns>
    public static BPlusTree Open(string treeFileName, string blockFileName)
    {
      BPlusTreeBytes tree = BPlusTreeBytes.Open(treeFileName, blockFileName);
      return new BPlusTree(tree);
    }

    /// <summary>
    /// 从文件打开B+树，该树为只读树。
    /// </summary>
    /// <param name="treeFileName">指定树文件的文件名</param>
    /// <param name="blockFileName">指定块文件的文件名</param>
    /// <returns>B+树</returns>
    public static BPlusTree OpenReadOnly(string treeFileName, string blockFileName)
    {
      BPlusTreeBytes tree = BPlusTreeBytes.OpenReadOnly(treeFileName, blockFileName);
      return new BPlusTree(tree);
    }

    #endregion

    #region ITreeIndex Members

    /// <summary>
    /// 在结构中映射键值对。
    /// Map the key to the item in the structure.
    /// </summary>
    /// <param name="key">the key</param>
    /// <param name="value">the item	(must coerce to the appropriate item for the tree instance).</param>
    public void Set(string key, object value)
    {
      string map = value as string;
      if (map == null)
      {
        throw new BPlusTreeException("Tree only stores strings as values.");
      }
      
      byte[] bytes = Encoding.UTF8.GetBytes(map);
      this.tree.Set(key, bytes);
    }

    /// <summary>
    /// 检索键关联的对象。
    /// Get the object associated with the key.
    /// </summary>
    /// <param name="key">key to retrieve.</param>
    /// <returns>the mapped item boxed as an object</returns>
    public object Get(string key)
    {
      object value = this.tree.Get(key);
      byte[] bytes = value as byte[];
      if (bytes != null)
      {
        return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
      }
      throw new BPlusTreeKeyNotFoundException("key not found " + key);
    }

    /// <summary>
    /// 由树决定键的比较规则
    /// </summary>
    /// <param name="left">节点的键</param>
    /// <param name="right">节点的键</param>
    /// <returns>节点的键比较结果</returns>
    public int Compare(string left, string right)
    {
      return this.tree.Compare(left, right);
    }

    /// <summary>
    /// 获取结构中最小的键。
    /// Get the least key in the structure.
    /// </summary>
    /// <returns>least key item or null if the tree is empty.</returns>
    public string FirstKey()
    {
      return this.tree.FirstKey();
    }

    /// <summary>
    /// 获取比指定的键稍大的键。如果无此类键则返回空。
    /// Get the least key in the structure strictly "larger" than the argument.  Return null if there is no such key.
    /// </summary>
    /// <param name="afterThisKey">The "lower limit" for the item to return</param>
    /// <returns>Least key greater than argument or null</returns>
    public string NextKey(string afterThisKey)
    {
      return this.tree.NextKey(afterThisKey);
    }

    /// <summary>
    /// 判断结构中是否存在此键。
    /// Return true if the key is present in the structure.
    /// </summary>
    /// <param name="key">Key to test</param>
    /// <returns>true if present, otherwise false.</returns>
    public bool ContainsKey(string key)
    {
      return this.tree.ContainsKey(key);
    }

    /// <summary>
    /// 弃用指定的键和关联的值。如果键未找到则抛出异常。
    /// Dispose of the key and its associated item.  Throw an exception if the key is missing.
    /// </summary>
    /// <param name="key">Key to erase.</param>
    public void RemoveKey(string key)
    {
      this.tree.RemoveKey(key);
    }

    /// <summary>
    /// 设置一个参数，用于决定何时释放内存映射的块。
    /// Set a parameter used to decide when to release memory mapped blocks.
    /// Larger values mean that more memory is used but accesses may be faster
    /// especially if there is locality of reference.  5 is too small and 1000
    /// may be too big.
    /// </summary>
    /// <param name="limit">maximum number of leaves with no materialized children to keep in memory.</param>
    public void SetFootprintLimit(int limit)
    {
      this.tree.SetFootprintLimit(limit);
    }

    /// <summary>
    /// 提交更改，将上次提交后的所有更改持久化。
    /// Make changes since the last commit permanent.
    /// </summary>
    public void Commit()
    {
      this.tree.Commit();
    }

    /// <summary>
    /// 丢弃自上次提交后的所有更改。
    /// Discard changes since the last commit and return to the state at the last commit point.
    /// </summary>
    public void Abort()
    {
      this.tree.Abort();
    }

    /// <summary>
    /// 关闭并刷新流，而未进行Commit或Abort。
    /// Close and flush the streams without committing or aborting.
    /// (This is equivalent to abort, except unused space in the streams may be left unreachable).
    /// </summary>
    public void Shutdown()
    {
      this.tree.Shutdown();
    }

    /// <summary>
    /// 检测并尝试回收再利用不可达的空间。当一块空间被修改后未进行Commit或Abort操作可能导致空间不可达。
    /// Examine the structure and optionally try to reclaim unreachable space.  A structure which was modified without a
    /// concluding commit or abort may contain unreachable space.
    /// </summary>
    /// <param name="correctErrors">如果为真则确认异常为可预知的，如果为假则当发生错误是抛出异常</param>
    public void Recover(bool correctErrors)
    {
      this.tree.Recover(correctErrors);
    }

    #endregion

    #region Index

    /// <summary>
    /// Gets or sets the <see cref="System.String"/> with the specified key.
    /// </summary>
    public string this[string key]
    {
      get
      {
        object map = this.tree.Get(key);
        byte[] bytes = map as byte[];
        if (bytes != null)
        {
          return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
        throw new BPlusTreeKeyNotFoundException("key not found " + key);
      }
      set
      {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        this.tree.Set(key, bytes);
      }
    }

    #endregion

    #region ToString

    /// <summary>
    /// 将树转成字符串描述
    /// </summary>
    /// <returns>树的字符串描述</returns>
    public virtual string ToText()
    {
      return this.tree.ToText();
    }

    #endregion
  }
}
