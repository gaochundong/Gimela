using System;
using System.Globalization;

namespace Gimela.Data.Magpie
{
  /// <summary>
  /// 集合配置
  /// </summary>
  public abstract class MagpieCollectionSettings
  {
    private MagpieDatabase _database;
    private string _collectionName;
    private Type _defaultDocumentType;

    /// <summary>
    /// 集合配置
    /// </summary>
    /// <param name="database">集合所属数据库</param>
    /// <param name="collectionName">集合名称</param>
    /// <param name="defaultDocumentType">默认的文档类型</param>
    protected MagpieCollectionSettings(MagpieDatabase database, string collectionName, Type defaultDocumentType)
    {
      if (database == null)
      {
        throw new ArgumentNullException("database");
      }
      if (collectionName == null)
      {
        throw new ArgumentNullException("collectionName");
      }
      if (defaultDocumentType == null)
      {
        throw new ArgumentNullException("defaultDocumentType");
      }

      _database = database;
      _collectionName = collectionName;
      _defaultDocumentType = defaultDocumentType;
    }

    /// <summary>
    /// 集合所属数据库
    /// </summary>
    public MagpieDatabase Database
    {
      get { return _database; }
    }

    /// <summary>
    /// 集合名称
    /// </summary>
    public string CollectionName
    {
      get { return _collectionName; }
    }

    /// <summary>
    /// 默认的文档类型
    /// </summary>
    public Type DefaultDocumentType
    {
      get { return _defaultDocumentType; }
    }

    /// <summary>
    /// 集合索引文件名称
    /// </summary>
    public string CollectionIndexFileName
    {
      get { return _collectionName + ".idx"; }
    }

    /// <summary>
    /// 集合数据文件名称
    /// </summary>
    public string CollectionDataFileName
    {
      get { return _collectionName + ".dat"; }
    }

    /// <summary>
    /// 集合索引文件路径
    /// </summary>
    public string CollectionIndexFilePath
    {
      get { return System.IO.Path.Combine(_database.Settings.Path, CollectionIndexFileName); }
    }

    /// <summary>
    /// 集合数据文件路径
    /// </summary>
    public string CollectionDataFilePath
    {
      get { return System.IO.Path.Combine(_database.Settings.Path, CollectionDataFileName); }
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
      var rhs = obj as MagpieCollectionSettings;
      if (rhs == null)
      {
        return false;
      }
      else
      {
        return
          _database.Settings == rhs._database.Settings &&
          _collectionName == rhs._collectionName &&
          _defaultDocumentType == rhs._defaultDocumentType;
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
      hash = 37 * hash + _database.Settings.GetHashCode();
      hash = 37 * hash + _collectionName.GetHashCode();
      hash = 37 * hash + _defaultDocumentType.GetHashCode();
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
      return string.Format(CultureInfo.InvariantCulture, "CollectionName={0};DefaultDocumentType={1};", _collectionName, _defaultDocumentType);
    }
  }

  /// <summary>
  /// 集合配置
  /// </summary>
  /// <typeparam name="TDefaultDocument">文档类型</typeparam>
  public class MagpieCollectionSettings<TDefaultDocument> : MagpieCollectionSettings
  {
    /// <summary>
    /// 集合配置
    /// </summary>
    /// <param name="database">集合所属数据库</param>
    /// <param name="collectionName">集合名称</param>
    public MagpieCollectionSettings(MagpieDatabase database, string collectionName)
      : base(database, collectionName, typeof(TDefaultDocument))
    {
    }
  }
}
