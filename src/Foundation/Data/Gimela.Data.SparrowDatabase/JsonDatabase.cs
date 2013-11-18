using System;
using Gimela.Data.Json;

namespace Gimela.Data.Sparrow
{
  /// <summary>
  /// JSON文件数据库
  /// </summary>
  public class JsonDatabase : FileDatabase
  {
    /// <summary>
    /// JSON文件数据库
    /// </summary>
    /// <param name="directory">数据库文件所在目录</param>
    public JsonDatabase(string directory)
      : base(directory)
    {
      FileExtension = @"json";
    }

    /// <summary>
    /// 将指定的文档对象序列化至字符串
    /// </summary>
    /// <param name="value">指定的文档对象</param>
    /// <returns>
    /// 文档对象序列化后的字符串
    /// </returns>
    protected override string Serialize(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");

      return JsonConvert.SerializeObject(value, OutputIndent);
    }

    /// <summary>
    /// 将字符串反序列化成文档对象
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="data">字符串</param>
    /// <returns>
    /// 文档对象
    /// </returns>
    protected override TDocument Deserialize<TDocument>(string data)
    {
      if (string.IsNullOrEmpty(data))
        throw new ArgumentNullException("data");

      return JsonConvert.DeserializeObject<TDocument>(data);
    }
  }
}
