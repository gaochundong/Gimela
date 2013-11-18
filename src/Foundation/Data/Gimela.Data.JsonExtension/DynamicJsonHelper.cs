using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;

namespace Gimela.Data.JsonExtension
{
  internal static class DynamicJsonHelper
  {
    #region Serialize
    
    internal static string CreateJsonString(XStreamingElement element)
    {
      using (var ms = new MemoryStream())
      using (var writer = JsonReaderWriterFactory.CreateJsonWriter(ms, Encoding.Unicode))
      {
        element.WriteTo(writer);
        writer.Flush();
        return Encoding.Unicode.GetString(ms.ToArray());
      }
    }

    internal static XAttribute CreateTypeAttr(DynamicJsonType type)
    {
      return new XAttribute("type", type.ToString());
    }

    internal static object CreateJsonNode(object obj)
    {
      var type = GetDynamicJsonType(obj);
      switch (type)
      {
        case DynamicJsonType.@string:
        case DynamicJsonType.number:
          return obj;
        case DynamicJsonType.boolean:
          return obj.ToString().ToLower();
        case DynamicJsonType.@object:
          return CreateXObject(obj);
        case DynamicJsonType.array:
          return CreateXArray(obj as IEnumerable);
        case DynamicJsonType.@null:
        default:
          return null;
      }
    }

    internal static DynamicJsonType GetDynamicJsonType(object obj)
    {
      if (obj == null) return DynamicJsonType.@null;

      switch (Type.GetTypeCode(obj.GetType()))
      {
        case TypeCode.Boolean:
          return DynamicJsonType.boolean;
        case TypeCode.String:
        case TypeCode.Char:
        case TypeCode.DateTime:
          return DynamicJsonType.@string;
        case TypeCode.Int16:
        case TypeCode.Int32:
        case TypeCode.Int64:
        case TypeCode.UInt16:
        case TypeCode.UInt32:
        case TypeCode.UInt64:
        case TypeCode.Single:
        case TypeCode.Double:
        case TypeCode.Decimal:
        case TypeCode.SByte:
        case TypeCode.Byte:
          return DynamicJsonType.number;
        case TypeCode.Object:
          return (obj is IEnumerable) ? DynamicJsonType.array : DynamicJsonType.@object;
        case TypeCode.DBNull:
        case TypeCode.Empty:
        default:
          return DynamicJsonType.@null;
      }
    }

    private static IEnumerable<XStreamingElement> CreateXArray<T>(T obj) where T : IEnumerable
    {
      return obj.Cast<object>()
          .Select(o => new XStreamingElement("item", CreateTypeAttr(GetDynamicJsonType(o)), CreateJsonNode(o)));
    }

    private static IEnumerable<XStreamingElement> CreateXObject(object obj)
    {
      return obj.GetType()
          .GetProperties(BindingFlags.Public | BindingFlags.Instance)
          .Select(pi => new { Name = pi.Name, Value = pi.GetValue(obj, null) })
          .Select(a => new XStreamingElement(a.Name, CreateTypeAttr(GetDynamicJsonType(a.Value)), CreateJsonNode(a.Value)));
    }

    #endregion

    #region Deserialize

    internal static dynamic ConvertElementToValue(XElement element)
    {
      var type = (DynamicJsonType)Enum.Parse(typeof(DynamicJsonType), element.Attribute("type").Value);
      switch (type)
      {
        case DynamicJsonType.boolean:
          return (bool)element;
        case DynamicJsonType.number:
          return (double)element;
        case DynamicJsonType.@string:
          return (string)element;
        case DynamicJsonType.@object:
        case DynamicJsonType.array:
          return new DynamicJson(element, type);
        case DynamicJsonType.@null:
        default:
          return null;
      }
    }

    #endregion
  }
}
