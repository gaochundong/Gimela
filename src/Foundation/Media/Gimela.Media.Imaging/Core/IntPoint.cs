using System;
using System.ComponentModel;

namespace Gimela.Media.Imaging
{
  /// <summary>
  /// 坐标点，使用整型值存储坐标
  /// </summary>
  [Serializable]
  public struct IntPoint
  {
    /// <summary> 
    /// X 坐标
    /// </summary> 
    public int X;

    /// <summary> 
    /// Y 坐标
    /// </summary> 
    public int Y;

    /// <summary>
    /// 初始化坐标点的新实例
    /// </summary>
    /// <param name="x">X axis coordinate.</param>
    /// <param name="y">Y axis coordinate.</param>
    public IntPoint(int x, int y)
    {
      this.X = x;
      this.Y = y;
    }

    /// <summary>
    /// 计算两点之间的距离
    /// </summary>
    /// <param name="anotherPoint">Point to calculate distance to.</param>
    /// <returns>Returns Euclidean distance between this point and
    /// <paramref name="anotherPoint"/> points.</returns>
    public float DistanceTo(IntPoint anotherPoint)
    {
      int dx = X - anotherPoint.X;
      int dy = Y - anotherPoint.Y;

      return (float)System.Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Implements the operator +.
    /// </summary>
    /// <param name="point1">The point1.</param>
    /// <param name="point2">The point2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static IntPoint operator +(IntPoint point1, IntPoint point2)
    {
      return new IntPoint(point1.X + point2.X, point1.Y + point2.Y);
    }

    /// <summary>
    /// Adds the specified point1.
    /// </summary>
    /// <param name="point1">The point1.</param>
    /// <param name="point2">The point2.</param>
    /// <returns></returns>
    public static IntPoint Add(IntPoint point1, IntPoint point2)
    {
      return new IntPoint(point1.X + point2.X, point1.Y + point2.Y);
    }

    /// <summary>
    /// Implements the operator -.
    /// </summary>
    /// <param name="point1">The point1.</param>
    /// <param name="point2">The point2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static IntPoint operator -(IntPoint point1, IntPoint point2)
    {
      return new IntPoint(point1.X - point2.X, point1.Y - point2.Y);
    }

    /// <summary>
    /// Subtracts the specified point1.
    /// </summary>
    /// <param name="point1">The point1.</param>
    /// <param name="point2">The point2.</param>
    /// <returns></returns>
    public static IntPoint Subtract(IntPoint point1, IntPoint point2)
    {
      return new IntPoint(point1.X - point2.X, point1.Y - point2.Y);
    }

    /// <summary>
    /// Implements the operator +.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="valueToAdd">The value to add.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static IntPoint operator +(IntPoint point, int valueToAdd)
    {
      return new IntPoint(point.X + valueToAdd, point.Y + valueToAdd);
    }

    /// <summary>
    /// Adds the specified point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="valueToAdd">The value to add.</param>
    /// <returns></returns>
    public static IntPoint Add(IntPoint point, int valueToAdd)
    {
      return new IntPoint(point.X + valueToAdd, point.Y + valueToAdd);
    }

    /// <summary>
    /// Implements the operator -.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="valueToSubtract">The value to subtract.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static IntPoint operator -(IntPoint point, int valueToSubtract)
    {
      return new IntPoint(point.X - valueToSubtract, point.Y - valueToSubtract);
    }

    /// <summary>
    /// Subtracts the specified point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="valueToSubtract">The value to subtract.</param>
    /// <returns></returns>
    public static IntPoint Subtract(IntPoint point, int valueToSubtract)
    {
      return new IntPoint(point.X - valueToSubtract, point.Y - valueToSubtract);
    }

    /// <summary>
    /// Implements the operator *.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="factor">The factor.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static IntPoint operator *(IntPoint point, int factor)
    {
      return new IntPoint(point.X * factor, point.Y * factor);
    }

    /// <summary>
    /// Multiplies the specified point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="factor">The factor.</param>
    /// <returns></returns>
    public static IntPoint Multiply(IntPoint point, int factor)
    {
      return new IntPoint(point.X * factor, point.Y * factor);
    }

    /// <summary>
    /// Implements the operator /.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="factor">The factor.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static IntPoint operator /(IntPoint point, int factor)
    {
      return new IntPoint(point.X / factor, point.Y / factor);
    }

    /// <summary>
    /// Divides the specified point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="factor">The factor.</param>
    /// <returns></returns>
    public static IntPoint Divide(IntPoint point, int factor)
    {
      return new IntPoint(point.X / factor, point.Y / factor);
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="point1">The point1.</param>
    /// <param name="point2">The point2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(IntPoint point1, IntPoint point2)
    {
      return ((point1.X == point2.X) && (point1.Y == point2.Y));
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="point1">The point1.</param>
    /// <param name="point2">The point2.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator !=(IntPoint point1, IntPoint point2)
    {
      return ((point1.X != point2.X) || (point1.Y != point2.Y));
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
      return (obj is IntPoint) ? (this == (IntPoint)obj) : false;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
      return X.GetHashCode() + Y.GetHashCode();
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", X, Y);
    }
  }
}
