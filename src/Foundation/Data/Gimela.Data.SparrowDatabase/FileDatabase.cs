using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Gimela.Data.Json;

namespace Gimela.Data.Sparrow
{
  /// <summary>
  /// 文件数据库，这是一个抽象类。
  /// </summary>
  public abstract class FileDatabase
  {
    #region Fields

    /// <summary>
    /// 文件数据库操作锁
    /// </summary>
    protected static readonly object operationLock = new object();
    private static HashSet<char> invalidFileNameChars;

    static FileDatabase()
    {
      invalidFileNameChars = new HashSet<char>() { '\0', ' ', '.', '$', '/', '\\' };
      foreach (var c in Path.GetInvalidPathChars()) { invalidFileNameChars.Add(c); }
      foreach (var c in Path.GetInvalidFileNameChars()) { invalidFileNameChars.Add(c); }
    }

    /// <summary>
    /// 文件数据库
    /// </summary>
    /// <param name="directory">数据库文件所在目录</param>
    protected FileDatabase(string directory)
    {
      Directory = directory;
    }

    #endregion

    #region Properties

    /// <summary>
    /// 数据库文件所在目录
    /// </summary>
    public virtual string Directory { get; private set; }

    /// <summary>
    /// 是否输出缩进
    /// </summary>
    public virtual bool OutputIndent { get; set; }

    /// <summary>
    /// 文件扩展名
    /// </summary>
    public virtual string FileExtension { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// 保存文档
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="document">文档对象</param>
    /// <returns>文档ID</returns>
    public virtual string Save<TDocument>(TDocument document)
    {
      return Save<TDocument>(ObjectId.NewObjectId().ToString(), document);
    }

    /// <summary>
    /// 保存文档
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="id">文档ID</param>
    /// <param name="document">文档对象</param>
    /// <returns>文档ID</returns>
    public virtual string Save<TDocument>(string id, TDocument document)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException("id");

      if (document == null)
        throw new ArgumentNullException("document");

      Delete<TDocument>(id);

      try
      {
        string fileName = GenerateFileFullPath<TDocument>(id);
        string output = Serialize(document);

        lock (operationLock)
        {
          System.IO.FileInfo info = new System.IO.FileInfo(fileName);
          System.IO.Directory.CreateDirectory(info.Directory.FullName);
          System.IO.File.WriteAllText(fileName, output);
        }
      }
      catch (Exception ex)
      {
        throw new FileDatabaseException(
          string.Format(CultureInfo.InvariantCulture, 
          "Save document failed with id [{0}].", id), ex);
      }

      return id;
    }

    /// <summary>
    /// 根据文档ID查找文档
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="id">文档ID</param>
    /// <returns>文档对象</returns>
    public virtual TDocument FindOneById<TDocument>(string id)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException("id");

      try
      {
        string fileName = GenerateFileFullPath<TDocument>(id);
        if (File.Exists(fileName))
        {
          string fileData = File.ReadAllText(fileName);
          return Deserialize<TDocument>(fileData);
        }

        return default(TDocument);
      }
      catch (Exception ex)
      {
        throw new FileDatabaseException(
          string.Format(CultureInfo.InvariantCulture, 
          "Find document by id [{0}] failed.", id), ex);
      }
    }

    /// <summary>
    /// 查找指定类型的所有文档
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <returns>文档对象序列</returns>
    public virtual IEnumerable<TDocument> FindAll<TDocument>()
    {
      try
      {
        string[] files = System.IO.Directory.GetFiles(
          GenerateFilePath<TDocument>(), 
          "*." + FileExtension, 
          SearchOption.TopDirectoryOnly);

        List<TDocument> list = new List<TDocument>();
        foreach (string fileName in files)
        {
          string fileData = File.ReadAllText(fileName);
          TDocument document = Deserialize<TDocument>(fileData);
          if (document != null)
          {
            list.Add(document);
          }
        }

        return list;
      }
      catch (Exception ex)
      {
        throw new FileDatabaseException(
          "Find all documents failed.", ex);
      }
    }

    /// <summary>
    /// 根据指定文档ID删除文档
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="id">文档ID</param>
    public virtual void Delete<TDocument>(string id)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException("id");

      try
      {
        string fileName = GenerateFileFullPath<TDocument>(id);
        if (File.Exists(fileName))
        {
          lock (operationLock)
          {
            File.Delete(fileName);
          }
        }
      }
      catch (Exception ex)
      {
        throw new FileDatabaseException(
          string.Format(CultureInfo.InvariantCulture, 
          "Delete document by id [{0}] failed.", id), ex);
      }
    }

    /// <summary>
    /// 删除所有指定类型的文档
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    public virtual void DeleteAll<TDocument>()
    {
      try
      {
        string[] files = System.IO.Directory.GetFiles(
          GenerateFilePath<TDocument>(), "*." + FileExtension, 
          SearchOption.TopDirectoryOnly);

        foreach (string fileName in files)
        {
          lock (operationLock)
          {
            File.Delete(fileName);
          }
        }
      }
      catch (Exception ex)
      {
        throw new FileDatabaseException(
          "Delete all documents failed.", ex);
      }
    }

    /// <summary>
    /// 获取指定类型文档的数量
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <returns>文档的数量</returns>
    public virtual int Count<TDocument>()
    {
      try
      {
        string[] files = System.IO.Directory.GetFiles(
          GenerateFilePath<TDocument>(), 
          "*." + FileExtension, SearchOption.TopDirectoryOnly);
        if (files != null)
        {
          return files.Length;
        }
        else
        {
          return 0;
        }
      }
      catch (Exception ex)
      {
        throw new FileDatabaseException(
          "Count all documents failed.", ex);
      }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// 生成文件全路径
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="id">文档ID</param>
    /// <returns>文件路径</returns>
    protected virtual string GenerateFileFullPath<TDocument>(string id)
    {
      return Path.Combine(GenerateFilePath<TDocument>(), 
        GenerateFileName<TDocument>(id));
    }

    /// <summary>
    /// 生成文件路径
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <returns>文件路径</returns>
    protected virtual string GenerateFilePath<TDocument>()
    {
      return Path.Combine(this.Directory, typeof(TDocument).Name);
    }

    /// <summary>
    /// 生成文件名
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="id">文档ID</param>
    /// <returns>文件名</returns>
    protected virtual string GenerateFileName<TDocument>(string id)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException("id");

      foreach (char c in id)
      {
        if (invalidFileNameChars.Contains(c))
        {
          throw new FileDatabaseException(
            string.Format(CultureInfo.InvariantCulture, 
            "The character '{0}' is not a valid file name identifier.", c));
        }
      }

      return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", id, FileExtension);
    }

    /// <summary>
    /// 将指定的文档对象序列化至字符串
    /// </summary>
    /// <param name="value">指定的文档对象</param>
    /// <returns>文档对象序列化后的字符串</returns>
    protected abstract string Serialize(object value);

    /// <summary>
    /// 将字符串反序列化成文档对象
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="data">字符串</param>
    /// <returns>文档对象</returns>
    protected abstract TDocument Deserialize<TDocument>(string data);

    #endregion
  }
}
