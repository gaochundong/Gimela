using System;
using System.Collections.Generic;

namespace Gimela.Data.Magpie
{
  /// <summary>
  /// 数据库URL
  /// </summary>
  [Serializable]
  public class MagpieUrl : IEquatable<MagpieUrl>
  {
    private static object __staticLock = new object();
    private static Dictionary<string, MagpieUrl> __cache = new Dictionary<string, MagpieUrl>();

    private MagpieServerSettings _serverSettings;
    private string _url;

    /// <summary>
    /// 数据库URL
    /// </summary>
    /// <param name="url">数据库URL字符串</param>
    public MagpieUrl(string url)
    {
      var builder = new MagpieUrlBuilder(url);
      _serverSettings = builder.ToServerSettings();
      _url = builder.ToString();
    }

    /// <summary>
    /// 数据库服务器地址
    /// </summary>
    public MagpieServerAddress Server
    {
      get { return _serverSettings.Address; }
    }

    /// <summary>
    /// 数据库URL
    /// </summary>
    public string Url
    {
      get { return _url; }
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(MagpieUrl left, MagpieUrl right)
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
    public static bool operator !=(MagpieUrl left, MagpieUrl right)
    {
      return !(left == right);
    }

    /// <summary>
    /// 清理缓存
    /// </summary>
    public static void ClearCache()
    {
      __cache.Clear();
    }

    /// <summary>
    /// 根据字符串创建数据库URL
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static MagpieUrl Create(string url)
    {
      lock (__staticLock)
      {
        MagpieUrl orioleUrl;
        if (!__cache.TryGetValue(url, out orioleUrl))
        {
          orioleUrl = new MagpieUrl(url);
          var canonicalUrl = orioleUrl.ToString();
          if (canonicalUrl != url)
          {
            if (__cache.ContainsKey(canonicalUrl))
            {
              orioleUrl = __cache[canonicalUrl];
            }
            else
            {
              __cache[canonicalUrl] = orioleUrl;
            }
          }
          __cache[url] = orioleUrl;
        }
        return orioleUrl;
      }
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the other parameter; otherwise, false.
    /// </returns>
    public bool Equals(MagpieUrl other)
    {
      if (object.ReferenceEquals(other, null) || GetType() != other.GetType()) { return false; }
      return _url == other._url;
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
      return Equals(obj as MagpieUrl);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
      return _url.GetHashCode();
    }

    /// <summary>
    /// 生成数据库服务器配置
    /// </summary>
    /// <returns></returns>
    public MagpieServerSettings ToServerSettings()
    {
      return _serverSettings;
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return _url;
    }
  }
}
