using System;

namespace Gimela.Data.Json
{
  /// <summary>
  /// 对象Id
  /// </summary>
  public class ObjectId
  {
    private string _string;

    /// <summary>
    /// 对象Id
    /// </summary>
    public ObjectId()
    {
    }

    /// <summary>
    /// 对象Id
    /// </summary>
    /// <param name="value">ObjectId中的值</param>
    public ObjectId(string value)
      : this(DecodeHex(value))
    {
    }

    /// <summary>
    /// 对象Id
    /// </summary>
    /// <param name="value">ObjectId中的值</param>
    internal ObjectId(byte[] value)
    {
      Value = value;
    }

    /// <summary>
    /// 空ObjectId
    /// </summary>
    public static ObjectId Empty
    {
      get { return new ObjectId("000000000000000000000000"); }
    }

    /// <summary>
    /// ObjectId中的值
    /// </summary>
    public byte[] Value { get; private set; }

    /// <summary>
    /// 创建新的ObjectId
    /// </summary>
    /// <returns></returns>
    public static ObjectId NewObjectId()
    {
      return new ObjectId { Value = ObjectIdGenerator.Generate() };
    }

    /// <summary>
    /// 尝试解析指定的值至ObjectId
    /// </summary>
    /// <param name="value">指定的值</param>
    /// <param name="objectId">ObjectId</param>
    /// <returns>是否成功解析</returns>
    public static bool TryParse(string value, out ObjectId objectId)
    {
      objectId = Empty;
      if (value == null || value.Length != 24)
      {
        return false;
      }

      try
      {
        objectId = new ObjectId(value);
        return true;
      }
      catch (FormatException)
      {
        return false;
      }
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(ObjectId left, ObjectId right)
    {
      if (ReferenceEquals(left, right))
      {
        return true;
      }

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
    public static bool operator !=(ObjectId left, ObjectId right)
    {
      return !(left == right);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
      return Value != null ? ToString().GetHashCode() : 0;
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      if (_string == null && Value != null)
      {
        _string = BitConverter.ToString(Value).Replace("-", string.Empty).ToLowerInvariant();
      }

      return _string;
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
      var other = obj as ObjectId;
      return Equals(other);
    }

    /// <summary>
    /// Determines whether the specified ObjectId is equal to this instance.
    /// </summary>
    /// <param name="other">The other ObjectId.</param>
    /// <returns>
    ///   <c>true</c> if the specified ObjectId is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(ObjectId other)
    {
      return other != null && ToString() == other.ToString();
    }

    /// <summary>
    /// 解码16进制数据字符串
    /// </summary>
    /// <param name="value">16进制数据字符串</param>
    /// <returns></returns>
    protected static byte[] DecodeHex(string value)
    {
      if (string.IsNullOrEmpty(value))
        throw new ArgumentNullException("value");

      var chars = value.ToCharArray();
      var numberChars = chars.Length;
      var bytes = new byte[numberChars / 2];

      for (var i = 0; i < numberChars; i += 2)
      {
        bytes[i / 2] = Convert.ToByte(new string(chars, i, 2), 16);
      }

      return bytes;
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="Gimela.Data.Json.ObjectId"/> to <see cref="System.String"/>.
    /// </summary>
    /// <param name="objectId">The object id.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator string(ObjectId objectId)
    {
      return objectId == null ? null : objectId.ToString();
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Gimela.Data.Json.ObjectId"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator ObjectId(string value)
    {
      return new ObjectId(value);
    }
  }
}
