using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// B+树节点
  /// </summary>
  public class BPlusTreeNode
  {
    #region Properties
    
    /// <summary>
    /// 包含该节点的树
    /// </summary>
    internal BPlusTreeLong Tree { get; private set; }
    /// <summary>
    /// 节点的父节点
    /// </summary>
    internal BPlusTreeNode Parent { get; private set; }
    /// <summary>
    /// 在父节点中的索引
    /// </summary>
    internal int IndexInParent { get; private set; }
    /// <summary>
    /// 是否为叶子节点
    /// </summary>
    internal bool IsLeaf { get; private set; }
    /// <summary>
    /// 节点的块序号
    /// </summary>
    internal long BlockNumber { get; private set; }
    /// <summary>
    /// 节点拥有子节点的最大容量
    /// </summary>
    internal int Capacity { get; private set; }
    /// <summary>
    /// 如果为真则具化节点需要被持久化
    /// </summary>
    internal bool Dirty { get; private set; }
    /// <summary>
    /// 获取已被使用的容量
    /// </summary>
    internal int Count
    {
      get
      {
        int inUsing = 0;

        for (int i = 0; i < this.Capacity; i++)
        {
          if (this.ChildKeys[i] == null)
          {
            break;
          }
          inUsing++;
        }

        return inUsing;
      }
    }

    #endregion

    #region Fields
    
    /// <summary>
    /// 节点键数组
    /// </summary>
    private string[] ChildKeys;
    /// <summary>
    /// 节点值数组
    /// </summary>
    private long[] ChildValues;
    /// <summary>
    /// 具化子节点数组
    /// </summary>
    private BPlusTreeNode[] ChildNodes;

    #endregion

    #region Ctors
    
    /// <summary>
    /// B+树节点
    /// </summary>
    /// <param name="tree">包含该节点的树</param>
    /// <param name="parent">节点的父节点</param>
    /// <param name="indexInParent">在父节点中的索引</param>
    /// <param name="isLeaf">是否为叶子节点</param>
    public BPlusTreeNode(BPlusTreeLong tree, BPlusTreeNode parent, int indexInParent, bool isLeaf)
    {
      if(tree == null)
        throw new ArgumentNullException("tree");

      this.Tree = tree;
      this.Parent = parent;
      this.IsLeaf = isLeaf;

      this.IndexInParent = -1;
      this.BlockNumber = StoredConstants.NullBlockNumber;
      this.Capacity = tree.NodeCapacity;
      this.Dirty = true;

      this.Clear();

      // 存在父节点
      if (parent != null && indexInParent >= 0)
      {
        // B+树 父节点中键值数与子节点数量相等
        if (indexInParent > this.Capacity)
        {
          throw new BPlusTreeException("parent index too large");
        }
        // 建立与父节点关系
        this.Parent.ChildNodes[indexInParent] = this;
        this.BlockNumber = this.Parent.ChildValues[indexInParent];
        this.IndexInParent = indexInParent;
      }
    }

    #endregion

    #region Insert & Delete

    /// <summary>
    /// 在节点中插入键值对
    /// </summary>
    /// <param name="key">叶子节点的键</param>
    /// <param name="value">键对应的值</param>
    /// <param name="splitFirstKey">if not null then the smallest key in the new split leaf</param>
    /// <param name="splitNode">if not null then the node was split and this is the leaf to the right.</param>
    /// <returns>
    /// null unless the smallest key under this node has changed, in which case it returns the smallest key.
    /// </returns>
    public string Insert(string key, long value, out string splitFirstKey, out BPlusTreeNode splitNode)
    {
      if (this.IsLeaf)
      {
        return this.InsertLeaf(key, value, out splitFirstKey, out splitNode);
      }

      // 我不是叶子 我是中间节点 找到Key对应的位置 在子节点中插入键值对
      splitFirstKey = null;
      splitNode = null;

      // 查找新键的位置 由于是中间节点 则新键的位置必须存在
      int insertPosition = this.FindAtOrNextPosition(key, false);

      // 非叶子节点中的值数组存储叶子节点的块序号
      long insertValue = this.ChildValues[insertPosition];
      if (insertValue == StoredConstants.NullBlockNumber)
      {
        throw new BPlusTreeException("key not followed by block number in non-leaf");
      }

      // 加载子节点
      BPlusTreeNode insertChild = this.LoadNodeAtIndex(insertPosition);

      string childSplitFirstKey;
      BPlusTreeNode childSplitNode;
      // 在子节点中插入新的键值对
      string childInsert = insertChild.Insert(key, value, out childSplitFirstKey, out childSplitNode);

      // 发现子节点已满，也需要分割
      if (childSplitNode != null)
      {
        // 我即将被更改
        this.Soil(); // redundant -- a child will have a change so this node will need to be copied

        // 为新的子节点创建位置索引，即下一个
        int newChildPosition = insertPosition + 1;

        // 自己是否已满
        bool doSplit = false;

        // 如果我作为中间节点容量也满了，则中间节点也需要被分割
        if (this.ChildValues[this.Capacity] != StoredConstants.NullBlockNumber)
        {
          doSplit = true;
        }

        if (doSplit)
        {
          // 做分割准备
          this.PrepareBeforeSplit();
        }

        // bubble over the current values to make space for new child
        // 新节点位置上及其右侧内容全部向右移动1位，为新节点空出位置
        for (int i = this.ChildKeys.Length - 2; i >= newChildPosition - 1; i--)
        {
          int iPlus1 = i + 1;
          int iPlus2 = iPlus1 + 1;
          this.ChildKeys[iPlus1] = this.ChildKeys[i];
          this.ChildValues[iPlus2] = this.ChildValues[iPlus1];
          this.ChildNodes[iPlus2] = this.ChildNodes[iPlus1];
        }

        // record the new child
        // 新节点的位置存放新节点的第一个键
        this.ChildKeys[newChildPosition - 1] = childSplitFirstKey;

        // 被分割出的子节点的父节点为自己
        childSplitNode.ResetParent(this, newChildPosition);

        // 如果我作为中间节点容量也满了，则中间节点也需要被分割
        if (doSplit)
        {
          // 从中间开始分割 折半
          int splitPoint = this.ChildNodes.Length / 2 - 1;

          // 分割出的新节点的第一个Key
          splitFirstKey = this.ChildKeys[splitPoint];

          // 新建节点 包含分割点右侧所有数据
          splitNode = new BPlusTreeNode(this.Tree, this.Parent, -1, this.IsLeaf);
          splitNode.Clear(); // redundant.

          // 记录已经扩充的数据结构         
          long[] values = this.ChildValues;
          string[] keys = this.ChildKeys;
          BPlusTreeNode[] nodes = this.ChildNodes;

          // 重置和清空数据
          this.Clear();

          // 将分割点左侧的数据拷贝至此节点
          Array.Copy(keys, 0, this.ChildKeys, 0, splitPoint);
          Array.Copy(values, 0, this.ChildValues, 0, splitPoint + 1);
          Array.Copy(nodes, 0, this.ChildNodes, 0, splitPoint + 1);

          // 将分割点右侧的数据拷贝至新的分割节点
          int remainingKeys = this.Capacity - splitPoint;          
          Array.Copy(keys, splitPoint + 1, splitNode.ChildKeys, 0, remainingKeys);
          Array.Copy(values, splitPoint + 1, splitNode.ChildValues, 0, remainingKeys + 1);
          Array.Copy(nodes, splitPoint + 1, splitNode.ChildNodes, 0, remainingKeys + 1);

          // 重置新节点中所有的子节点的父节点
          splitNode.ResetAllChildrenParent();

          // 存储新节点
          splitNode.DumpToNewBlock();
          splitNode.CheckIfTerminal();
          splitNode.Soil();

          this.CheckIfTerminal();
        } // end do split

        // 重置节点中所有的子节点的父节点
        this.ResetAllChildrenParent();
      }

      // 返回最小的那个键
      if (insertPosition == 0)
      {
        return childInsert; // the smallest key may have changed
      }
      else
      {
        return null;  // no change in smallest key
      }
    }

    /// <summary>
    /// 在节点中插入键值对并作为叶子节点
    /// </summary>
    /// <param name="key">叶子节点的键</param>
    /// <param name="value">键对应的值</param>
    /// <param name="splitFirstKey">if not null then the smallest key in the new split leaf</param>
    /// <param name="splitNode">if not null then the node was split and this is the leaf to the right.</param>
    /// <returns>smallest key item in keys, or null if no change</returns>
    public string InsertLeaf(string key, long value, out string splitFirstKey, out BPlusTreeNode splitNode)
    {
      splitFirstKey = null;
      splitNode = null;

      if (!this.IsLeaf)
      {
        throw new BPlusTreeException("bad call to insert leaf, this is not a leaf");
      }

      // 标示节点已被更改
      this.Soil();

      // 查找新键的位置 键可能已经存在
      int insertPosition = this.FindAtOrNextPosition(key, false);

      bool doSplit = false;

      // 节点未满
      if (insertPosition < this.Capacity)
      {
        // 如果键已存在，则更改其对应值及位置，不支持重复的条目
        if (this.ChildKeys[insertPosition] == null || this.Tree.Compare(this.ChildKeys[insertPosition], key) == 0)
        {
          this.ChildKeys[insertPosition] = key;
          this.ChildValues[insertPosition] = value;

          // 返回键序列中的最小值，如果无更改则返回空
          if (insertPosition == 0)
          {
            return key;
          }
          else
          {
            return null;
          }
        }
        // 插入点为比指定键稍大的键
      }
      else
      {
        // 节点已满，准备分割节点
        doSplit = true;
      }

      // 查看是否还有空位置
      int nullIndex = insertPosition;
      while (nullIndex < this.ChildKeys.Length && this.ChildKeys[nullIndex] != null)
      {
        nullIndex++;
      }
      if (nullIndex >= this.ChildKeys.Length)
      {
        doSplit = true;
      }

      // 做分割的准备 数组增加1
      if (doSplit)
      {
        this.PrepareBeforeSplit();
      }

      // 将新数据插入至数组中，将已存在的值向右移动
      string nextKey = this.ChildKeys[insertPosition];
      long nextValue = this.ChildValues[insertPosition];
      this.ChildKeys[insertPosition] = key;
      this.ChildValues[insertPosition] = value;
      while (nextKey != null)
      {
        key = nextKey;
        value = nextValue;

        insertPosition++;

        nextKey = this.ChildKeys[insertPosition];
        nextValue = this.ChildValues[insertPosition];

        this.ChildKeys[insertPosition] = key;
        this.ChildValues[insertPosition] = value;
      }

      // 如果需要分割
      if (doSplit)
      {
        // 从中间开始分割 折半
        int splitPoint = this.ChildKeys.Length / 2;
        int splitLength = this.ChildKeys.Length - splitPoint;

        // 新创建的分割出的节点，始终是右节点
        splitNode = new BPlusTreeNode(this.Tree, this.Parent, -1, this.IsLeaf);

        // 将指定分割点左侧的数据拷贝至新的节点
        Array.Copy(this.ChildKeys, splitPoint, splitNode.ChildKeys, 0, splitLength);
        Array.Copy(this.ChildValues, splitPoint, splitNode.ChildValues, 0, splitLength);
        Array.Copy(this.ChildNodes, splitPoint, splitNode.ChildNodes, 0, splitLength);

        // 记录分割节点的第一个键，右节点的第一个键
        splitFirstKey = splitNode.ChildKeys[0];

        // 存储新节点至块文件
        splitNode.DumpToNewBlock();

        // 分割完毕 恢复之前的准备 处理分割点右侧数据，保留左侧数据，删除右侧数据
        this.RepairAfterSplit(splitPoint);

        // 记录新的节点
        this.Tree.RecordTerminalNode(splitNode); // InsertLeaf

        // 新节点及其父节点需要处理
        splitNode.Soil();
      }

      if (insertPosition == 0)
      {
        return key; // smallest key changed.
      }
      else
      {
        return null; // no change in smallest key
      }
    }

    /// <summary>
    /// 删除指定键的记录
    /// </summary>
    /// <param name="key">指定键</param>
    /// <param name="mergeMe">true if the node is less than half full after deletion</param>
    /// <returns>null unless the smallest key under this node has changed in which case it returns the smallest key.</returns>
    public string Delete(string key, out bool mergeMe)
    {
      if (this.IsLeaf)
      {
        return this.DeleteLeaf(key, out mergeMe);
      }

      // 假设我是不需要被合并的
      mergeMe = false; // assumption

      // 我是中间节点，找到指定Key的位置
      int deletePosition = this.FindAtOrNextPosition(key, false);
      long deleteBlockNumber = this.ChildValues[deletePosition];
      if (deleteBlockNumber == StoredConstants.NullBlockNumber)
      {
        throw new BPlusTreeException("key not followed by block number in non-leaf (del)");
      }

      // 加载要被删除位置的子节点
      BPlusTreeNode deleteChildNode = this.LoadNodeAtIndex(deletePosition);

      // 从子节点中删除Key
      bool isChildNodeNeedMerge;
      string deletedChildKey = deleteChildNode.Delete(key, out isChildNodeNeedMerge);

      // delete succeeded... now fix up the child node if needed.
      this.Soil(); // redundant ?

      string result = null;
      // bizarre special case for 2-3  or 3-4 trees -- empty leaf
      // 发现子节点的最小键与被删除的键相同，则认为子节点已经没用了
      if (deletedChildKey != null && this.Tree.Compare(deletedChildKey, key) == 0)
      {
        if (this.Capacity > 3)
        {
          throw new BPlusTreeException("assertion error: delete returned delete key for too large node size: " + this.Capacity);
        }

        // junk this leaf and shift everything over
        // 节点位置右侧全部左移一位
        if (deletePosition == 0)
        {
          result = this.ChildKeys[deletePosition];
        }
        else if (deletePosition == this.Capacity)
        {
          this.ChildKeys[deletePosition - 1] = null;
        }
        else
        {
          this.ChildKeys[deletePosition - 1] = this.ChildKeys[deletePosition];
        }

        if (result != null && this.Tree.Compare(result, key) == 0)
        {
          // I'm not sure this ever happens
          this.LoadNodeAtIndex(1);
          result = this.ChildNodes[1].LeastKey();
        }

        // 这个子节点彻底没用了
        deleteChildNode.Free();

        for (int i = deletePosition; i < this.Capacity - 1; i++)
        {
          this.ChildKeys[i] = this.ChildKeys[i + 1];
          this.ChildValues[i] = this.ChildValues[i + 1];
          this.ChildNodes[i] = this.ChildNodes[i + 1];          
        }
        this.ChildKeys[this.Capacity - 1] = null;

        if (deletePosition < this.Capacity)
        {
          this.ChildValues[this.Capacity - 1] = this.ChildValues[this.Capacity];
          this.ChildNodes[this.Capacity - 1] = this.ChildNodes[this.Capacity];          
        }
        this.ChildNodes[this.Capacity] = null;
        this.ChildValues[this.Capacity] = StoredConstants.NullBlockNumber;

        // 如果节点的使用率小于一半，则需要合并节点
        if (this.Count < this.Capacity / 2)
        {
          mergeMe = true;
        }

        // 重置子节点的父节点
        this.ResetAllChildrenParent();

        return result;
      } // end if

      if (deletePosition == 0)
      {
        // smallest key may have changed.
        result = deletedChildKey;
      }
      else if (deletedChildKey != null && deletePosition > 0)
      {
        // update key array if needed
        if (this.Tree.Compare(deletedChildKey, key) != 0)
        {
          this.ChildKeys[deletePosition - 1] = deletedChildKey;
        }
      }

      // if the child needs merging... do it
      // 如果子节点需要合并
      if (isChildNodeNeedMerge)
      {
        int leftIndex, rightIndex;
        BPlusTreeNode leftNode, rightNode;
        string keyBetween;

        if (deletePosition == 0)
        {
          // merge with next
          leftIndex = deletePosition;
          rightIndex = deletePosition + 1;
          leftNode = deleteChildNode;
          rightNode = this.LoadNodeAtIndex(rightIndex);
        }
        else
        {
          // merge with previous
          leftIndex = deletePosition - 1;
          rightIndex = deletePosition;
          leftNode = this.LoadNodeAtIndex(leftIndex);
          rightNode = deleteChildNode;
        }

        keyBetween = this.ChildKeys[leftIndex];

        // 合并节点
        string rightLeastKey;
        bool isDeleteRight;
        Merge(leftNode, keyBetween, rightNode, out rightLeastKey, out isDeleteRight);

        // 是否需要删除右节点
        if (isDeleteRight)
        {
          for (int i = rightIndex; i < this.Capacity; i++)
          {
            this.ChildKeys[i - 1] = this.ChildKeys[i];
            this.ChildValues[i] = this.ChildValues[i + 1];
            this.ChildNodes[i] = this.ChildNodes[i + 1];
          }
          this.ChildKeys[this.Capacity - 1] = null;
          this.ChildValues[this.Capacity] = StoredConstants.NullBlockNumber;
          this.ChildNodes[this.Capacity] = null;

          this.ResetAllChildrenParent();

          rightNode.Free();

          // does this node need merging?
          if (this.Count < this.Capacity / 2)
          {
            mergeMe = true;
          }
        }
        else
        {
          // update the key entry
          this.ChildKeys[rightIndex - 1] = rightLeastKey;
        }
      }

      return result;
    }

    /// <summary>
    /// 删除指定键叶子节点的记录
    /// </summary>
    /// <param name="key">指定键</param>
    /// <param name="mergeMe"></param>
    /// <returns></returns>
    public string DeleteLeaf(string key, out bool mergeMe)
    {
      // 假设我是不需要被合并的
      mergeMe = false; // assumption

      // 先找到指定的Key和Key的位置
      bool isKeyFound = false;
      int deletePosition = 0;
      foreach (string childKey in this.ChildKeys)
      {
        if (childKey != null && this.Tree.Compare(childKey, key) == 0)
        {
          isKeyFound = true;
          break;
        }
        deletePosition++;
      }
      if (!isKeyFound)
      {
        throw new BPlusTreeKeyNotFoundException("cannot delete missing key: " + key);
      }

      // 这个节点即将被修改
      this.Soil();

      // 将指定Key位置右侧的数据左移1位，覆盖掉指定Key
      for (int i = deletePosition; i < this.Capacity - 1; i++)
      {
        this.ChildKeys[i] = this.ChildKeys[i + 1];
        this.ChildValues[i] = this.ChildValues[i + 1];
      }
      this.ChildKeys[this.Capacity - 1] = null;

      // 如果节点的使用率小于一半，则需要合并节点
      if (this.Count < this.Capacity / 2)
      {
        mergeMe = true;
      }

      // 返回被删后的最小值
      string result = null;
      if (deletePosition == 0)
      {
        result = this.ChildKeys[0];

        // this is only relevant for the case of 2-3 trees (empty leaf after deletion)
        if (result == null)
        {
          result = key; // deleted item
        }
      }

      return result;
    }

    /// <summary>
    /// 清理节点
    /// </summary>
    private void Clear()
    {
      this.ChildKeys = new string[this.Capacity];
      this.ChildValues = new long[this.Capacity + 1];
      this.ChildNodes = new BPlusTreeNode[this.Capacity + 1];

      for (int i = 0; i < this.Capacity; i++)
      {
        this.ChildKeys[i] = null;
        this.ChildValues[i] = StoredConstants.NullBlockNumber;
        this.ChildNodes[i] = null;
      }

      this.ChildValues[this.Capacity] = StoredConstants.NullBlockNumber;
      this.ChildNodes[this.Capacity] = null;

      // this is now a terminal node
      CheckIfTerminal();
    }

    /// <summary>
    /// 检查节点是否为终端节点，由树记录节点是否为终端节点，终端节点没有子节点，而根或中间节点有子节点
    /// </summary>
    private void CheckIfTerminal()
    {
      // 如果我不是叶子节点
      if (!this.IsLeaf)
      {
        for (int i = 0; i < this.Capacity + 1; i++)
        {
          // 如果我有一个已经被具体化的子节点
          if (this.ChildNodes[i] != null)
          {
            // 则标示我已经不是终端节点了
            this.Tree.ForgetTerminalNode(this); // 我已经有了一个具体化的子节点，我不再是终端节点
            return;
          }
        }
      }

      // 我没有一个已经被具体化的子节点，所以我还是终端节点，终端节点是没有一个已经被具体化的子节点
      this.Tree.RecordTerminalNode(this); // CheckIfTerminal
    }

    /// <summary>
    /// 重置所有子节点的父节点
    /// </summary>
    private void ResetAllChildrenParent()
    {
      for (int i = 0; i <= this.Capacity; i++)
      {
        BPlusTreeNode node = this.ChildNodes[i];
        if (node != null)
        {
          node.ResetParent(this, i);
        }
      }
    }

    /// <summary>
    /// 标示节点及其所有父节点均为脏数据节点
    /// </summary>
    private void Soil()
    {
      if (!this.Dirty)
      {
        this.Dirty = true;
        if (this.Parent != null)
        {
          this.Parent.Soil();
        }
      }
    }

    /// <summary>
    /// 获取最小的键
    /// </summary>
    /// <returns>最小的键</returns>
    private string LeastKey()
    {
      string key = null;
      if (this.IsLeaf)
      {
        key = this.ChildKeys[0];
      }
      else
      {
        this.LoadNodeAtIndex(0);
        key = this.ChildNodes[0].LeastKey();
      }

      if (key == null)
      {
        throw new BPlusTreeException("no key found");
      }

      return key;
    }

    #endregion

    #region Dump & Load

    /// <summary>
    /// 将数据导入新的块
    /// </summary>
    /// <returns></returns>
    internal long DumpToNewBlock()
    {
      // 由树非配节点存储的块序号
      long oldBlockNumber = this.BlockNumber;
      long newBlockNumber = this.Tree.AllocateBlock();

      this.DumpToBlock(newBlockNumber);

      if (oldBlockNumber != StoredConstants.NullBlockNumber)
      {
        if (this.Tree.FreeBlocksOnAbort.ContainsKey(oldBlockNumber))
        {
          // free it now
          this.Tree.FreeBlocksOnAbort.Remove(oldBlockNumber);
          this.Tree.ReclaimBlock(oldBlockNumber);
        }
        else
        {
          // free on commit
          this.Tree.FreeBlocksOnCommit[oldBlockNumber] = oldBlockNumber;
        }
      }

      this.Tree.FreeBlocksOnAbort[newBlockNumber] = newBlockNumber;

      return newBlockNumber;
    }

    /// <summary>
    /// 将数据导入指定的块
    /// </summary>
    /// <param name="blockNumber">块序号</param>
    private void DumpToBlock(long blockNumber)
    {
      byte[] raw = new byte[this.Tree.BlockSize];
      this.Dump(raw);

      this.Tree.BlockFile.WriteBlock(blockNumber, raw, 0, raw.Length);
      this.Dirty = false;
      this.BlockNumber = blockNumber;

      if (this.Parent != null
        && this.IndexInParent >= 0
        && this.Parent.ChildValues[this.IndexInParent] != blockNumber)
      {
        if (this.Parent.ChildNodes[this.IndexInParent] != this)
        {
          throw new BPlusTreeException("invalid parent connection " + this.Parent.BlockNumber + " at " + this.IndexInParent);
        }
        this.Parent.ChildValues[this.IndexInParent] = blockNumber;
        this.Parent.Soil();
      }
    }

    /// <summary>
    /// 将数据导入指定的块
    /// </summary>
    /// <param name="block">指定的块</param>
    private void Dump(byte[] block)
    {
      // indicator | first seek position | [ key storage | item storage ] * node capacity
      if (block.Length != Tree.BlockSize)
      {
        throw new BPlusTreeException("bad block size " + block.Length + " should be " + Tree.BlockSize);
      }

      int index = 0;

      // 设置是否为叶子节点
      block[0] = (byte)BPlusTreeNodeIndicator.Internal;
      if (this.IsLeaf)
      {
        block[0] = (byte)BPlusTreeNodeIndicator.Leaf;
      }
      index++;

      // store first seek position
      // 存储节点中值序列的第一个
      StoredHelper.Store(this.ChildValues[0], block, index);
      index += StoredConstants.LongLength;

      // store remaining keys and values
      Encoder encode = Encoding.UTF8.GetEncoder();
      int maxKeyLength = this.Tree.KeyLength;
      int maxKeyPayload = maxKeyLength - StoredConstants.ShortLength; // Key的内容长度(外部指定)
      string lastkey = "";
      for (int keyIndex = 0; keyIndex < this.Capacity; keyIndex++)
      {
        // 存储Key
        string key = this.ChildKeys[keyIndex];
        short charCount = -1;
        if (key != null)
        {
          char[] keyChars = key.ToCharArray();
          charCount = (short)encode.GetByteCount(keyChars, 0, keyChars.Length, true);
          if (charCount > maxKeyPayload)
          {
            throw new BPlusTreeException("string bytes to large for use as key " + charCount + ">" + maxKeyPayload);
          }

          // 存储Key的长度(Short=2Bytes)
          StoredHelper.Store(charCount, block, index);
          index += StoredConstants.ShortLength;

          // 存储Key的内容 拷贝
          encode.GetBytes(keyChars, 0, keyChars.Length, block, index, true);
          // Key的存储分两部分 = Key的长度(Short=2Bytes) + Key的内容(外部指定)
          index += maxKeyPayload;
        }
        else
        {
          // null case (no string to read) 存储默认-1
          StoredHelper.Store(charCount, block, index);
          index += StoredConstants.ShortLength;
          index += maxKeyPayload;
        }

        // 存储Value
        long seekPosition = this.ChildValues[keyIndex + 1]; // 第一个已经被存储了
        if (key == null && seekPosition != StoredConstants.NullBlockNumber && !this.IsLeaf)
        {
          throw new BPlusTreeException("null key paired with non-null location " + keyIndex);
        }
        if (lastkey == null && key != null)
        {
          throw new BPlusTreeException("null key followed by non-null key " + keyIndex);
        }
        lastkey = key;

        StoredHelper.Store(seekPosition, block, index); // 存储Value
        index += StoredConstants.LongLength;
      }
    }

    /// <summary>
    /// 从指定的块序号位置读取数据
    /// </summary>
    /// <param name="blockNumber">指定的块序号位置</param>
    internal void LoadFromBlock(long blockNumber)
    {
      // freelist bookkeeping done elsewhere
      byte[] raw = new byte[this.Tree.BlockSize];
      this.Tree.BlockFile.ReadBlock(blockNumber, raw, 0, raw.Length);
      this.Load(raw);
      this.Dirty = false;
      this.BlockNumber = blockNumber;

      // it's terminal until a child is materialized
      // 我是终端直到一个子节点被具体化
      this.Tree.RecordTerminalNode(this); // LoadFromBlock
    }

    /// <summary>
    /// 从给定的块中加载数据
    /// </summary>
    /// <param name="block">给定的块</param>
    private void Load(byte[] block)
    {
      // indicator | first seek position | [ key storage | item storage ] * node capacity
      this.Clear();

      if (block.Length != Tree.BlockSize)
      {
        throw new BPlusTreeException("bad block size " + block.Length + " should be " + Tree.BlockSize);
      }

      // indicator
      byte indicator = block[0];
      this.IsLeaf = false;
      if (indicator == (byte)BPlusTreeNodeIndicator.Leaf)
      {
        this.IsLeaf = true;
      }
      else if (indicator != (byte)BPlusTreeNodeIndicator.Internal)
      {
        throw new BPlusTreeException("bad indicator, not leaf or non-leaf in tree " + indicator);
      }

      int index = 1;

      // get the first seek position
      // 获取存储的第一个值
      this.ChildValues[0] = StoredHelper.RetrieveLong(block, index);
      index += StoredConstants.LongLength;

      Decoder decode = Encoding.UTF8.GetDecoder();      
      int maxKeyLength = this.Tree.KeyLength;
      int maxKeyPayload = maxKeyLength - StoredConstants.ShortLength;

      // get remaining key storages and seek positions
      string lastKey = "";
      for (int keyIndex = 0; keyIndex < this.Capacity; keyIndex++)
      {
        // 获取Key的长度
        short keylength = StoredHelper.RetrieveShort(block, index);
        if (keylength < -1 || keylength > maxKeyPayload)
        {
          throw new BPlusTreeException("invalid key length decoded");
        }
        index += StoredConstants.ShortLength;

        // 获取Key
        string key = null;
        if (keylength == 0)
        {
          key = "";
        }
        else if (keylength > 0)
        {
          int charCount = decode.GetCharCount(block, index, keylength);
          char[] ca = new char[charCount];
          decode.GetChars(block, index, keylength, ca, 0);
          key = new string(ca);
        }

        // 把Key放入列表
        this.ChildKeys[keyIndex] = key;
        index += maxKeyPayload;

        // decode and store a seek position
        // 获取存储的Value
        long seekValue = StoredHelper.RetrieveLong(block, index);

        // 检测Key顺序问题
        if (!this.IsLeaf)
        {
          if (key == null && seekValue != StoredConstants.NullBlockNumber)
          {
            throw new BPlusTreeException("key is null but position is not " + keyIndex);
          }
          else if (lastKey == null && key != null)
          {
            throw new BPlusTreeException("null key followed by non-null key " + keyIndex);
          }
        }
        lastKey = key;

        // 把Value放入列表
        this.ChildValues[keyIndex + 1] = seekValue; // 第一个已经被获取了
        index += StoredConstants.LongLength;
      }
    }

    /// <summary>
    /// 加载指定插入的索引点的节点对象
    /// </summary>
    /// <param name="insertPosition">指定插入的索引点</param>
    /// <returns>节点对象</returns>
    private BPlusTreeNode LoadNodeAtIndex(int insertPosition)
    {
      if (this.IsLeaf) // 只对中间节点应用有效
      {
        throw new BPlusTreeException("cannot materialize child for leaf");
      }

      // 获取指定位置对应子节点的块序号
      long childBlockNumber = this.ChildValues[insertPosition];
      if (childBlockNumber == StoredConstants.NullBlockNumber)
      {
        throw new BPlusTreeException("can't search null subtree at position " + insertPosition + " in " + this.BlockNumber);
      }

      // 节点对象已经存在吗
      BPlusTreeNode node = this.ChildNodes[insertPosition];
      if (node != null)
      {
        return node;
      }

      // 如果不存在则从块中加载
      node = new BPlusTreeNode(this.Tree, this, insertPosition, true); // dummy isLeaf item
      node.LoadFromBlock(childBlockNumber);
      this.ChildNodes[insertPosition] = node;

      // 该节点不在是终端节点
      this.Tree.ForgetTerminalNode(this); // 我已经有了一个具体化的子节点，我不再是终端节点

      return node;
    }

    #endregion

    #region Position & Split

    /// <summary>
    /// 在节点中查找指定键值的索引，如果无此键则查找第一个较大的键值索引，或者返回节点容量
    /// </summary>
    /// <param name="compareKey">需要比较的键值</param>
    /// <param name="lookPastOnly">如果节点为叶子节点，并且此参数为真，则查找一个较大的键值</param>
    /// <returns>在节点中查找指定键值的索引，如果无此键则查找第一个较大的键值索引，或者返回节点容量</returns>
    private int FindAtOrNextPosition(string compareKey, bool lookPastOnly)
    {
      int insertPosition = 0;

      if (this.IsLeaf && !lookPastOnly)
      {
        // look for exact match or greater or null
        while (insertPosition < this.Capacity
          && this.ChildKeys[insertPosition] != null
          && this.Tree.Compare(this.ChildKeys[insertPosition], compareKey) < 0)
        {
          insertPosition++;
        }
      }
      else
      {
        // look for greater or null only
        while (insertPosition < this.Capacity
          && this.ChildKeys[insertPosition] != null
          && this.Tree.Compare(this.ChildKeys[insertPosition], compareKey) <= 0)
        {
          insertPosition++;
        }
      }

      return insertPosition;
    }

    /// <summary>
    /// Find near-index of comparekey in leaf under this node. 
    /// </summary>
    /// <param name="compareKey">the key to look for</param>
    /// <param name="inLeaf">the leaf where found</param>
    /// <param name="lookPastOnly">If true then only look for a greater item, not an exact match.</param>
    /// <returns>index of match in leaf</returns>
    private int FindAtOrNextPositionInLeaf(string compareKey, out BPlusTreeNode inLeaf, bool lookPastOnly)
    {
      int keyPosition = this.FindAtOrNextPosition(compareKey, lookPastOnly);

      // 如果自己即是叶子节点
      if (this.IsLeaf)
      {
        inLeaf = this;
        return keyPosition;
      }

      // 尝试在子节点中查找
      BPlusTreeNode child = this.LoadNodeAtIndex(keyPosition);
      return child.FindAtOrNextPositionInLeaf(compareKey, out inLeaf, lookPastOnly);
    }

    /// <summary>
    /// 将当前节点的容量扩大(+1)，为插入和分割做准备
    /// </summary>
    private void PrepareBeforeSplit()
    {
      int superSize = this.Capacity + 1;

      string[] keys = new string[superSize];
      long[] positions = new long[superSize + 1];
      BPlusTreeNode[] materialized = new BPlusTreeNode[superSize + 1];

      Array.Copy(this.ChildKeys, 0, keys, 0, this.Capacity);
      keys[this.Capacity] = null;
      Array.Copy(this.ChildValues, 0, positions, 0, this.Capacity + 1);
      positions[this.Capacity + 1] = StoredConstants.NullBlockNumber;
      Array.Copy(this.ChildNodes, 0, materialized, 0, this.Capacity + 1);
      materialized[this.Capacity + 1] = null;

      this.ChildValues = positions;
      this.ChildKeys = keys;
      this.ChildNodes = materialized;
    }

    /// <summary>
    /// 节点分割后，恢复节点容量(-1)
    /// </summary>
    /// <param name="splitPoint">分割点</param>
    private void RepairAfterSplit(int splitPoint)
    {
      // store the node data temporarily
      string[] keys = this.ChildKeys;
      long[] values = this.ChildValues;
      BPlusTreeNode[] nodes = this.ChildNodes;

      // repair current node, copy in the other part of the split
      this.ChildKeys = new string[this.Capacity];
      this.ChildValues = new long[this.Capacity + 1];
      this.ChildNodes = new BPlusTreeNode[this.Capacity + 1];

      // 保留左侧数据
      Array.Copy(keys, 0, this.ChildKeys, 0, splitPoint);
      Array.Copy(values, 0, this.ChildValues, 0, splitPoint);
      Array.Copy(nodes, 0, this.ChildNodes, 0, splitPoint);

      // 删除右侧数据
      for (int i = splitPoint; i < this.ChildKeys.Length; i++)
      {
        this.ChildKeys[i] = null;
        this.ChildValues[i] = StoredConstants.NullBlockNumber;
        this.ChildNodes[i] = null;
      }
    }

    #endregion

    #region Merge Node

    /// <summary>
    /// 合并节点，当节点的使用率不足50%时，则需要合并
    /// </summary>
    /// <param name="left">左节点</param>
    /// <param name="keyBetween">左右节点的中间键</param>
    /// <param name="right">右节点</param>
    /// <param name="rightLeastKey">合并后的键的最小值</param>
    /// <param name="canDeleteRightNode">是否可以删除右节点</param>
    public static void Merge(BPlusTreeNode left, string keyBetween, BPlusTreeNode right, out string rightLeastKey, out bool canDeleteRightNode)
    {
      if (left == null)
        throw new ArgumentNullException("left");
      if (right == null)
        throw new ArgumentNullException("right");

      rightLeastKey = null; // only if DeleteRight

      // 合并叶子节点
      if (left.IsLeaf || right.IsLeaf)
      {
        if (!(left.IsLeaf && right.IsLeaf))
        {
          throw new BPlusTreeException("can't merge leaf with non-leaf");
        }

        // 合并子节点
        MergeLeaves(left, right, out canDeleteRightNode);

        rightLeastKey = right.ChildKeys[0];

        return;
      }

      // 合并非叶子节点
      canDeleteRightNode = false;

      if (left.ChildValues[0] == StoredConstants.NullBlockNumber || right.ChildValues[0] == StoredConstants.NullBlockNumber)
      {
        throw new BPlusTreeException("cannot merge empty non-leaf with non-leaf");
      }

      string[] allKeys = new string[left.Capacity * 2 + 1];
      long[] allValues = new long[left.Capacity * 2 + 2];
      BPlusTreeNode[] allNodes = new BPlusTreeNode[left.Capacity * 2 + 2];

      // 拷贝左节点的数据
      int index = 0;
      allValues[0] = left.ChildValues[0];
      allNodes[0] = left.ChildNodes[0];
      for (int i = 0; i < left.Capacity; i++)
      {
        if (left.ChildKeys[i] == null)
        {
          break;
        }

        allKeys[index] = left.ChildKeys[i];
        allValues[index + 1] = left.ChildValues[i + 1];
        allNodes[index + 1] = left.ChildNodes[i + 1];

        index++;
      }

      // 拷贝中间键
      allKeys[index] = keyBetween;
      index++;

      // 拷贝右节点的数据
      allValues[index] = right.ChildValues[0];
      allNodes[index] = right.ChildNodes[0];
      int rightCount = 0;
      for (int i = 0; i < right.Capacity; i++)
      {
        if (right.ChildKeys[i] == null)
        {
          break;
        }

        allKeys[index] = right.ChildKeys[i];
        allValues[index + 1] = right.ChildValues[i + 1];
        allNodes[index + 1] = right.ChildNodes[i + 1];
        index++;

        rightCount++;
      }

      // 如果数量小于左节点的能力，则右节点可以删除掉
      if (index <= left.Capacity)
      {
        // it will all fit in one node
        canDeleteRightNode = true;

        for (int i = 0; i < index; i++)
        {
          left.ChildKeys[i] = allKeys[i];
          left.ChildValues[i] = allValues[i];
          left.ChildNodes[i] = allNodes[i];
        }

        left.ChildValues[index] = allValues[index];
        left.ChildNodes[index] = allNodes[index];

        left.ResetAllChildrenParent();
        left.Soil();

        right.Free();

        return;
      }

      // otherwise split the content between the nodes
      left.Clear();
      right.Clear();
      left.Soil();
      right.Soil();

      int leftContent = index / 2;
      int rightContent = index - leftContent - 1;

      rightLeastKey = allKeys[leftContent];

      int outputIndex = 0;
      for (int i = 0; i < leftContent; i++)
      {
        left.ChildKeys[i] = allKeys[outputIndex];
        left.ChildValues[i] = allValues[outputIndex];
        left.ChildNodes[i] = allNodes[outputIndex];
        outputIndex++;
      }

      rightLeastKey = allKeys[outputIndex];

      left.ChildValues[outputIndex] = allValues[outputIndex];
      left.ChildNodes[outputIndex] = allNodes[outputIndex];
      outputIndex++;

      rightCount = 0;
      for (int i = 0; i < rightContent; i++)
      {
        right.ChildKeys[i] = allKeys[outputIndex];
        right.ChildValues[i] = allValues[outputIndex];
        right.ChildNodes[i] = allNodes[outputIndex];
        outputIndex++;

        rightCount++;
      }

      right.ChildValues[rightCount] = allValues[outputIndex];
      right.ChildNodes[rightCount] = allNodes[outputIndex];

      left.ResetAllChildrenParent();
      right.ResetAllChildrenParent();
    }

    /// <summary>
    /// 合并叶子节点，当节点的使用率不足50%时，则需要合并
    /// </summary>
    /// <param name="left">左节点</param>
    /// <param name="right">右节点</param>
    /// <param name="canDeleteRightNode">是否可以删除右节点</param>
    public static void MergeLeaves(BPlusTreeNode left, BPlusTreeNode right, out bool canDeleteRightNode)
    {
      if (left == null)
        throw new ArgumentNullException("left");
      if (right == null)
        throw new ArgumentNullException("right");

      canDeleteRightNode = false;

      string[] allKeys = new string[left.Capacity * 2];
      long[] allValues = new long[left.Capacity * 2];

      int index = 0;
      for (int i = 0; i < left.Capacity; i++)
      {
        if (left.ChildKeys[i] == null)
        {
          break;
        }
        allKeys[index] = left.ChildKeys[i];
        allValues[index] = left.ChildValues[i];
        index++;
      }

      for (int i = 0; i < right.Capacity; i++)
      {
        if (right.ChildKeys[i] == null)
        {
          break;
        }
        allKeys[index] = right.ChildKeys[i];
        allValues[index] = right.ChildValues[i];
        index++;
      }

      // 如果左节点的容量足够，则可删除右节点
      if (index <= left.Capacity)
      {
        canDeleteRightNode = true;

        left.Clear();
        
        for (int i = 0; i < index; i++)
        {
          left.ChildKeys[i] = allKeys[i];
          left.ChildValues[i] = allValues[i];
        }

        left.Soil();
        right.Free();        

        return;
      }

      // 左节点的容量不够了
      left.Clear();
      right.Clear();
      left.Soil();
      right.Soil();

      int rightContent = index / 2;
      int leftContent = index - rightContent;
      int newIndex = 0;
      for (int i = 0; i < leftContent; i++)
      {
        left.ChildKeys[i] = allKeys[newIndex];
        left.ChildValues[i] = allValues[newIndex];
        newIndex++;
      }
      for (int i = 0; i < rightContent; i++)
      {
        right.ChildKeys[i] = allKeys[newIndex];
        right.ChildValues[i] = allValues[newIndex];
        newIndex++;
      }
    }

    #endregion

    #region Root Operation

    /// <summary>
    /// 将当前节点置为根节点
    /// </summary>
    /// <returns></returns>
    public long MakeAsRoot()
    {
      this.Parent = null;
      this.IndexInParent = -1;
      if (this.BlockNumber == StoredConstants.NullBlockNumber)
      {
        throw new BPlusTreeException("no root seek allocated to new root");
      }
      return this.BlockNumber;
    }

    /// <summary>
    /// 为指定树构造一个根节点
    /// </summary>
    /// <param name="tree">指定树</param>
    /// <param name="isLeaf">是否为叶子节点</param>
    /// <returns>根节点</returns>
    public static BPlusTreeNode MakeRoot(BPlusTreeLong tree, bool isLeaf)
    {
      return new BPlusTreeNode(tree, null, -1, isLeaf);
    }

    /// <summary>
    /// 重新组织分割树，新的根节点将有两个子节点
    /// </summary>
    /// <param name="oldRoot">原根节点</param>
    /// <param name="splitFirstKey">新根节点的第一个Key</param>
    /// <param name="splitNode">新分割出的节点</param>
    /// <param name="tree">指定的树</param>
    /// <returns>新根节点</returns>
    public static BPlusTreeNode BinaryRoot(BPlusTreeNode oldRoot, string splitFirstKey, BPlusTreeNode splitNode, BPlusTreeLong tree)
    {
      if (oldRoot == null)
        throw new ArgumentNullException("oldRoot");
      if (splitNode == null)
        throw new ArgumentNullException("splitNode");

      // 已不是叶子节点
      BPlusTreeNode newRoot = MakeRoot(tree, false);

      // 新的跟记录分割节点的第一个Key
      newRoot.ChildKeys[0] = splitFirstKey;

      // 新旧节点分别为新的根节点的索引 0 1 位置
      oldRoot.ResetParent(newRoot, 0);
      splitNode.ResetParent(newRoot, 1);

      // new root is stored elsewhere
      return newRoot;
    }

    /// <summary>
    /// 重置节点的父节点
    /// </summary>
    /// <param name="newParent">父节点</param>
    /// <param name="indexInParent">在父节点中的索引</param>
    private void ResetParent(BPlusTreeNode newParent, int indexInParent)
    {
      // keys and existing parent structure must be updated elsewhere.
      this.Parent = newParent;
      this.IndexInParent = indexInParent;

      newParent.ChildValues[indexInParent] = this.BlockNumber; // 中间节点存储的值为子节点的块序号
      newParent.ChildNodes[indexInParent] = this;

      // parent is no longer terminal
      this.Tree.ForgetTerminalNode(this.Parent); // 父节点已经有了一个具体化的子节点，父节点不再是终端节点
    }

    #endregion

    #region Find Key

    /// <summary>
    /// 判断指定的键是否存在，如存在则返回对应的值
    /// </summary>
    /// <param name="compareKey">指定的键</param>
    /// <param name="valueFound">对应的值</param>
    /// <returns>指定的键是否存在</returns>
    public bool FindKey(string compareKey, out long valueFound)
    {
      valueFound = 0; // dummy item on failure
      BPlusTreeNode leafNode;

      int position = this.FindAtOrNextPositionInLeaf(compareKey, out leafNode, false);
      if (position < leafNode.Capacity)
      {
        string key = leafNode.ChildKeys[position];
        if ((key != null) && this.Tree.Compare(key, compareKey) == 0)
        {
          valueFound = leafNode.ChildValues[position];
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// 找到指定键值的下一个键值
    /// </summary>
    /// <param name="compareKey">指定键值</param>
    /// <returns>下一个键值</returns>
    public string FindNextKey(string compareKey)
    {
      string foundKey = null;

      BPlusTreeNode leaf;
      int position = this.FindAtOrNextPositionInLeaf(compareKey, out leaf, true);
      if (position >= leaf.Capacity || leaf.ChildKeys[position] == null)
      {
        // try to traverse to the right.
        BPlusTreeNode foundInLeaf;
        leaf.TraverseToFollowingKey(leaf.Capacity, out foundInLeaf, out foundKey);
      }
      else
      {
        foundKey = leaf.ChildKeys[position];
      }

      return foundKey;
    }

    /// <summary>
    /// 查找指定索引处的下一个键值，如果没有则遍历右子树，如果仍没找到则返回空
    /// </summary>
    /// <param name="atIndex">开始查找的索引</param>
    /// <param name="foundInLeaf">找到键值的叶子节点</param>
    /// <param name="foundKey">找到的键值</param>
    private void TraverseToFollowingKey(int atIndex, out BPlusTreeNode foundInLeaf, out string foundKey)
    {
      foundInLeaf = null;
      foundKey = null;

      bool lookInParent = false;
      if (this.IsLeaf)
      {
        lookInParent = (atIndex >= this.Capacity) || (this.ChildKeys[atIndex] == null);
      }
      else
      {
        lookInParent = (atIndex > this.Capacity) || (atIndex > 0 && this.ChildKeys[atIndex - 1] == null);
      }

      if (lookInParent)
      {
        // if it's anywhere it's in the next child of parent
        if (this.Parent != null && this.IndexInParent >= 0)
        {
          this.Parent.TraverseToFollowingKey(this.IndexInParent + 1, out foundInLeaf, out foundKey);
          return;
        }
        else
        {
          return; // no such following key
        }
      }

      if (this.IsLeaf)
      {
        // leaf, we found it.
        foundInLeaf = this;
        foundKey = this.ChildKeys[atIndex];
        return;
      }
      else
      {
        // nonleaf, look in child (if there is one)
        if (atIndex == 0 || this.ChildKeys[atIndex - 1] != null)
        {
          BPlusTreeNode child = this.LoadNodeAtIndex(atIndex);
          child.TraverseToFollowingKey(0, out foundInLeaf, out foundKey);
        }
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 获取节点的第一个子节点
    /// </summary>
    /// <returns>节点的第一个子节点</returns>
    public BPlusTreeNode FirstChild()
    {
      BPlusTreeNode firstNode = this.LoadNodeAtIndex(0);
      if (firstNode == null)
      {
        throw new BPlusTreeException("no first child");
      }
      return firstNode;
    }

    /// <summary>
    /// 使节点作废，同时作废所有子节点，并从父节点中移除关系
    /// </summary>
    /// <param name="isDestroy">是否销毁掉节点</param>
    /// <returns>节点新的块序号</returns>
    public long Invalidate(bool isDestroy)
    {
      long blockNumber = this.BlockNumber;

      // 非叶子节点
      if (!this.IsLeaf)
      {
        // need to invalidate kids
        for (int i = 0; i < this.Capacity + 1; i++)
        {
          if (this.ChildNodes[i] != null)
          {
            // new block numbers are recorded automatically
            this.ChildValues[i] = this.ChildNodes[i].Invalidate(true);
          }
        }
      }

      // store if dirty
      if (this.Dirty)
      {
        blockNumber = this.DumpToNewBlock();
      }

      // remove from owner archives if present
      this.Tree.ForgetTerminalNode(this); // 我已经有了一个具体化的子节点，我不再是终端节点

      // remove from parent
      if (this.Parent != null && this.IndexInParent >= 0)
      {
        this.Parent.ChildNodes[this.IndexInParent] = null;
        this.Parent.ChildValues[this.IndexInParent] = blockNumber; // should be redundant
        this.Parent.CheckIfTerminal();
        this.IndexInParent = -1;
      }

      // render all structures useless, just in case...
      if (isDestroy)
      {
        this.Destroy();
      }

      return blockNumber;
    }

    /// <summary>
    /// 销毁节点
    /// </summary>
    private void Destroy()
    {
      // make sure the structure is useless, it should no longer be used.
      this.Tree = null;
      this.Parent = null;
      this.Capacity = -100;
      this.ChildValues = null;
      this.ChildKeys = null;
      this.ChildNodes = null;
      this.BlockNumber = StoredConstants.NullBlockNumber;
      this.IndexInParent = -100;
      this.Dirty = false;
    }

    /// <summary>
    /// 当节点被删除后释放节点及块，块即被回收
    /// </summary>
    public void Free()
    {
      if (this.BlockNumber != StoredConstants.NullBlockNumber)
      {
        if (this.Tree.FreeBlocksOnAbort.ContainsKey(this.BlockNumber))
        {
          // free it now
          this.Tree.FreeBlocksOnAbort.Remove(this.BlockNumber);
          this.Tree.ReclaimBlock(this.BlockNumber);
        }
        else
        {
          // free on commit
          this.Tree.FreeBlocksOnCommit[this.BlockNumber] = this.BlockNumber;
        }
      }
      this.BlockNumber = StoredConstants.NullBlockNumber; // don't do it twice...
    }

    /// <summary>
    /// 合理性检查
    /// </summary>
    /// <param name="visited"></param>
    /// <returns></returns>
    public string SanityCheck(Hashtable visited)
    {
      string result = null;

      if (visited == null)
      {
        visited = new Hashtable();
      }
      if (visited.ContainsKey(this))
      {
        throw new BPlusTreeException("node visited twice " + this.BlockNumber);
      }

      visited[this] = this.BlockNumber;
      if (this.BlockNumber != StoredConstants.NullBlockNumber)
      {
        if (visited.ContainsKey(this.BlockNumber))
        {
          throw new BPlusTreeException("block number seen twice " + this.BlockNumber);
        }
        visited[this.BlockNumber] = this;
      }

      if (this.Parent != null)
      {
        if (this.Parent.IsLeaf)
        {
          throw new BPlusTreeException("parent is leaf");
        }

        this.Parent.LoadNodeAtIndex(this.IndexInParent);
        if (this.Parent.ChildNodes[this.IndexInParent] != this)
        {
          throw new BPlusTreeException("incorrect index in parent");
        }

        // since not at root there should be at least size/2 keys
        int limit = this.Capacity / 2;
        if (this.IsLeaf)
        {
          limit--;
        }
        for (int i = 0; i < limit; i++)
        {
          if (this.ChildKeys[i] == null)
          {
            throw new BPlusTreeException("null child in first half");
          }
        }
      }

      result = this.ChildKeys[0]; // for leaf
      if (!this.IsLeaf)
      {
        this.LoadNodeAtIndex(0);
        result = this.ChildNodes[0].SanityCheck(visited);

        for (int i = 0; i < this.Capacity; i++)
        {
          if (this.ChildKeys[i] == null)
          {
            break;
          }

          this.LoadNodeAtIndex(i + 1);
          string least = this.ChildNodes[i + 1].SanityCheck(visited);
          if (least == null)
          {
            throw new BPlusTreeException("null least in child doesn't match node entry " + this.ChildKeys[i]);
          }
          if (!least.Equals(this.ChildKeys[i]))
          {
            throw new BPlusTreeException("least in child " + least + " doesn't match node entry " + this.ChildKeys[i]);
          }
        }
      }

      // look for duplicate keys
      string lastkey = this.ChildKeys[0];
      for (int i = 1; i < this.Capacity; i++)
      {
        if (this.ChildKeys[i] == null)
        {
          break;
        }
        if (lastkey.Equals(this.ChildKeys[i]))
        {
          throw new BPlusTreeException("duplicate key in node " + lastkey);
        }
        lastkey = this.ChildKeys[i];
      }

      return result;
    }

    #endregion

    #region ToString

    /// <summary>
    /// 将节点转成字符串描述
    /// </summary>
    /// <param name="indent">缩进</param>
    /// <returns>节点的字符串描述</returns>
    public string ToText(string indent)
    {
      StringBuilder sb = new StringBuilder();

      string indentPlus = indent + "\t";

      sb.AppendLine(indent + "Node{");

      sb.Append(indentPlus + "IsLeaf = " + this.IsLeaf);
      sb.Append(", Capacity = " + this.Capacity);
      sb.Append(", Count = " + this.Count);
      sb.Append(", Dirty = " + this.Dirty);
      sb.Append(", BlockNumber = " + this.BlockNumber);
      sb.Append(", ParentBlockNumber = " + (this.Parent == null ? "NULL" : this.Parent.BlockNumber.ToString(CultureInfo.InvariantCulture)));
      sb.Append(", IndexInParent = " + this.IndexInParent);
      sb.AppendLine();

      if (this.IsLeaf) // 如果是叶子节点
      {
        for (int i = 0; i < this.Capacity; i++)
        {
          string key = this.ChildKeys[i];
          long value = this.ChildValues[i];
          if (key != null)
          {
            key = string.IsNullOrEmpty(key) ? "NULL" : key;
            sb.AppendLine(indentPlus + "[Key : " + key + ", Value : " + value + "]");
          }
        }
      }
      else // 如果是非叶子节点
      {
        int count = 0;
        for (int i = 0; i < this.Capacity; i++)
        {
          string key = this.ChildKeys[i];
          long value = this.ChildValues[i];
          if (key != null)
          {
            key = string.IsNullOrEmpty(key) ? "NULL" : key;
            sb.AppendLine(indentPlus + "[Key : " + key + ", Value : " + value + "]");

            count++;
          }
        }

        for (int i = 0; i <= count; i++)
        {
          try
          {
            this.LoadNodeAtIndex(i);
            sb.Append(this.ChildNodes[i].ToText(indentPlus));
          }
          catch (BPlusTreeException ex)
          {
            sb.AppendLine(ex.Message);
          }
        }
      }

      sb.AppendLine(indent + "}");

      return sb.ToString();
    }

    #endregion
  }
}
