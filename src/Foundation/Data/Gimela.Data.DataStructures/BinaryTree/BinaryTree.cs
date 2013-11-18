using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 二叉树
  /// </summary>
  /// <typeparam name="T">二叉树中节点内容类型</typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public class BinaryTree<T> : ICollection<T>, IEnumerable<T> where T : IComparable<T>
  {
    #region Constructor

    /// <summary>
    /// 二叉树
    /// </summary>
    public BinaryTree()
    {
      NumberOfNodes = 0;
    }

    /// <summary>
    /// 二叉树
    /// </summary>
    /// <param name="root">二叉树根节点</param>
    public BinaryTree(BinaryTreeNode<T> root)
      : this()
    {
      this.Root = root;
    }

    #endregion

    #region Properties

    /// <summary>
    /// 树的根节点
    /// </summary>
    public BinaryTreeNode<T> Root { get; set; }

    /// <summary>
    /// 树中节点的数量
    /// </summary>
    protected int NumberOfNodes { get; set; }

    /// <summary>
    /// 树是否为空
    /// </summary>
    public bool IsEmpty { get { return Root == null; } }

    /// <summary>
    /// 获取树中节点的最小值
    /// </summary>
    public T MinValue
    {
      get
      {
        if (IsEmpty)
          return default(T);

        BinaryTreeNode<T> minNode = Root;
        while (minNode.Left != null)
          minNode = minNode.Left;

        return minNode.Value;
      }
    }

    /// <summary>
    /// 获取树中节点的最大值
    /// </summary>
    public T MaxValue
    {
      get
      {
        if (IsEmpty)
          return default(T);

        BinaryTreeNode<T> maxNode = Root;
        while (maxNode.Right != null)
          maxNode = maxNode.Right;

        return maxNode.Value;
      }
    }

    #endregion

    #region IEnumerable<T> Members

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> 
    /// that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<T> GetEnumerator()
    {
      foreach (BinaryTreeNode<T> node in Traverse(Root))
      {
        yield return node.Value;
      }
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> 
    /// object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      foreach (BinaryTreeNode<T> node in Traverse(Root))
      {
        yield return node.Value;
      }
    }

    #endregion

    #region ICollection<T> Members

    /// <summary>
    /// 新增节点
    /// </summary>
    /// <param name="item">The object to add to the 
    /// <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
    /// <exception cref="T:System.NotSupportedException">The 
    /// <see cref="T:System.Collections.Generic.ICollection`1"></see> 
    /// is read-only.</exception>
    public void Add(T item)
    {
      if (Root == null)
      {
        Root = new BinaryTreeNode<T>(item);
        ++NumberOfNodes;
      }
      else
      {
        Insert(item);
      }
    }

    /// <summary>
    /// 清除树
    /// </summary>
    public void Clear()
    {
      Root = null;
    }

    /// <summary>
    /// 树中是否包含此节点
    /// </summary>
    /// <param name="item">The object to locate in the 
    /// <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
    /// <returns>
    /// true if item is found in the 
    /// <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
    /// </returns>
    public bool Contains(T item)
    {
      if (IsEmpty)
        return false;

      BinaryTreeNode<T> currentNode = Root;
      while (currentNode != null)
      {
        int comparedValue = currentNode.Value.CompareTo(item);
        if (comparedValue == 0)
          return true;
        else if (comparedValue < 0)
          currentNode = currentNode.Left;
        else
          currentNode = currentNode.Right;
      }

      return false;
    }

    /// <summary>
    /// 将树中节点拷贝至目标数组
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayIndex">Index of the array.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
      T[] tempArray = new T[NumberOfNodes];
      int counter = 0;
      foreach (T value in this)
      {
        tempArray[counter] = value;
        ++counter;
      }
      Array.Copy(tempArray, 0, array, arrayIndex, Count);
    }

    /// <summary>
    /// 树中节点的数量
    /// </summary>
    public int Count
    {
      get { return NumberOfNodes; }
    }

    /// <summary>
    /// 树是否为只读
    /// </summary>
    public bool IsReadOnly
    {
      get { return false; }
    }

    /// <summary>
    /// 移除节点
    /// </summary>
    /// <param name="item">节点值</param>
    /// <returns>是否移除成功</returns>
    public bool Remove(T item)
    {
      BinaryTreeNode<T> node = Find(item);
      if (node == null)
        return false;

      List<T> values = new List<T>();
      foreach (BinaryTreeNode<T> l in Traverse(node.Left))
      {
        values.Add(l.Value);
      }
      foreach (BinaryTreeNode<T> r in Traverse(node.Right))
      {
        values.Add(r.Value);
      }

      if (node.Parent.Left == node)
      {
        node.Parent.Left = null;
      }
      else
      {
        node.Parent.Right = null;
      }

      node.Parent = null;

      foreach (T v in values)
      {
        this.Add(v);
      }

      return true;
    }

    #endregion

    #region Private Functions

    /// <summary>
    /// 查找指定值的节点
    /// </summary>
    /// <param name="value">指定值</param>
    /// <returns>
    /// 指定值的节点
    /// </returns>
    protected BinaryTreeNode<T> Find(T value)
    {
      foreach (BinaryTreeNode<T> node in Traverse(Root))
        if (node.Value.Equals(value))
          return node;
      return null;
    }

    /// <summary>
    /// 遍历树
    /// </summary>
    /// <param name="node">遍历搜索的起始节点</param>
    /// <returns>
    /// The individual items from the tree
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    protected IEnumerable<BinaryTreeNode<T>> Traverse(BinaryTreeNode<T> node)
    {
      // 遍历左子树
      if (node.Left != null)
      {
        foreach (BinaryTreeNode<T> left in Traverse(node.Left))
          yield return left;
      }

      // 中序遍历二叉树, 顺序是 左子树，根，右子树
      yield return node;

      // 遍历右子树
      if (node.Right != null)
      {
        foreach (BinaryTreeNode<T> right in Traverse(node.Right))
          yield return right;
      }
    }

    /// <summary>
    /// 插入节点
    /// </summary>
    /// <param name="value">插入的节点值</param>
    protected void Insert(T value)
    {
      // 从根节点开始比较
      BinaryTreeNode<T> currentNode = Root;
      
      while (true)
      {
        if (currentNode == null)
          throw new InvalidProgramException("The current tree node cannot be null.");

        // 比较当前节点与新节点的值
        int comparedValue = currentNode.Value.CompareTo(value);
        if (comparedValue < 0)
        {
          // 当前节点值小于新节点值
          if (currentNode.Left == null)
          {
            currentNode.Left = new BinaryTreeNode<T>(value, currentNode);
            ++NumberOfNodes;
            return;
          }
          else
          {
            currentNode = currentNode.Left;
          }
        }
        else if (comparedValue > 0)
        {
          // 当前节点值大于新节点值
          if (currentNode.Right == null)
          {
            currentNode.Right = new BinaryTreeNode<T>(value, currentNode);
            ++NumberOfNodes;
            return;
          }
          else
          {
            currentNode = currentNode.Right;
          }
        }
        else
        {
          // 当前节点值等于新节点值
          currentNode = currentNode.Right;
        }
      }
    }

    #endregion
  }
}
