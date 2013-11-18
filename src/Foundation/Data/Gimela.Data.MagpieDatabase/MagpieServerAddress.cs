using System;
using System.Globalization;

namespace Gimela.Data.Magpie
{
  /// <summary>
  /// 数据库服务器地址
  /// </summary>
  [Serializable]
  public class MagpieServerAddress : IEquatable<MagpieServerAddress>
  {
    private string _path;

    /// <summary>
    /// 数据库服务器地址
    /// </summary>
    /// <param name="path">数据库路径</param>
    public MagpieServerAddress(string path)
    {
      if (string.IsNullOrEmpty(path))
      {
        throw new ArgumentNullException("path");
      }

      _path = path;
    }

    /// <summary>
    /// 根据路径创建数据库服务器地址
    /// </summary>
    /// <param name="path">数据库路径</param>
    /// <returns>数据库服务器地址</returns>
    public static MagpieServerAddress Create(string path)
    {
      return new MagpieServerAddress(path);
    }

    /// <summary>
    /// 数据库路径
    /// </summary>
    public string Path
    {
      get { return _path; }
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(MagpieServerAddress left, MagpieServerAddress right)
    {
      return object.Equals(left, right);
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator !=(MagpieServerAddress left, MagpieServerAddress right)
    {
      return !(left == right);
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
      return Equals(obj as MagpieServerAddress);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the other parameter; otherwise, false.
    /// </returns>
    public bool Equals(MagpieServerAddress other)
    {
      if (object.ReferenceEquals(other, null) || GetType() != other.GetType()) { return false; }
      return _path.Equals(other._path, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
      int hash = 17;
      hash = 37 * hash + _path.ToLower(CultureInfo.InvariantCulture).GetHashCode();
      return hash;
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, "{0}", _path);
    }
  }
}
