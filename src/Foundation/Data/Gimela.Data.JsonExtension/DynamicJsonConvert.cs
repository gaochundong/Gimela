using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Gimela.Data.JsonExtension
{
  /// <summary>
  /// DynamicJson转换器
  /// </summary>
  public static class DynamicJsonConvert
  {
    /// <summary>
    /// 序列化指定的对象至JSON字符串
    /// </summary>
    /// <param name="target">指定的对象</param>
    /// <returns>JSON字符串</returns>
    public static string SerializeObject(object target)
    {
      return DynamicJsonHelper.CreateJsonString(
        new XStreamingElement("root",
          DynamicJsonHelper.CreateTypeAttr(DynamicJsonHelper.GetDynamicJsonType(target)),
          DynamicJsonHelper.CreateJsonNode(target)));
    }

    /// <summary>
    /// Convert JSON string to DynamicJson
    /// </summary>
    /// <param name="json">JSON字符串</param>
    /// <returns>DynamicJson</returns>
    public static dynamic Parse(string json)
    {
      using (var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.Unicode.GetBytes(json), XmlDictionaryReaderQuotas.Max))
      {
        return DynamicJsonHelper.ConvertElementToValue(XElement.Load(reader));
      }
    }
  }
}
