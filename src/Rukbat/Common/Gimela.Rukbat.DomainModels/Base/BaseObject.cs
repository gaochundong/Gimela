using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Gimela.Rukbat.DomainModels
{
  [Serializable]
  [DataContract]
  public abstract class BaseObject : IBaseObject
  {
    #region Constructors

    public BaseObject()
    {
    }

    #endregion

    #region Properties

    [DataMember]
    [XmlAttribute]
    public string Id { get; set; }

    [DataMember]
    [XmlAttribute]
    public virtual string Name { get; set; }

    #endregion

    #region IComparable Members

    /// <summary>
    /// Compares the current instance with another object of the same 
    /// type and returns an integer that indicates whether the current instance precedes, 
    /// follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the relative order of 
    /// the objects being compared. The return value has these meanings:
    /// Value
    /// Meaning
    /// Less than zero
    /// This instance is less than <paramref name="obj"/>.
    /// Zero
    /// This instance is equal to <paramref name="obj"/>.
    /// Greater than zero
    /// This instance is greater than <paramref name="obj"/>.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    /// 	<paramref name="obj"/> is not the same type as this instance.
    /// </exception>
    public virtual int CompareTo(object obj)
    {
      // 小于零 此实例小于 obj。 
      // 零 此实例等于 obj。 
      // 大于零 此实例大于 obj。

      if (obj is BaseObject)
      {
        BaseObject other = (BaseObject)obj;

        // 根据名称进行比较
        return this.Name.CompareTo(other.Name);
      }
      else
      {
        throw new ArgumentException("The target object is not a <ObjectBase>.");
      }
    }

    #endregion

    #region Override ToString

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, @"ID={0}, Name={1}", this.Id, this.Name.ToString());
    }

    #endregion

    #region Overload Operator

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(BaseObject left, BaseObject right)
    {
      if (!(left is BaseObject) && !(right is BaseObject))
      {
        return true; // 这里包含判断NULL的情形
      }
      else if (!(left is BaseObject) || !(right is BaseObject))
      {
        return false;
      }
      else
      {
        return left.Id == right.Id;
      }
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(BaseObject left, BaseObject right)
    {
      return !(left == right);
    }

    #endregion

    #region Override Equals

    /// <summary>
    /// Determines whether the specified <see cref="System.Object"/> 
    /// is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object"/> 
    /// to compare with this instance.</param>
    /// <returns>
    /// 	<c>true</c> if the specified <see cref="System.Object"/> 
    /// 	is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="T:System.NullReferenceException">
    /// The <paramref name="obj"/> parameter is null.
    /// </exception>
    public override bool Equals(object obj)
    {
      if (!(obj is BaseObject))
      {
        return false;
      }

      BaseObject t = (BaseObject)obj;
      return t.Id == this.Id;
    }

    #endregion

    #region Override GetHashCode

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, 
    /// suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
      return this.Id.GetHashCode();
    }

    #endregion
  }
}
