using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Gimela.Common.ExceptionHandling;

namespace Gimela.Infrastructure.Serialization
{
  /// <summary>
  /// 文件序列化器
  /// </summary>
  public static class FileSerializer
  {
    /// <summary>
    /// Deserialize an object from xml string.
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="xmlData">Xml data to deserialze to an object.</param>
    /// <returns>Serialized string for the item</returns>
    [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads")]
    public static T LoadItem<T>(string xmlData)
    {
      T target = default(T);

      using (StringReader reader = new StringReader(xmlData))
      {
        target = LoadItem<T>(reader);
      }

      return target;
    }

    /// <summary>
    /// Deserialize an object from a file
    /// </summary>
    /// <typeparam name="T">Data type of object</typeparam>
    /// <param name="uri">Full path to file</param>
    /// <returns>New instance of the desired object</returns>
    public static T LoadItem<T>(Uri uri)
    {
      if (uri == null)
        throw new ArgumentNullException("uri");

      T target = default(T);

      using (FileStream fs = new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        target = LoadItem<T>(fs);
      }

      return target;
    }

    /// <summary>
    /// Deserialize an object from a stream
    /// </summary>
    /// <typeparam name="T">Data type of object</typeparam>
    /// <param name="fileStream">Stream to read object from</param>
    /// <returns>New instance of the desired object</returns>
    public static T LoadItem<T>(Stream fileStream)
    {
      XmlSerializer xs = new XmlSerializer(typeof(T));
      return (T)xs.Deserialize(fileStream);
    }

    /// <summary>
    /// Deserialize an object from a TextReader
    /// </summary>
    /// <typeparam name="T">Data type of object</typeparam>
    /// <param name="reader">TextReader to deserialize from</param>
    /// <returns>New instance of the desired object</returns>
    public static T LoadItem<T>(TextReader reader)
    {
      XmlSerializer xs = new XmlSerializer(typeof(T));
      return (T)xs.Deserialize(reader);
    }

    /// <summary>
    /// 将对象序列化为给定XML文件。
    /// </summary>
    /// <typeparam name="T">指定类型</typeparam>
    /// <param name="target">对象实例</param>
    /// <param name="file">XML文件路径名称</param>
    public static void SerializeToXmlFile<T>(T target, string file)
    {
      try
      {
        XmlSerializer xs = new XmlSerializer(typeof(T));
        using (StreamWriter sw = new StreamWriter(file))
        {
          xs.Serialize(sw, target);
        }
      }
      catch (InvalidOperationException ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }

    /// <summary>
    /// 将XML文件反序列化为对象。
    /// </summary>
    /// <typeparam name="T">指定对象的类型</typeparam>
    /// <param name="file">XML文件路径名称</param>
    /// <returns>对象</returns>
    public static T DeserializeFromXmlFile<T>(string file)
    {
      T target = default(T);

      try
      {
        // Beleive it or not, this is normal behaviour. An exception is thrown but handled by the XmlSerializer, 
        // so if you just ignore it everything should continue on fine.
        // I have found this very anoying, there have been many complaints about this if you search around 
        // a bit but from what I've read Microsoft don't plan on doing anything about it.
        // You can avoid getting Exception popups all the time while debugging if you switch off first 
        // chance exceptions for that specific exception. In visual studio go to 
        // Debug -> Exceptions (or press CTRL+ALT+E), 
        // Common Language Runtime Exceptions -> System.IO -> System.IO.FileNotFoundException.
        // You can find information about another way around here:
        // http://stackoverflow.com/questions/1127431/xmlserializer-giving-filenotfoundexception-at-constructor
        // http://bloggingabout.net/blogs/rick/archive/2005/03/01/2333.aspx

        XmlSerializer xs = new XmlSerializer(typeof(T));
        using (StreamReader sr = new StreamReader(file))
        {
          object o = xs.Deserialize(sr);
          if (o is T)
          {
            target = (T)o;
          }
        }
      }
      catch (InvalidOperationException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (FileLoadException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (FileNotFoundException ex)
      {
        ExceptionHandler.Handle(ex);
      }

      return target;
    }

    /// <summary>
    /// 将对象序列化为XML字符串
    /// </summary>
    /// <typeparam name="T">指定类型</typeparam>
    /// <param name="target">对象实例</param>
    /// <returns>XML字符串</returns>
    public static string SerializeToXmlString<T>(T target)
    {
      string str = null;
      MemoryStream ms = null;

      try
      {
        XmlSerializer xs = new XmlSerializer(typeof(T));

        ms = new MemoryStream(); // stream to which you want to write.
        using (XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8)
        {
          Formatting = System.Xml.Formatting.Indented
        })
        {
          ms = null;
          xs.Serialize(xtw, target);
          xtw.BaseStream.Seek(0, SeekOrigin.Begin);
          using (StreamReader sr = new StreamReader(xtw.BaseStream))
          {
            str = sr.ReadToEnd();
          }
        }
      }
      catch (InvalidOperationException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      finally
      {
        if (ms != null)
        {
          ms.Close();
          ms = null;
        }
      }

      return str;
    }

    /// <summary>
    /// 将XML字符串反序列化为对象
    /// </summary>
    /// <typeparam name="T">指定对象的类型</typeparam>
    /// <param name="xml">XML字符串</param>
    /// <returns>T 对象</returns>
    public static T DeserializeFromXmlString<T>(string xml)
    {
      T target = default(T);
      StringReader sr = null;

      try
      {
        XmlSerializer xs = new XmlSerializer(typeof(T));

        sr = new StringReader(xml); // containing the XML data to read
        using (XmlTextReader xtr = new XmlTextReader(sr))
        {
          sr = null;
          object o = xs.Deserialize(xtr);
          if (o is T)
          {
            target = (T)o;
          }
        }
      }
      catch (InvalidOperationException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      finally
      {
        if (sr != null)
        {
          sr.Close();
        }
      }

      return target;
    }

    /// <summary>
    /// XmlSerializerClone is typically used when copies of an Xml serializable object are needed to prevent modification
    /// </summary>
    /// <typeparam name="T">Data type of object being cloned</typeparam>
    /// <param name="original">Original object being cloned</param>
    /// <returns>Duplicate copy of original object</returns>
    public static T XmlSerializerClone<T>(T original)
    {
      T target = default(T);

      XmlSerializer xs = new XmlSerializer(typeof(T));
      using (MemoryStream ms = new MemoryStream())
      {
        xs.Serialize(ms, original);
        ms.Seek(0, SeekOrigin.Begin);
        target = (T)xs.Deserialize(ms);
      }

      return target;
    }
  }
}
