using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Gimela.Data.Sparrow
{
  /// <summary>
  /// XML文件数据库
  /// </summary>
  public class XmlDatabase : FileDatabase
  {
    /// <summary>
    /// XML文件数据库
    /// </summary>
    /// <param name="directory">数据库文件所在目录</param>
    public XmlDatabase(string directory)
      : base(directory)
    {
      FileExtension = @"xml";
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

      using (StringWriterWithEncoding sw = new StringWriterWithEncoding(Encoding.UTF8))
      {
        XmlSerializer serializer = new XmlSerializer(value.GetType());
        serializer.Serialize(sw, value);
        return sw.ToString();
      }
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

      using (StringReader sr = new StringReader(data))
      {
        XmlSerializer serializer = new XmlSerializer(typeof(TDocument));
        return (TDocument)serializer.Deserialize(sr);
      }
    }
  }
}
