
namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 二叉树节点
  /// </summary>
  /// <typeparam name="T">The item type</typeparam>
  public class BinaryTreeNode<T>
  {
    #region Constructors

    /// <summary>
    /// 二叉树节点
    /// </summary>
    public BinaryTreeNode()
    {
    }

    /// <summary>
    /// 二叉树节点
    /// </summary>
    /// <param name="value">节点中的值</param>
    public BinaryTreeNode(T value)
    {
      this.Value = value;
    }

    /// <summary>
    /// 二叉树节点
    /// </summary>
    /// <param name="value">节点中的值</param>
    /// <param name="parent">节点的父节点</param>
    public BinaryTreeNode(T value, BinaryTreeNode<T> parent)
    {
      this.Value = value;
      this.Parent = parent;
    }

    /// <summary>
    /// 二叉树节点
    /// </summary>
    /// <param name="value">节点中的值</param>
    /// <param name="parent">节点的父节点</param>
    /// <param name="left">节点的左节点</param>
    /// <param name="right">节点的右节点</param>
    public BinaryTreeNode(T value, 
      BinaryTreeNode<T> parent, 
      BinaryTreeNode<T> left, 
      BinaryTreeNode<T> right)
    {
      this.Value = value;
      this.Right = right;
      this.Left = left;
      this.Parent = parent;
    }

    #endregion

    #region Properties

    /// <summary>
    /// 节点值
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// 父节点
    /// </summary>
    public BinaryTreeNode<T> Parent { get; set; }

    /// <summary>
    /// 左节点
    /// </summary>
    public BinaryTreeNode<T> Left { get; set; }

    /// <summary>
    /// 右节点
    /// </summary>
    public BinaryTreeNode<T> Right { get; set; }

    /// <summary>
    /// 是否为根节点
    /// </summary>
    public bool IsRoot { get { return Parent == null; } }

    /// <summary>
    /// 是否为叶子节点
    /// </summary>
    public bool IsLeaf { get { return Left == null && Right == null; } }

    /// <summary>
    /// 是否为可访问的
    /// </summary>
    internal bool Visited { get; set; }

    #endregion

    #region Public Overridden Functions

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return Value.ToString();
    }

    #endregion
  }
}
