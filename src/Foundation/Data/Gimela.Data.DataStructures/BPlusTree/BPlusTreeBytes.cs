using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// B+树，固定长度字符串为键，映射至Byte数组。
  /// </summary>
  public class BPlusTreeBytes : IByteTree
  {
    #region Fields
    
    /// <summary>
    /// 绑定到的B+树
    /// </summary>
    private BPlusTreeLong tree;
    /// <summary>
    /// 绑定到的链式块文件
    /// </summary>
    private LinkedFile archive;
    /// <summary>
    /// 提交时需要释放的块
    /// </summary>
    private Hashtable freeChunksOnCommit = new Hashtable();
    /// <summary>
    /// 放弃时需要释放的块
    /// </summary>
    private Hashtable freeChunksOnAbort = new Hashtable();
    /// <summary>
    /// 默认的树允许的各子节点的最大容量
    /// </summary>
    static int DefaultNodeCapacity = 32;
    /// <summary>
    /// 默认的存储块大小
    /// </summary>
    static int DefaultBlockSize = 1024;    

    #endregion

    #region Ctors

    /// <summary>
    /// B+树，固定长度字符串为键，映射至Byte数组。
    /// </summary>
    /// <param name="tree">B+树</param>
    /// <param name="archive">链式块文件</param>
    internal BPlusTreeBytes(BPlusTreeLong tree, LinkedFile archive)
    {
      this.tree = tree;
      this.archive = archive;
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
    public static BPlusTreeBytes Create(string treeFileName, string blockFileName, int keyLength, int nodeCapacity, int blockSize)
    {
      Stream treeFileStream = new FileStream(treeFileName, FileMode.CreateNew, FileAccess.ReadWrite);
      Stream blockFileStream = new FileStream(blockFileName, FileMode.CreateNew, FileAccess.ReadWrite);
      return Create(treeFileStream, blockFileStream, keyLength, nodeCapacity, blockSize);
    }

    /// <summary>
    /// 创建B+树
    /// </summary>
    /// <param name="treeFileName">指定树文件的文件名</param>
    /// <param name="blockFileName">指定块文件的文件名</param>
    /// <param name="keyLength">树允许的键的长度</param>
    /// <returns>B+树</returns>
    public static BPlusTreeBytes Create(string treeFileName, string blockFileName, int keyLength)
    {
      Stream treeFileStream = new FileStream(treeFileName, FileMode.CreateNew, FileAccess.ReadWrite);
      Stream blockFileStream = new FileStream(blockFileName, FileMode.CreateNew, FileAccess.ReadWrite);
      return Create(treeFileStream, blockFileStream, keyLength);
    }

    /// <summary>
    /// 创建B+树
    /// </summary>
    /// <param name="treeFileStream">指定树文件的文件流</param>
    /// <param name="blockFileStream">指定块文件的文件流</param>
    /// <param name="keyLength">树允许的键的长度</param>
    /// <returns>B+树</returns>
    public static BPlusTreeBytes Create(Stream treeFileStream, Stream blockFileStream, int keyLength)
    {
      return Create(treeFileStream, blockFileStream, keyLength, DefaultNodeCapacity, DefaultBlockSize);
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
    public static BPlusTreeBytes Create(Stream treeFileStream, Stream blockFileStream, int keyLength, int nodeCapacity, int blockSize)
    {
      BPlusTreeLong tree = BPlusTreeLong.InitializeInStream(treeFileStream, (long)0, keyLength, nodeCapacity);
      LinkedFile archive = LinkedFile.InitializeInStream(blockFileStream, (long)0, blockSize);
      return new BPlusTreeBytes(tree, archive);
    }

    /// <summary>
    /// 从文件打开B+树
    /// </summary>
    /// <param name="treeFileName">指定树文件的文件名</param>
    /// <param name="blockFileName">指定块文件的文件名</param>
    /// <param name="access">文件访问权限</param>
    /// <returns>B+树</returns>
    public static BPlusTreeBytes Open(string treeFileName, string blockFileName, FileAccess access)
    {
      Stream treeFileStream = new FileStream(treeFileName, FileMode.Open, access);
      Stream blockFileStream = new FileStream(blockFileName, FileMode.Open, access);
      return Open(treeFileStream, blockFileStream);
    }

    /// <summary>
    /// 从文件打开B+树
    /// </summary>
    /// <param name="treeFileName">指定树文件的文件名</param>
    /// <param name="blockFileName">指定块文件的文件名</param>
    /// <returns>B+树</returns>
    public static BPlusTreeBytes Open(string treeFileName, string blockFileName)
    {
      return Open(treeFileName, blockFileName, FileAccess.ReadWrite);
    }

    /// <summary>
    /// 从文件打开B+树
    /// </summary>
    /// <param name="treeFileStream">指定树文件的文件流</param>
    /// <param name="blockFileStream">指定块文件的文件流</param>
    /// <returns>B+树</returns>
    public static BPlusTreeBytes Open(Stream treeFileStream, Stream blockFileStream)
    {
      BPlusTreeLong tree = BPlusTreeLong.SetupFromExistingStream(treeFileStream, (long)0);
      LinkedFile archive = LinkedFile.SetupFromExistingStream(blockFileStream, (long)0);
      return new BPlusTreeBytes(tree, archive);
    }

    /// <summary>
    /// 从文件打开B+树，该树为只读树。
    /// </summary>
    /// <param name="treeFileName">指定树文件的文件名</param>
    /// <param name="blockFileName">指定块文件的文件名</param>
    /// <returns>B+树</returns>
    public static BPlusTreeBytes OpenReadOnly(string treeFileName, string blockFileName)
    {
      return Open(treeFileName, blockFileName, FileAccess.Read);
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
      byte[] bytes = value as byte[];
      if (bytes == null)
      {
        throw new BPlusTreeBadKeyValueException("tree can only archive byte array as value");
      }
      this[key] = bytes;
    }

    /// <summary>
    /// 检索键关联的对象。
    /// Get the object associated with the key.
    /// </summary>
    /// <param name="key">key to retrieve.</param>
    /// <returns>the mapped item boxed as an object</returns>
    public object Get(string key)
    {
      if (this.tree.ContainsKey(key))
      {
        long valueFound = (long)this.tree.Get(key);
        return (object)this.archive.GetChunk(valueFound);
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
      long map = this.tree[key];

      if (this.freeChunksOnAbort.ContainsKey(map))
      {
        // free it now
        this.freeChunksOnAbort.Remove(map);
        this.archive.ReleaseBlocks(map);
      }
      else
      {
        // free when committed
        this.freeChunksOnCommit[map] = map;
      }

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
      // store all new blocks
      this.archive.Flush();

      // commit the tree
      this.tree.Commit();

      // at this point the new blocks have been committed, now free the old ones
      ArrayList toFree = new ArrayList();
      foreach (DictionaryEntry d in this.freeChunksOnCommit)
      {
        toFree.Add(d.Key);
      }
      toFree.Sort();
      toFree.Reverse();
      foreach (object thing in toFree)
      {
        long chunknumber = (long)thing;
        this.archive.ReleaseBlocks(chunknumber);
      }

      this.archive.Flush();

      this.freeChunksOnCommit.Clear();
      this.freeChunksOnAbort.Clear();
    }

    /// <summary>
    /// 丢弃自上次提交后的所有更改。
    /// Discard changes since the last commit and return to the state at the last commit point.
    /// </summary>
    public void Abort()
    {
      ArrayList toFree = new ArrayList();
      foreach (DictionaryEntry d in this.freeChunksOnAbort)
      {
        toFree.Add(d.Key);
      }
      toFree.Sort();
      toFree.Reverse();

      foreach (object item in toFree)
      {
        long headBlockNumber = (long)item;
        this.archive.ReleaseBlocks(headBlockNumber);
      }

      this.tree.Abort();
      this.archive.Flush();

      this.freeChunksOnCommit.Clear();
      this.freeChunksOnAbort.Clear();
    }

    /// <summary>
    /// 关闭并刷新流，而未进行Commit或Abort。
    /// Close and flush the streams without committing or aborting.
    /// (This is equivalent to abort, except unused space in the streams may be left unreachable).
    /// </summary>
    public void Shutdown()
    {
      this.tree.Shutdown();
      this.archive.Close();
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

      IDictionary<long, string> chunksInUse = new Dictionary<long, string>();

      string key = this.tree.FirstKey();
      while (key != null)
      {
        long blockNumber = this.tree[key];
        if (chunksInUse.ContainsKey(blockNumber))
        {
          throw new BPlusTreeException("block number " + blockNumber + " associated with more than one key '"
            + key + "' and '" + chunksInUse[blockNumber] + "'");
        }
        chunksInUse[blockNumber] = key;
        key = this.tree.NextKey(key);
      }

      // also consider the un-deallocated chunks to be in use
      foreach (DictionaryEntry thing in this.freeChunksOnCommit)
      {
        long buffernumber = (long)thing.Key;
        chunksInUse[buffernumber] = "awaiting commit";
      }

      this.archive.Recover(chunksInUse, correctErrors);
    }

    #endregion

    #region Index

    /// <summary>
    /// Gets or sets the byte array with the specified key.
    /// </summary>
    public byte[] this[string key]
    {
      get
      {
        long map = this.tree[key];
        return this.archive.GetChunk(map);
      }
      set
      {
        long headBlockNumber = this.archive.StoreChunk(value, 0, value.Length);

        this.freeChunksOnAbort[headBlockNumber] = headBlockNumber;

        if (this.tree.ContainsKey(key))
        {
          long valueFound = (long)this.tree.Get(key);
          if (this.freeChunksOnAbort.ContainsKey(valueFound))
          {
            // free it now
            this.freeChunksOnAbort.Remove(valueFound);
            this.archive.ReleaseBlocks(valueFound);
          }
          else
          {
            // release at commit.
            this.freeChunksOnCommit[valueFound] = valueFound;
          }
        }

        this.tree[key] = headBlockNumber;
      }
    }

    #endregion

    #region ToString

    /// <summary>
    /// 将树转成字符串描述
    /// </summary>
    /// <returns>树的字符串描述</returns>
    public string ToText()
    {
      StringBuilder sb = new StringBuilder();

      sb.Append(this.tree.ToText());
      sb.AppendLine();

      sb.AppendLine("BPlusTreeBytes -->");
      sb.Append("FreeChunksOnCommit = " + this.freeChunksOnCommit.Count + " : [");
      foreach (DictionaryEntry thing in this.freeChunksOnCommit)
      {
        sb.Append(thing.Key + ",");
      }
      sb.AppendLine("]");

      sb.Append("FreeChunksOnAbort = " + this.freeChunksOnAbort.Count + " : [");
      foreach (DictionaryEntry thing in this.freeChunksOnAbort)
      {
        sb.Append(thing.Key + ",");
      }
      sb.AppendLine("]");

      return sb.ToString(); // archive info not included
    }

    #endregion
  }
}
