using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gimela.Data.Magpie
{
  /// <summary>
  /// 数据库
  /// </summary>
  public class MagpieDatabase
  {
    private object _databaseLock = new object();
    private MagpieServer _server;
    private MagpieDatabaseSettings _settings;
    private string _name;
    private Dictionary<MagpieCollectionSettings, MagpieCollection> _collections = new Dictionary<MagpieCollectionSettings, MagpieCollection>();

    /// <summary>
    /// 数据库
    /// </summary>
    /// <param name="server">数据库所属服务器</param>
    /// <param name="settings">数据库配置</param>
    public MagpieDatabase(MagpieServer server, MagpieDatabaseSettings settings)
    {
      if (server == null)
      {
        throw new ArgumentNullException("server");
      }
      if (settings == null)
      {
        throw new ArgumentNullException("settings");
      }
      if (!server.IsDatabaseNameValid(settings.DatabaseName))
      {
        throw new ArgumentException("Invalid database name.");
      }

      _server = server;
      _settings = settings;
      _name = settings.DatabaseName;
    }

    /// <summary>
    /// 数据库名称
    /// </summary>
    public virtual string Name
    {
      get { return _name; }
    }

    /// <summary>
    /// 数据库所属服务器
    /// </summary>
    public virtual MagpieServer Server
    {
      get { return _server; }
    }

    /// <summary>
    /// 数据库配置
    /// </summary>
    public virtual MagpieDatabaseSettings Settings
    {
      get { return _settings; }
    }

    /// <summary>
    /// 是否给定的集合名称合法
    /// </summary>
    /// <param name="collectionName">集合名称</param>
    /// <returns>是否给定的集合名称合法</returns>
    public virtual bool IsCollectionNameValid(string collectionName)
    {
      if (string.IsNullOrEmpty(collectionName))
      {
        throw new ArgumentNullException("collectionName");
      }

      if (collectionName.IndexOf('\0') != -1)
      {
        return false;
      }

      if (Encoding.UTF8.GetBytes(collectionName).Length > 121)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// 获取指定名称的集合
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="collectionName">集合名称</param>
    /// <returns>集合</returns>
    public virtual MagpieCollection<TDocument> GetCollection<TDocument>(string collectionName) where TDocument : IMagpieDocumentId
    {
      var collectionSettings = new MagpieCollectionSettings<TDocument>(this, collectionName);
      return GetCollection(collectionSettings);
    }

    /// <summary>
    /// 获取指定配置的集合
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="collectionSettings">集合配置</param>
    /// <returns>集合</returns>
    protected virtual MagpieCollection<TDocument> GetCollection<TDocument>(MagpieCollectionSettings<TDocument> collectionSettings) where TDocument : IMagpieDocumentId
    {
      lock (_databaseLock)
      {
        MagpieCollection collection;
        if (!_collections.TryGetValue(collectionSettings, out collection))
        {
          collection = new MagpieCollection<TDocument>(this, collectionSettings);
          _collections.Add(collectionSettings, collection);
        }
        return (MagpieCollection<TDocument>)collection;
      }
    }

    /// <summary>
    /// 删除指定名称的集合
    /// </summary>
    /// <param name="collectionName">集合名称</param>
    public virtual void DropCollection(string collectionName)
    {
      lock (_databaseLock)
      {
        for (int i = 0; i < _collections.Keys.Count; i++)
        {
          var collectionSettings = _collections.Keys.ElementAt(i);
          if (collectionSettings.CollectionName == collectionName)
          {
            _collections.Remove(collectionSettings);

            File.Delete(collectionSettings.CollectionIndexFilePath);
            File.Delete(collectionSettings.CollectionDataFilePath);
          }
        }
      }
    }

    /// <summary>
    /// 删除数据库
    /// </summary>
    public virtual void Drop()
    {
      _server.DropDatabase(_name);
    }

    /// <summary>
    /// 关闭数据库
    /// </summary>
    internal virtual void Shutdown()
    {
      lock (_databaseLock)
      {
        foreach (var item in _collections)
        {
          item.Value.Shutdown();
        }
        _collections.Clear();
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
      return _name;
    }
  }
}
