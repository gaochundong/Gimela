using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Gimela.Data.Magpie
{
  /// <summary>
  /// 数据库服务器
  /// </summary>
  public class MagpieServer
  {
    #region Static Fields

    private readonly static object __staticLock = new object();
    private readonly static Dictionary<MagpieServerSettings, MagpieServer> __servers = new Dictionary<MagpieServerSettings, MagpieServer>();
    /// <summary>
    /// 数据库实例序号计数器
    /// </summary>
    private static int __nextSequentialId;
    /// <summary>
    /// 数据库名称中的非法字符串
    /// </summary>
    private static HashSet<char> __invalidDatabaseNameChars;

    #endregion

    #region Fields

    private readonly object _serverLock = new object();
    private readonly MagpieServerSettings _settings;
    private readonly Dictionary<MagpieDatabaseSettings, MagpieDatabase> _databases = new Dictionary<MagpieDatabaseSettings, MagpieDatabase>();
    private int _sequentialId;

    #endregion

    #region Ctors
    
    static MagpieServer()
    {
      __invalidDatabaseNameChars = new HashSet<char>() { '\0', ' ', '.', '$', '/', '\\' };
      foreach (var c in Path.GetInvalidPathChars()) { __invalidDatabaseNameChars.Add(c); }
      foreach (var c in Path.GetInvalidFileNameChars()) { __invalidDatabaseNameChars.Add(c); }
    }

    /// <summary>
    /// 数据库服务器
    /// </summary>
    /// <param name="settings">数据库服务器配置</param>
    public MagpieServer(MagpieServerSettings settings)
    {
      _settings = settings;
      _sequentialId = Interlocked.Increment(ref __nextSequentialId);
    }

    #endregion

    #region Create

    /// <summary>
    /// 根据数据库URL字符串生成数据库服务器
    /// </summary>
    /// <param name="url">数据库URL字符串</param>
    /// <returns>数据库服务器</returns>
    public static MagpieServer Create(string url)
    {
      if (string.IsNullOrEmpty(url))
        throw new ArgumentNullException("url");

      var orioleUrl = MagpieUrl.Create(url);
      return Create(orioleUrl);
    }

    /// <summary>
    /// 根据数据库URL生成数据库服务器
    /// </summary>
    /// <param name="url">数据库URL</param>
    /// <returns>数据库服务器</returns>
    public static MagpieServer Create(MagpieUrl url)
    {
      if (url == null)
        throw new ArgumentNullException("url");

      return Create(url.ToServerSettings());
    }

    /// <summary>
    /// 根据数据库配置生成数据库服务器
    /// </summary>
    /// <param name="settings">数据库配置</param>
    /// <returns>数据库服务器</returns>
    public static MagpieServer Create(MagpieServerSettings settings)
    {
      if (settings == null)
        throw new ArgumentNullException("settings");

      lock (__staticLock)
      {
        MagpieServer server;
        if (!__servers.TryGetValue(settings, out server))
        {
          if (__servers.Count >= MaxServerCount)
          {
            var message = string.Format(CultureInfo.InvariantCulture, "Already created {0} servers which is the maximum number of servers allowed.", MaxServerCount);
            throw new MagpieException(message);
          }
          server = new MagpieServer(settings);
          __servers.Add(settings, server);
        }
        return server;
      }
    }

    #endregion

    #region Static Properties
    
    /// <summary>
    /// 数据库服务器最大实例数量
    /// </summary>
    public static int MaxServerCount
    {
      get { return MagpieDefaults.MaxServerCount; }
    }

    /// <summary>
    /// 当前的数据库服务器数量
    /// </summary>
    public static int ServerCount
    {
      get
      {
        lock (__staticLock)
        {
          return __servers.Count;
        }
      }
    }

    #endregion

    #region Properties
    
    /// <summary>
    /// 数据库服务器的唯一序号
    /// </summary>
    public virtual int SequentialId
    {
      get { return _sequentialId; }
    }

    /// <summary>
    /// 数据库服务器的配置
    /// </summary>
    public virtual MagpieServerSettings Settings
    {
      get { return _settings; }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 是否给定的数据库名称合法
    /// </summary>
    /// <param name="databaseName">数据库名称</param>
    /// <returns>是否给定的数据库名称合法</returns>
    public virtual bool IsDatabaseNameValid(string databaseName)
    {
      if (string.IsNullOrEmpty(databaseName))
      {
        throw new ArgumentNullException("databaseName");
      }

      foreach (var c in databaseName)
      {
        if (__invalidDatabaseNameChars.Contains(c))
        {
          return false;
        }
      }

      if (Encoding.UTF8.GetBytes(databaseName).Length > 64)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// 获取指定名称的数据库
    /// </summary>
    /// <param name="databaseName">数据库名称</param>
    /// <returns>数据库</returns>
    public virtual MagpieDatabase GetDatabase(string databaseName)
    {
      if (string.IsNullOrEmpty(databaseName))
      {
        throw new ArgumentNullException("databaseName");
      }

      var databaseSettings = new MagpieDatabaseSettings(this, databaseName);
      return GetDatabase(databaseSettings);
    }

    /// <summary>
    /// 获取指定配置的数据库
    /// </summary>
    /// <param name="databaseSettings">数据库配置</param>
    /// <returns>数据库</returns>
    protected virtual MagpieDatabase GetDatabase(MagpieDatabaseSettings databaseSettings)
    {
      if (databaseSettings == null)
        throw new ArgumentNullException("databaseSettings");

      lock (_serverLock)
      {
        MagpieDatabase database;
        if (!_databases.TryGetValue(databaseSettings, out database))
        {
          database = new MagpieDatabase(this, databaseSettings);
          _databases.Add(databaseSettings, database);

          Directory.CreateDirectory(databaseSettings.Path);
        }
        return database;
      }
    }

    /// <summary>
    /// 删除指定名称的数据库
    /// </summary>
    /// <param name="databaseName">数据库名称</param>
    public virtual void DropDatabase(string databaseName)
    {
      if (string.IsNullOrEmpty(databaseName))
      {
        throw new ArgumentNullException("databaseName");
      }

      lock (_serverLock)
      {
        var databaseSettings = new MagpieDatabaseSettings(this, databaseName);
        _databases.Remove(databaseSettings);

        Directory.Delete(databaseSettings.Path, true);
      }
    }

    /// <summary>
    /// 关闭数据库服务器
    /// </summary>
    public virtual void Shutdown()
    {
      lock (_serverLock)
      {
        foreach (var item in _databases)
        {
          item.Value.Shutdown();
        }
        _databases.Clear();
      }
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, "{0};SequentialId={1};", Settings, SequentialId);
    }

    #endregion
  }
}
