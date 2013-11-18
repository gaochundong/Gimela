using System;
using System.Collections.Generic;
using System.IO;
using Gimela.Data.DataStructures;

namespace Gimela.Data.Magpie
{
  /// <summary>
  /// 集合
  /// </summary>
  public abstract class MagpieCollection
  {
    private object _collectionLock = new object();
    private MagpieDatabase _database;
    private MagpieCollectionSettings _settings;
    private string _name;
    private IIndexTree _tree;

    /// <summary>
    /// 集合
    /// </summary>
    /// <param name="database">集合所属数据库</param>
    /// <param name="settings">集合配置</param>
    protected MagpieCollection(MagpieDatabase database, MagpieCollectionSettings settings)
    {
      if (database == null)
      {
        throw new ArgumentNullException("database");
      }
      if (settings == null)
      {
        throw new ArgumentNullException("settings");
      }
      if (!database.IsCollectionNameValid(settings.CollectionName))
      {
        throw new ArgumentException("Invalid collection name.");
      }

      _database = database;
      _settings = settings;
      _name = settings.CollectionName;
      _tree = GetIndexTree();
      _tree.Commit();
    }

    /// <summary>
    /// 集合所属数据库
    /// </summary>
    public virtual MagpieDatabase Database
    {
      get { return _database; }
    }

    /// <summary>
    /// 集合名称
    /// </summary>
    public virtual string Name
    {
      get { return _name; }
    }

    /// <summary>
    /// 集合配置
    /// </summary>
    public virtual MagpieCollectionSettings Settings
    {
      get { return _settings; }
    }

    /// <summary>
    /// 查找指定类型的所有文档
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <returns>指定类型的所有文档序列</returns>
    public virtual IEnumerable<TDocument> FindAll<TDocument>() where TDocument : IMagpieDocumentId
    {
      lock (_collectionLock)
      {
        string cursor = _tree.FirstKey();
        while (!string.IsNullOrEmpty(cursor))
        {
          yield return (TDocument)_tree.Get(cursor);
          cursor = _tree.NextKey(cursor);
        }
      }
    }

    /// <summary>
    /// 查找一个指定类型的文档
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <returns>指定类型的文档</returns>
    public virtual TDocument FindOne<TDocument>() where TDocument : IMagpieDocumentId
    {
      lock (_collectionLock)
      {
        string cursor = _tree.FirstKey();
        if (!string.IsNullOrEmpty(cursor))
        {
          return (TDocument)_tree.Get(cursor);
        }
        else
        {
          return default(TDocument);
        }
      }
    }

    /// <summary>
    /// 根据文档ID查找指定类型的文档
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="id">文档ID</param>
    /// <returns>指定类型的文档</returns>
    public virtual TDocument FindOneById<TDocument>(string id) where TDocument : IMagpieDocumentId
    {
      lock (_collectionLock)
      {
        if (_tree.ContainsKey(id))
        {
          return (TDocument)_tree.Get(id);
        }
        else
        {
          return default(TDocument);
        }
      }
    }

    /// <summary>
    /// 保存文档
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="document">文档对象</param>
    public virtual void Save<TDocument>(TDocument document) where TDocument : IMagpieDocumentId
    {
      lock (_collectionLock)
      {
        _tree.Set(document.Id, document);
        _tree.Commit();
      }
    }

    /// <summary>
    /// 批量保存文档
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="documents">文档对象序列</param>
    public virtual void SaveBatch<TDocument>(IEnumerable<TDocument> documents) where TDocument : IMagpieDocumentId
    {
      if (documents == null)
      {
        throw new ArgumentNullException("documents");
      }

      lock (_collectionLock)
      {
        foreach (var document in documents)
        {
          _tree.Set(document.Id, document);
        }
        _tree.Commit();
      }
    }

    /// <summary>
    /// 删除指定ID的文档
    /// </summary>
    /// <param name="id"></param>
    public virtual void Remove(string id)
    {
      lock (_collectionLock)
      {
        _tree.RemoveKey(id);
        _tree.Commit();
      }
    }

    /// <summary>
    /// 删除所有文档
    /// </summary>
    public virtual void RemoveAll()
    {
      lock (_collectionLock)
      {
        List<string> keys = new List<string>();
        string cursor = _tree.FirstKey();
        while (!string.IsNullOrEmpty(cursor))
        {
          keys.Add(cursor);
          cursor = _tree.NextKey(cursor);
        }
        foreach (var key in keys)
        {
          _tree.RemoveKey(key);
        }
        _tree.Commit();
      }
    }

    /// <summary>
    /// 获取文档的数量
    /// </summary>
    /// <returns>文档的数量</returns>
    public virtual int Count()
    {
      lock (_collectionLock)
      {
        int count = 0;

        string cursor = _tree.FirstKey();
        while (!string.IsNullOrEmpty(cursor))
        {
          count++;
          cursor = _tree.NextKey(cursor);
        }

        return count;
      }
    }

    /// <summary>
    /// 删除集合
    /// </summary>
    public virtual void Drop()
    {
      _database.DropCollection(_name);
    }

    /// <summary>
    /// 关闭集合
    /// </summary>
    internal virtual void Shutdown()
    {
      lock (_collectionLock)
      {
        _tree.Shutdown();
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
      return _database.Name + "." + _name;
    }

    private IIndexTree GetIndexTree()
    {
      IIndexTree tree;

      if (File.Exists(_settings.CollectionIndexFilePath)
        && File.Exists(_settings.CollectionDataFilePath))
      {
        tree = BPlusTreeObject.Open(_settings.CollectionIndexFilePath, _settings.CollectionDataFilePath);
      }
      else if (File.Exists(_settings.CollectionIndexFilePath)
        && !File.Exists(_settings.CollectionDataFilePath))
      {
        throw new MagpieException("Data file missing.");
      }
      else if (!File.Exists(_settings.CollectionIndexFilePath)
        && File.Exists(_settings.CollectionDataFilePath))
      {
        throw new MagpieException("Index file missing.");
      }
      else
      {
        tree = BPlusTreeObject.Create(_settings.CollectionIndexFilePath, _settings.CollectionDataFilePath, MagpieDefaults.DocumentIdLength);
      }

      return tree;
    }
  }

  /// <summary>
  /// 集合
  /// </summary>
  /// <typeparam name="TDocument">文档类型</typeparam>
  public class MagpieCollection<TDocument> : MagpieCollection where TDocument : IMagpieDocumentId
  {
    /// <summary>
    /// 集合
    /// </summary>
    /// <param name="database">集合所属数据库</param>
    /// <param name="settings">集合配置</param>
    public MagpieCollection(MagpieDatabase database, MagpieCollectionSettings<TDocument> settings)
      : base(database, settings)
    {
    }

    /// <summary>
    /// 获取所有文档
    /// </summary>
    /// <returns>所有文档序列</returns>
    public virtual IEnumerable<TDocument> FindAll()
    {
      return FindAll<TDocument>();
    }

    /// <summary>
    /// 获取一个文档
    /// </summary>
    /// <returns>文档</returns>
    public virtual TDocument FindOne()
    {
      return FindOne<TDocument>();
    }

    /// <summary>
    /// 获取指定ID的文档
    /// </summary>
    /// <param name="id">文档ID</param>
    /// <returns>文档</returns>
    public virtual TDocument FindOneById(string id)
    {
      return FindOneById<TDocument>(id);
    }

    /// <summary>
    /// 保存文档
    /// </summary>
    /// <param name="document">文档对象</param>
    public virtual void Save(TDocument document)
    {
      Save<TDocument>(document);
    }

    /// <summary>
    /// 批量保存文档
    /// </summary>
    /// <param name="documents">文档对象序列</param>
    public virtual void SaveBatch(IEnumerable<TDocument> documents)
    {
      SaveBatch<TDocument>(documents);
    }
  }
}
