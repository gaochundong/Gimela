using System;
using System.Globalization;

namespace Gimela.Data.Magpie
{
  /// <summary>
  /// 数据库服务器配置
  /// </summary>
  [Serializable]
  public class MagpieServerSettings
  {
    private MagpieServerAddress _address;

    /// <summary>
    /// 数据库服务器配置
    /// </summary>
    public MagpieServerSettings()
    {
      _address = null;
    }

    /// <summary>
    /// 数据库服务器配置
    /// </summary>
    /// <param name="address">数据库服务器地址</param>
    public MagpieServerSettings(MagpieServerAddress address)
      : this()
    {
      if (address == null)
      {
        throw new ArgumentNullException("address");
      }

      _address = address;
    }

    /// <summary>
    /// 数据库服务器地址
    /// </summary>
    public MagpieServerAddress Address
    {
      get { return _address; }
      set { _address = value; }
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
      var rhs = obj as MagpieServerSettings;
      if (rhs == null)
      {
        return false;
      }
      else
      {
        return _address.Equals(rhs._address);
      }
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
      hash = 37 * hash + _address.GetHashCode();
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
      return string.Format(CultureInfo.InvariantCulture, "Server={0};", _address.ToString());
    }
  }
}
