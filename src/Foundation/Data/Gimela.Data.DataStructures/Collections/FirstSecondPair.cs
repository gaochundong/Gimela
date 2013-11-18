using System;
using System.Text;

namespace Gimela.Data.DataStructures
{
  /// <summary>
  /// 双值对
  /// </summary>
  /// <typeparam name="TFirst">第一个值的类型</typeparam>
  /// <typeparam name="TSecond">第二个值的类型</typeparam>
  [Serializable]
  public struct FirstSecondPair<TFirst, TSecond>
  {
    private TFirst first;
    private TSecond second;

    /// <summary>
    /// 第一个值
    /// </summary>
    public TFirst First
    {
      get
      {
        return this.first;
      }
    }

    /// <summary>
    /// 第二个值
    /// </summary>
    public TSecond Second
    {
      get
      {
        return this.second;
      }
    }

    /// <summary>
    /// 双值对
    /// </summary>
    /// <param name="first">第一个值</param>
    /// <param name="second">第二个值</param>
    public FirstSecondPair(TFirst first, TSecond second)
    {
      if (first == null)
        throw new ArgumentNullException("first");
      if (second == null)
        throw new ArgumentNullException("second");

      this.first = first;
      this.second = second;
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;

      FirstSecondPair<TFirst, TSecond> target = (FirstSecondPair<TFirst, TSecond>)obj;
      return this.First.Equals(target.First) && this.Second.Equals(target.Second);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append('[');

      if (this.First != null)
      {
        sb.Append(this.First.ToString());
      }

      sb.Append(", ");

      if (this.Second != null)
      {
        sb.Append(this.Second.ToString());
      }

      sb.Append(']');

      return sb.ToString();
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(FirstSecondPair<TFirst, TSecond> left, FirstSecondPair<TFirst, TSecond> right)
    {
      if (((object)left == null) || ((object)right == null))
      {
        return false;
      }

      return left.Equals(right);
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator !=(FirstSecondPair<TFirst, TSecond> left, FirstSecondPair<TFirst, TSecond> right)
    {
      return !(left == right);
    }
  }
}
