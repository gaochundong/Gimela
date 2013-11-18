using System.Collections.Generic;

namespace Gimela.Data.Json
{
  /// <summary>
  /// JSON (JavaScript Object Notation) is a lightweight data-interchange format. 
  /// It is easy for humans to read and write. It is easy for machines to parse and generate. 
  /// It is based on a subset of the JavaScript Programming Language, Standard ECMA-262 3rd Edition - December 1999. 
  /// </summary>
  public static class JsonConvert
  {
    /// <summary>
    /// 序列化指定的对象至JSON字符串
    /// </summary>
    /// <param name="target">指定的对象</param>
    /// <returns>JSON字符串</returns>
    public static string SerializeObject(object target)
    {
      return SerializeObject(target, true);
    }

    /// <summary>
    /// 序列化指定的对象至JSON字符串
    /// </summary>
    /// <param name="target">指定的对象</param>
    /// <param name="isOutputIndented">是否输出缩进</param>
    /// <returns>JSON字符串</returns>
    public static string SerializeObject(object target, bool isOutputIndented)
    {
      return SerializeObject(target, isOutputIndented, false, false);
    }

    /// <summary>
    /// 序列化指定的对象至JSON字符串
    /// </summary>
    /// <param name="target">指定的对象</param>
    /// <param name="isOutputIndented">是否输出缩进</param>
    /// <param name="isSerializeNullValue">是否序列化NULL值的属性</param>
    /// <param name="isSerializeExtension">是否序列化扩展内容</param>
    /// <returns>JSON字符串</returns>
    public static string SerializeObject(object target, bool isOutputIndented, bool isSerializeNullValue, bool isSerializeExtension)
    {
      return new JsonSerializer()
      {
        IsOutputIndented = isOutputIndented,
        IsSerializeNullValue = isSerializeNullValue,
        IsSerializeExtension = isSerializeExtension,
      }
      .Serialize(target);
    }

    /// <summary>
    /// 反序列化JSON字符串至指定类型的对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="json">JSON字符串</param>
    /// <returns>指定类型的对象</returns>
    public static T DeserializeObject<T>(string json)
    {
      Dictionary<string, object> propertyPairs = JsonParser.Decode(json);
      return (T)JsonDeserializer.ParseProperties(propertyPairs, typeof(T));
    }
  }
}