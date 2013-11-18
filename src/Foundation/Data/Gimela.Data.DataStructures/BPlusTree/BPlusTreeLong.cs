using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// B+树，固定长度字符串为键，映射至长整形。
  /// </summary>
  public class BPlusTreeLong : ILongTree
  {
    #region Fields

    // 记录在提交或者丢弃时的内存块
    internal Hashtable FreeBlocksOnCommit = new Hashtable();
    internal Hashtable FreeBlocksOnAbort = new Hashtable();

    // 记录终端叶子节点，用于在需要释放内存时销毁掉
    private Hashtable _IdToTerminalNode = new Hashtable();
    private Hashtable _TerminalNodeToId = new Hashtable();
    private int _TerminalNodeCount = 0;
    private int _LowerTerminalNodeCount = 0;

    #endregion

    #region Properties

    /// <summary>
    /// 树序列化存储的头部长度(Byte数量)
    /// </summary>
    internal static int HeaderSize =
      StoredConstants.TreeFileHeaderPrefix.Length // 魔数前缀
      + 1                                         // 版本         1 Byte
      + StoredConstants.IntegerLength             // 节点容量     4 Bytes
      + StoredConstants.IntegerLength             // 键大小       4 Bytes
      + StoredConstants.LongLength                // 根节点块序号 8 Bytes
      + StoredConstants.LongLength;               // 空闲块头序号 8 Bytes

    /// <summary>
    /// 树序列化头部标示的版本号
    /// </summary>
    internal byte Version { get; private set; }
    /// <summary>
    /// 树序列化的流
    /// </summary>
    internal Stream Stream { get; private set; }
    /// <summary>
    /// 在树序列化流中查找的起始点
    /// </summary>
    internal long SeekStart { get; private set; }
    /// <summary>
    /// 树的块
    /// </summary>
    internal BlockFile BlockFile { get; private set; }
    /// <summary>
    /// 树块的大小
    /// </summary>
    internal int BlockSize
    {
      get
      {
        // indicator | first seek position | [ key storage | seek position ]*
        return 1                                                               // indicator     是否为叶子节点  1 Byte
          + StoredConstants.LongLength                                         // first seek position 块索引         8 Bytes
          + (this.KeyLength + StoredConstants.LongLength) * this.NodeCapacity; // (键长度 + 值长度) * 节点容量
      }
    }
    /// <summary>
    /// 树允许的键的长度
    /// </summary>
    internal int KeyLength { get; private set; }
    /// <summary>
    /// 树允许的各子节点的最大容量
    /// </summary>
    internal int NodeCapacity { get; private set; }
    /// <summary>
    /// 树根节点
    /// </summary>
    internal BPlusTreeNode RootNode { get; private set; }
    /// <summary>
    /// 根节点块序号
    /// </summary>
    internal long RootNodeBlockNumber { get; private set; }
    /// <summary>
    /// 空闲块头序号
    /// </summary>
    internal long FreeBlockHeadNumber { get; private set; }
    /// <summary>
    /// 缩减尺寸控制
    /// </summary>
    internal int FootprintLimit { get; private set; }

    #endregion

    #region Ctors

    /// <summary>
    /// B+树，固定长度字符串为键，映射至长整形。
    /// </summary>
    /// <param name="stream">指定的流</param>
    /// <param name="seekStart">流起始查询点</param>
    /// <param name="keyLength">树允许的键的长度</param>
    /// <param name="nodeCapacity">树允许的各子节点的最大容量</param>
    /// <param name="version">树的版本号</param>
    internal BPlusTreeLong(Stream stream, long seekStart, int keyLength, int nodeCapacity, byte version)
    {
      this.Stream = stream;
      this.SeekStart = seekStart;
      this.KeyLength = StoredConstants.ShortLength + keyLength; // Key的存储分两部分 = Key的长度(Short=2Bytes) + Key的内容(外部指定)
      this.NodeCapacity = nodeCapacity;
      this.Version = version;

      this.RootNode = null;
      this.RootNodeBlockNumber = StoredConstants.NullBlockNumber;
      this.FreeBlockHeadNumber = StoredConstants.NullBlockNumber;
      this.FootprintLimit = 100;

      if (this.SeekStart < 0)
      {
        throw new BPlusTreeException("start seek may not be negative");
      }
      if (this.NodeCapacity < StoredConstants.MinNodeCapacity)
      {
        throw new BPlusTreeException("node size must be larger than 2");
      }
      if (this.KeyLength < StoredConstants.MinKeyLength)
      {
        throw new BPlusTreeException("Key length must be larger than 5");
      }
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
      if (!(value is long))
      {
        throw new BPlusTreeBadKeyValueException("only long may be used as values in a tree: " + value);
      }
      this[key] = (long)value;
    }

    /// <summary>
    /// 检索键关联的对象。
    /// Get the object associated with the key.
    /// </summary>
    /// <param name="key">key to retrieve.</param>
    /// <returns>the mapped item boxed as an object</returns>
    public object Get(string key)
    {
      if (this.ContainsKey(key))
      {
        long valueFound = (long)0;
        this.RootNode.FindKey(key, out valueFound);
        return valueFound;
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
      return string.Compare(left, right, StringComparison.Ordinal);
    }

    /// <summary>
    /// 获取结构中最小的键。
    /// Get the least key in the structure.
    /// </summary>
    /// <returns>least key item or null if the tree is empty.</returns>
    public string FirstKey()
    {
      string firstKey = null;

      if (this.RootNode != null)
      {
        // empty string is smallest possible tree
        if (this.ContainsKey(""))
        {
          firstKey = "";
        }
        else
        {
          return this.RootNode.FindNextKey("");
        }

        this.ShrinkFootprint(); // shrink when FirstKey
      }

      return firstKey;
    }

    /// <summary>
    /// 获取比指定的键稍大的键。如果无此类键则返回空。
    /// Get the least key in the structure strictly "larger" than the argument.  Return null if there is no such key.
    /// </summary>
    /// <param name="afterThisKey">The "lower limit" for the item to return</param>
    /// <returns>Least key greater than argument or null</returns>
    public string NextKey(string afterThisKey)
    {
      if (afterThisKey == null)
      {
        throw new BPlusTreeBadKeyValueException("cannot search for null string");
      }

      string nextKey = this.RootNode.FindNextKey(afterThisKey);

      this.ShrinkFootprint(); // shrink when NextKey

      return nextKey;
    }

    /// <summary>
    /// 判断结构中是否存在此键。
    /// Return true if the key is present in the structure.
    /// </summary>
    /// <param name="key">Key to test</param>
    /// <returns>true if present, otherwise false.</returns>
    public bool ContainsKey(string key)
    {
      if (key == null)
      {
        throw new BPlusTreeBadKeyValueException("cannot search for null key");
      }

      bool isContainsKey = false;
      long valueFound = (long)0;
      if (this.RootNode != null)
      {
        isContainsKey = this.RootNode.FindKey(key, out valueFound);
      }

      this.ShrinkFootprint(); // shrink when ContainsKey

      return isContainsKey;
    }

    /// <summary>
    /// 弃用指定的键和关联的值。如果键未找到则抛出异常。
    /// Dispose of the key and its associated item.  Throw an exception if the key is missing.
    /// </summary>
    /// <param name="key">Key to erase.</param>
    public void RemoveKey(string key)
    {
      if (this.RootNode == null)
      {
        throw new BPlusTreeKeyNotFoundException("tree is empty: cannot delete");
      }

      bool mergeMe;
      BPlusTreeNode root = this.RootNode;

      // 从根节点开始遍历删除
      root.Delete(key, out mergeMe);

      // if the root is not a leaf and contains only one child (no key), reroot
      // 如果根节点不是一个叶子节点，并且仅还有一个子节点，则重新设置根节点
      if (mergeMe && !this.RootNode.IsLeaf && this.RootNode.Count == 0)
      {
        this.RootNode = this.RootNode.FirstChild();
        this.RootNodeBlockNumber = this.RootNode.MakeAsRoot();
        root.Free();
      }
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
      if (limit < 5)
      {
        throw new BPlusTreeException("foot print limit less than 5 is too small");
      }
      this.FootprintLimit = limit;
    }

    /// <summary>
    /// 提交更改，将上次提交后的所有更改持久化。
    /// Make changes since the last commit permanent.
    /// </summary>
    public void Commit()
    {
      // store all modifications
      if (this.RootNode != null)
      {
        this.RootNodeBlockNumber = this.RootNode.Invalidate(false);
      }

      // commit the new root
      this.Stream.Flush();
      this.WriteHeader();
      this.Stream.Flush();

      // at this point the changes are committed, but some space is unreachable.
      // now free all unfreed blocks no longer in use
      ArrayList toFree = new ArrayList();
      foreach (DictionaryEntry d in this.FreeBlocksOnCommit)
      {
        toFree.Add(d.Key);
      }
      if (toFree.Count > 0)
      {
        toFree.Sort();
        toFree.Reverse();
        foreach (object thing in toFree)
        {
          long blockNumber = (long)thing;
          this.ReclaimBlock(blockNumber);
        }
      }

      // store the free list head
      this.WriteHeader();
      this.Stream.Flush();

      this.ResetBookkeeping();
    }

    /// <summary>
    /// 丢弃自上次提交后的所有更改。
    /// Discard changes since the last commit and return to the state at the last commit point.
    /// </summary>
    public void Abort()
    {
      // deallocate allocated blocks
      ArrayList toFree = new ArrayList();
      foreach (DictionaryEntry d in this.FreeBlocksOnAbort)
      {
        toFree.Add(d.Key);
      }
      if (toFree.Count > 0)
      {
        toFree.Sort();
        toFree.Reverse();
        foreach (object thing in toFree)
        {
          long blockNumber = (long)thing;
          this.ReclaimBlock(blockNumber);
        }
      }

      long freeHead = this.FreeBlockHeadNumber;

      // reread the header (except for freelist head)
      this.ReadHeader();

      // restore the root
      if (this.RootNodeBlockNumber == StoredConstants.NullBlockNumber)
      {
        this.RootNode = null; // nothing was committed
      }
      else
      {
        this.RootNode.LoadFromBlock(this.RootNodeBlockNumber);
      }

      this.ResetBookkeeping();
      this.FreeBlockHeadNumber = freeHead;

      this.WriteHeader(); // store new freelist head
      this.Stream.Flush();
    }

    /// <summary>
    /// 关闭并刷新流，而未进行Commit或Abort。
    /// Close and flush the streams without committing or aborting.
    /// (This is equivalent to abort, except unused space in the streams may be left unreachable).
    /// </summary>
    public void Shutdown()
    {
      if (this.Stream != null)
      {
        this.Stream.Flush();
        this.Stream.Close();
      }
    }

    /// <summary>
    /// 检测并尝试回收再利用不可达的空间。当一块空间被修改后未进行Commit或Abort操作可能导致空间不可达。
    /// Examine the structure and optionally try to reclaim unreachable space.  A structure which was modified without a
    /// concluding commit or abort may contain unreachable space.
    /// </summary>
    /// <param name="correctErrors">如果为真则确认异常为可预知的，如果为假则当发生错误是抛出异常</param>
    public void Recover(bool correctErrors)
    {
      Hashtable visited = new Hashtable();

      if (this.RootNode != null)
      {
        // find all reachable nodes
        this.RootNode.SanityCheck(visited);
      }

      // traverse the free list
      long freeBlockNumber = this.FreeBlockHeadNumber;
      while (freeBlockNumber != StoredConstants.NullBlockNumber)
      {
        if (visited.ContainsKey(freeBlockNumber))
        {
          throw new BPlusTreeException("free block visited twice " + freeBlockNumber);
        }
        visited[freeBlockNumber] = (byte)BPlusTreeNodeIndicator.Free;
        freeBlockNumber = this.ParseFreeBlock(freeBlockNumber);
      }

      // find out what is missing
      Hashtable missing = new Hashtable();
      long maxBlockNumber = this.BlockFile.NextBlockNumber();
      for (long i = 0; i < maxBlockNumber; i++)
      {
        if (!visited.ContainsKey(i))
        {
          missing[i] = i;
        }
      }

      // remove from missing any free-on-commit blocks
      foreach (DictionaryEntry thing in this.FreeBlocksOnCommit)
      {
        long toBeFreed = (long)thing.Key;
        missing.Remove(toBeFreed);
      }

      // add the missing values to the free list
      if (correctErrors)
      {
        if (missing.Count > 0)
        {
          Debug.WriteLine("correcting " + missing.Count + " unreachable blocks");
        }

        ArrayList missingKeyList = new ArrayList();
        foreach (DictionaryEntry d in missing)
        {
          missingKeyList.Add(d.Key);
        }
        missingKeyList.Sort();
        missingKeyList.Reverse();

        foreach (object thing in missingKeyList)
        {
          long blockNumber = (long)thing;
          this.ReclaimBlock(blockNumber);
        }
      }
      else if (missing.Count > 0)
      {
        string blocks = "";
        foreach (DictionaryEntry thing in missing)
        {
          blocks += " " + thing.Key;
        }
        throw new BPlusTreeException("found " + missing.Count + " unreachable blocks." + blocks);
      }
    }

    #endregion

    #region Index

    /// <summary>
    /// Gets or sets the <see cref="System.Int64"/> with the specified key.
    /// </summary>
    public long this[string key]
    {
      get
      {
        return (long)this.Get(key);
      }
      set
      {
        // 验证给定的键是否符合要求
        if (!ValidateKey(key, this))
        {
          throw new BPlusTreeBadKeyValueException("null or too large key cannot be inserted into tree: " + key);
        }

        // 初始化根节点
        bool isInitRoot = false;
        if (this.RootNode == null)
        {
          // 分配根节点
          this.RootNode = BPlusTreeNode.MakeRoot(this, true);
          isInitRoot = true;
        }

        // 每个新值均由根节点插入
        string splitFirstKey; // 右节点的第一个键
        BPlusTreeNode splitNode; // 右节点
        this.RootNode.Insert(key, value, out splitFirstKey, out splitNode);

        // 根节点需要分割
        if (splitNode != null)
        {
          // 分割根节点，并构造一个新的根节点
          BPlusTreeNode oldRoot = this.RootNode;
          // 分割后的根节点已不是叶子节点
          this.RootNode = BPlusTreeNode.BinaryRoot(oldRoot, splitFirstKey, splitNode, this);
          isInitRoot = true;
        }

        // 是否需要将根节点写入新的块
        if (isInitRoot)
        {
          this.RootNodeBlockNumber = this.RootNode.DumpToNewBlock();
        }

        // 检测在内存中的大小
        this.ShrinkFootprint(); // shrink when Index
      }
    }

    #endregion

    #region File Header

    /// <summary>
    /// 写入B+树存储头
    /// </summary>
    private void WriteHeader()
    {
      byte[] header = this.MakeHeader();
      this.Stream.Seek(this.SeekStart, SeekOrigin.Begin);
      this.Stream.Write(header, 0, header.Length);
    }

    /// <summary>
    /// 读取B+树存储头
    /// </summary>
    private void ReadHeader()
    {
      // 魔数前缀 | 版本 | 节点容量 | 键大小 | 根节点块序号 | 空闲块头序号
      // prefix | version | node size | key size | block number of root | block number of free list head
      byte[] header = new byte[HeaderSize];

      this.Stream.Seek(this.SeekStart, SeekOrigin.Begin);
      this.Stream.Read(header, 0, HeaderSize);

      // 验证头前缀魔数
      int index = 0;
      foreach (byte b in StoredConstants.TreeFileHeaderPrefix)
      {
        if (header[index] != b)
        {
          throw new BlockFileException("invalid header prefix");
        }
        index++;
      }

      // 版本
      this.Version = header[index];
      index += 1;

      // 节点容量
      this.NodeCapacity = StoredHelper.RetrieveInt(header, index);
      index += StoredConstants.IntegerLength;

      // 键大小
      this.KeyLength = StoredHelper.RetrieveInt(header, index);
      index += StoredConstants.IntegerLength;

      // 根节点块序号
      this.RootNodeBlockNumber = StoredHelper.RetrieveLong(header, index);
      index += StoredConstants.LongLength;

      // 空闲块头序号
      this.FreeBlockHeadNumber = StoredHelper.RetrieveLong(header, index);
      index += StoredConstants.LongLength;

      if (this.NodeCapacity < 2)
      {
        throw new BPlusTreeException("node size must be larger than 2");
      }
      if (this.KeyLength < 5)
      {
        throw new BPlusTreeException("Key length must be larger than 5");
      }
    }

    /// <summary>
    /// 构造B+树存储头
    /// </summary>
    /// <returns></returns>
    private byte[] MakeHeader()
    {
      // 魔数前缀 | 版本 | 节点容量 | 键大小 | 根节点块序号 | 空闲块头序号
      // prefix | version | node size | key size | block number of root | block number of free list head
      byte[] header = new byte[HeaderSize];

      // 魔数前缀
      StoredConstants.TreeFileHeaderPrefix.CopyTo(header, 0);

      // 版本 1 Byte
      header[StoredConstants.TreeFileHeaderPrefix.Length] = Version;

      // 节点容量 4 Bytes
      int index = StoredConstants.TreeFileHeaderPrefix.Length + 1;
      StoredHelper.Store(this.NodeCapacity, header, index);
      index += StoredConstants.IntegerLength;

      // 键大小 4 Bytes
      StoredHelper.Store(this.KeyLength, header, index);
      index += StoredConstants.IntegerLength;

      // 根节点块序号 8 Bytes
      StoredHelper.Store(this.RootNodeBlockNumber, header, index);
      index += StoredConstants.LongLength;

      // 空闲块头序号 8 Bytes
      StoredHelper.Store(this.FreeBlockHeadNumber, header, index);
      index += StoredConstants.LongLength;

      return header;
    }

    #endregion

    #region File Stream

    /// <summary>
    /// 从指定的流初始化树
    /// </summary>
    /// <param name="fromFile">指定的流</param>
    /// <param name="seekStart">流起始查询点</param>
    /// <param name="keyLength">键长度</param>
    /// <param name="nodeCapacity">节点容量</param>
    /// <returns>树</returns>
    public static BPlusTreeLong InitializeInStream(Stream fromFile, long seekStart, int keyLength, int nodeCapacity)
    {
      if (fromFile == null)
        throw new ArgumentNullException("fromFile");

      if (fromFile.Length > seekStart)
      {
        throw new BPlusTreeException("can't initialize tree inside written area of stream");
      }

      BPlusTreeLong tree = new BPlusTreeLong(fromFile, seekStart, keyLength, nodeCapacity, (byte)1);
      tree.WriteHeader();
      tree.BlockFile = BlockFile.InitializeInStream(fromFile, seekStart + HeaderSize, StoredConstants.BlockFileHeaderPrefix, tree.BlockSize);

      return tree;
    }

    /// <summary>
    /// 从指定的流初始化树
    /// </summary>
    /// <param name="fromFile">指定的流</param>
    /// <param name="seekStart">流起始查询点</param>
    /// <returns>树</returns>
    public static BPlusTreeLong SetupFromExistingStream(Stream fromFile, long seekStart)
    {
      if (fromFile == null)
        throw new ArgumentNullException("fromFile");

      BPlusTreeLong tree = new BPlusTreeLong(fromFile, seekStart, 100, 7, (byte)1); // dummy values for nodesize, keysize
      tree.ReadHeader();
      tree.BlockFile = BlockFile.SetupFromExistingStream(fromFile, seekStart + HeaderSize, StoredConstants.BlockFileHeaderPrefix);

      if (tree.BlockFile.BlockSize != tree.BlockSize)
      {
        throw new BPlusTreeException("inner and outer block sizes should match");
      }

      if (tree.RootNodeBlockNumber != StoredConstants.NullBlockNumber)
      {
        tree.RootNode = BPlusTreeNode.MakeRoot(tree, true);
        tree.RootNode.LoadFromBlock(tree.RootNodeBlockNumber);
      }

      return tree;
    }

    #endregion

    #region Shrink Footprint

    /// <summary>
    /// 缩减内存尺寸，用于释放内存映射的缓冲区，减少内存中缓存的终端叶子节点的数量。
    /// </summary>
    public void ShrinkFootprint()
    {
      this.InvalidateTerminalNodes(this.FootprintLimit);
    }

    /// <summary>
    /// 使终端节点作废，减少内存中缓存的终端叶子节点的数量。
    /// </summary>
    /// <param name="limit">限制终端节点的数量</param>
    private void InvalidateTerminalNodes(int limit)
    {
      while (this._TerminalNodeToId.Count > limit)
      {
        // choose oldest nonterminal and deallocate it
        while (!this._IdToTerminalNode.ContainsKey(this._LowerTerminalNodeCount))
        {
          this._LowerTerminalNodeCount++; // since most nodes are terminal this should usually be a short walk
          if (this._LowerTerminalNodeCount > this._TerminalNodeCount)
          {
            throw new BPlusTreeException("internal error counting nodes, lower limit went too large");
          }
        }

        int id = this._LowerTerminalNodeCount;
        BPlusTreeNode victim = (BPlusTreeNode)this._IdToTerminalNode[id];

        this._IdToTerminalNode.Remove(id);
        this._TerminalNodeToId.Remove(victim);
        if (victim.BlockNumber != StoredConstants.NullBlockNumber)
        {
          victim.Invalidate(true);
        }
      }
    }

    #endregion

    #region Allocate Node Block

    /// <summary>
    /// 分配块，如果有空闲块则分配空闲块，否则分配新块。
    /// </summary>
    /// <returns>块序号</returns>
    public long AllocateBlock()
    {
      long allocated = -1;
      if (this.FreeBlockHeadNumber == StoredConstants.NullBlockNumber)
      {
        // should be written immediately after allocation
        allocated = this.BlockFile.NextBlockNumber();
        return allocated;
      }

      // 重新使用空闲的块
      allocated = this.FreeBlockHeadNumber;

      // 检索新的空闲块
      this.FreeBlockHeadNumber = this.ParseFreeBlock(allocated);

      return allocated;
    }

    /// <summary>
    /// 回收再利用指定序号的块
    /// </summary>
    /// <param name="blockNumber">指定序号</param>
    public void ReclaimBlock(long blockNumber)
    {
      int freeSize = 1 + StoredConstants.LongLength;
      byte[] block = new byte[freeSize];

      // it better not already be marked free
      this.BlockFile.ReadBlock(blockNumber, block, 0, 1);
      if (block[0] == (byte)BPlusTreeNodeIndicator.Free)
      {
        throw new BPlusTreeException("attempt to re-free free block not allowed");
      }
      block[0] = (byte)BPlusTreeNodeIndicator.Free;

      // 将指定序号的块置为空闲
      StoredHelper.Store(this.FreeBlockHeadNumber, block, 1);
      this.BlockFile.WriteBlock(blockNumber, block, 0, freeSize);
      this.FreeBlockHeadNumber = blockNumber;
    }

    #endregion

    #region Record Terminal Node

    /// <summary>
    /// 由树记录节点是否为终端节点，终端节点没有子节点，而根或中间节点有子节点
    /// </summary>
    /// <param name="terminalNode">这个节点是终端节点</param>
    public void RecordTerminalNode(BPlusTreeNode terminalNode)
    {
      if (terminalNode == this.RootNode)
      {
        return; // never record the root node
      }
      if (this._TerminalNodeToId.ContainsKey(terminalNode))
      {
        return; // don't record it again
      }

      int id = this._TerminalNodeCount;
      this._TerminalNodeCount++;

      this._TerminalNodeToId[terminalNode] = id;
      this._IdToTerminalNode[id] = terminalNode;
    }

    /// <summary>
    /// 由树记录节点是否为终端节点，终端节点没有子节点，而根或中间节点有子节点
    /// </summary>
    /// <param name="nonTerminalNode">这个节点已经不是终端节点</param>
    public void ForgetTerminalNode(BPlusTreeNode nonTerminalNode)
    {
      if (!this._TerminalNodeToId.ContainsKey(nonTerminalNode))
      {
        // silently ignore (?)
        return;
      }

      int id = (int)this._TerminalNodeToId[nonTerminalNode];
      if (id == this._LowerTerminalNodeCount)
      {
        this._LowerTerminalNodeCount++;
      }

      this._IdToTerminalNode.Remove(id);
      this._TerminalNodeToId.Remove(nonTerminalNode);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 检查空闲块
    /// </summary>
    public void CheckFreeBlock()
    {
      this.Recover(false);

      // look at all deferred deallocations -- they should not be free
      byte[] block = new byte[1];

      foreach (DictionaryEntry thing in this.FreeBlocksOnAbort)
      {
        long blockNumber = (long)thing.Key;
        this.BlockFile.ReadBlock(blockNumber, block, 0, 1);
        if (block[0] == (byte)BPlusTreeNodeIndicator.Free)
        {
          throw new BPlusTreeException("free on abort block already marked free " + blockNumber);
        }
      }

      foreach (DictionaryEntry thing in this.FreeBlocksOnCommit)
      {
        long blockNumber = (long)thing.Key;
        this.BlockFile.ReadBlock(blockNumber, block, 0, 1);
        if (block[0] == (byte)BPlusTreeNodeIndicator.Free)
        {
          throw new BPlusTreeException("free on commit block already marked free " + blockNumber);
        }
      }
    }

    /// <summary>
    /// 检测给定的键是否符合树要求键的长度
    /// </summary>
    /// <param name="key">给定的键</param>
    /// <param name="tree">给定的树</param>
    /// <returns>是否符合树要求键的长度</returns>
    public static bool ValidateKey(string key, BPlusTreeLong tree)
    {
      if (tree == null)
        throw new ArgumentNullException("tree");

      if (string.IsNullOrEmpty(key))
      {
        return false;
      }

      int maxKeyLength = tree.KeyLength;
      int maxKeyPayload = maxKeyLength - StoredConstants.ShortLength;

      char[] keyChars = key.ToCharArray();
      int charCount = Encoding.UTF8.GetEncoder().GetByteCount(keyChars, 0, keyChars.Length, true);
      if (charCount > maxKeyPayload)
      {
        return false;
      }

      return true;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 重置记录项
    /// </summary>
    private void ResetBookkeeping()
    {
      this.FreeBlocksOnCommit.Clear();
      this.FreeBlocksOnAbort.Clear();
      this._IdToTerminalNode.Clear();
      this._TerminalNodeToId.Clear();
    }

    /// <summary>
    /// 在指定的块序号之后，查找新的空闲块头序号
    /// </summary>
    /// <param name="blockNumber">指定的块序号</param>
    /// <returns>新的空闲块头序号</returns>
    private long ParseFreeBlock(long blockNumber)
    {
      int freeSize = 1 + StoredConstants.LongLength;
      byte[] block = new byte[freeSize];

      this.BlockFile.ReadBlock(blockNumber, block, 0, freeSize);
      if (block[0] != (byte)BPlusTreeNodeIndicator.Free)
      {
        throw new BPlusTreeException("free block not marked free");
      }

      long newHead = StoredHelper.RetrieveLong(block, 1);

      return newHead;
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

      sb.AppendLine("BPlusTreeLong Begin-->");

      sb.Append("NodeCapacity = " + this.NodeCapacity);
      sb.Append(", SeekStart = " + this.SeekStart);
      sb.Append(", HeaderSize = " + HeaderSize);
      sb.Append(", BlockSize = " + this.BlockSize);
      sb.Append(", KeyLength = " + this.KeyLength);
      sb.Append(", FootprintLimit = " + this.FootprintLimit);
      sb.Append(", RootNodeBlockNumber = " + this.RootNodeBlockNumber);
      sb.AppendLine(", FreeBlockHeadNumber = " + this.FreeBlockHeadNumber);

      // 打印空闲块
      sb.Append("FreeBlocks : ");
      long freeHead = this.FreeBlockHeadNumber;
      string allFreeString = "[";
      Hashtable freeVisit = new Hashtable();
      while (freeHead != StoredConstants.NullBlockNumber)
      {
        allFreeString = allFreeString + " " + freeHead + ",";
        if (freeVisit.ContainsKey(freeHead))
        {
          throw new BPlusTreeException("cycle in free block list " + freeHead);
        }
        freeVisit[freeHead] = freeHead;
        freeHead = this.ParseFreeBlock(freeHead);
      }
      sb.Append(allFreeString);
      sb.AppendLine("]");

      sb.Append("FreeBlocksOnCommit = " + this.FreeBlocksOnCommit.Count + " : [");
      foreach (DictionaryEntry thing in this.FreeBlocksOnCommit)
      {
        sb.Append(thing.Key + ",");
      }
      sb.AppendLine("]");

      sb.Append("FreeBlocksOnAbort = " + this.FreeBlocksOnAbort.Count + " : [");
      foreach (DictionaryEntry thing in this.FreeBlocksOnAbort)
      {
        sb.Append(thing.Key + ",");
      }
      sb.AppendLine("]");

      // 打印根节点
      sb.AppendLine("Root Begin-->");
      if (this.RootNode == null)
      {
        sb.Append("[NULL ROOT]");
      }
      else
      {
        sb.Append(this.RootNode.ToText(""));
      }
      sb.AppendLine("Root End-->");

      sb.Append("IdToTerminalNode : [");
      foreach (DictionaryEntry entry in _IdToTerminalNode)
      {
        sb.Append(entry.Key.ToString() + "-->" + ((BPlusTreeNode)entry.Value).BlockNumber + ", ");
      }
      sb.AppendLine("]");

      sb.AppendLine("BPlusTreeLong End-->");

      return sb.ToString();
    }

    #endregion
  }
}
