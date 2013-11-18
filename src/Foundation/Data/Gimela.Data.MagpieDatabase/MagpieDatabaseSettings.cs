using System;
using System.Globalization;

namespace Gimela.Data.Magpie
{
  /// <summary>
  /// 数据库配置
  /// </summary>
  public class MagpieDatabaseSettings
  {
    private MagpieServer _server;
    private string _databaseName;

    /// <summary>
    /// 数据库配置
    /// </summary>
    /// <param name="server">数据库服务器</param>
    /// <param name="databaseName">数据库名称</param>
    public MagpieDatabaseSettings(MagpieServer server, string databaseName)
    {
      if (server == null)
      {
        throw new ArgumentNullException("server");
      }
      if (databaseName == null)
      {
        throw new ArgumentNullException("databaseName");
      }

      _server = server;
      _databaseName = databaseName;
    }

    /// <summary>
    /// 数据库服务器
    /// </summary>
    public MagpieServer Server
    {
      get { return _server; }
    }

    /// <summary>
    /// 数据库名称
    /// </summary>
    public string DatabaseName
    {
      get { return _databaseName; }
    }

    /// <summary>
    /// 数据库路径
    /// </summary>
    public string Path
    {
      get { return System.IO.Path.Combine(_server.Settings.Address.Path, _databaseName); }
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
      var rhs = obj as MagpieDatabaseSettings;
      if (rhs == null)
      {
        return false;
      }
      else
      {
        return _server.Settings == rhs._server.Settings && _databaseName == rhs._databaseName;
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
      hash = 37 * hash + _server.Settings.GetHashCode();
      hash = 37 * hash + _databaseName.GetHashCode();
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
      return string.Format(CultureInfo.InvariantCulture, "DatabaseName={0};", _databaseName);
    }
  }
}
