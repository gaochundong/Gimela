using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Gimela.Data.Json
{
  internal static class JsonDeserializer
  {
    public static object ParseProperties(Dictionary<string, object> propertyPairs, Type type)
    {
      if (type == null)
        throw new JsonSerializationException("Cannot determine type.");

      object instance = Activator.CreateInstance(type);

      IDictionary<string, Property> typeProperties = PropertyHelper.GetTypeProperties(type);
      foreach (string propertyName in propertyPairs.Keys)
      {
        Property property;
        if (typeProperties.TryGetValue(propertyName, out property) == false)
        {
          continue;
        }

        object propertyValue = propertyPairs[propertyName];
        if (propertyValue == null)
        {
          continue;
        }

        object setValue = null;
        if (property.IsEnum)
        {
          setValue = CreateEnum(property.Type, (string)propertyValue);
        }
        else if (property.IsByteArray)
        {
          setValue = CreateByteArray((string)propertyValue);
        }
        else if (property.IsGuid)
        {
          setValue = CreateGuid((string)propertyValue);
        }
        else if (property.IsDateTime)
        {
          setValue = CreateDateTime((string)propertyValue);
        }
        else if (property.IsInt)
        {
          setValue = (int)CreateLong((string)propertyValue);
        }
        else if (property.IsLong)
        {
          setValue = CreateLong((string)propertyValue);
        }
        else if (property.IsString)
        {
          setValue = (string)propertyValue;
        }
        else if (property.IsBool)
        {
          setValue = (bool)propertyValue;
        }
        else if (property.IsValueType)
        {
          setValue = CreateValueType(propertyValue, property.ValueType);
        }
        else if (property.IsClass && property.IsArray && property.IsValueType == false)
        {
          setValue = CreateArray((ArrayList)propertyValue, property.Type, property.ElementType);
        }
        else if (property.IsClass && property.IsStringDictionary)
        {
          setValue = CreateStringDictionary((Dictionary<string, object>)propertyValue, property.Type, property.GenericTypes);
        }
        else if (property.IsClass && property.IsGenericType && property.IsValueType == false && property.IsDictionary == false)
        {
          setValue = CreateList((ArrayList)propertyValue, property.Type, property.ElementType);
        }
        else if (property.IsClass && property.IsGenericType && property.IsDictionary)
        {
          setValue = CreateStringDictionary((Dictionary<string, object>)propertyValue, property.Type, property.GenericTypes);
        }
        else if (property.IsClass && propertyValue is Dictionary<string, object>)
        {
          setValue = ParseProperties((Dictionary<string, object>)propertyValue, property.Type);
        }
        else
        {
          setValue = propertyValue;
        }

        property.Setter(instance, setValue);
      }

      return instance;
    }

    #region Value Creater

    private static object CreateArray(ArrayList data, Type propertyType, Type elementType)
    {
      ArrayList arr = new ArrayList();

      foreach (object ob in data)
      {
        if (ob is IDictionary)
          arr.Add(ParseProperties((Dictionary<string, object>)ob, elementType));
        else
          arr.Add(CreateValueType(ob, elementType));
      }

      return arr.ToArray(elementType);
    }

    private static object CreateList(ArrayList data, Type propertyType, Type elementType)
    {
      IList list = (IList)Activator.CreateInstance(propertyType);

      foreach (object item in data)
      {
        if (item is IDictionary)
          list.Add(ParseProperties((Dictionary<string, object>)item, elementType));

        else if (item is ArrayList)
          list.Add(((ArrayList)item).ToArray());

        else
          list.Add(CreateValueType(item, elementType));
      }

      return list;
    }

    private static object CreateStringDictionary(Dictionary<string, object> reader, Type propertyType, Type[] types)
    {
      var dict = (IDictionary)Activator.CreateInstance(propertyType);

      Type t1 = null;
      Type t2 = null;
      if (types != null)
      {
        t1 = types[0];
        t2 = types[1];
      }

      foreach (KeyValuePair<string, object> item in reader)
      {
        var key = item.Key;
        var val = item.Value;
        if (item.Value is Dictionary<string, object>)
          val = ParseProperties((Dictionary<string, object>)val, t2);
        else
          val = CreateValueType(val, t2);

        dict.Add(key, val);
      }

      return dict;
    }

    private static object CreateEnum(Type propertyType, string propertyValue)
    {
      return Enum.Parse(propertyType, propertyValue);
    }

    private static byte[] CreateByteArray(string propertyValue)
    {
      return Convert.FromBase64String(propertyValue);
    }

    private static Guid CreateGuid(string propertyValue)
    {
      if (propertyValue.Length > 30)
        return new Guid(propertyValue);
      else
        return new Guid(Convert.FromBase64String(propertyValue));
    }

    private static DateTime CreateDateTime(string propertyValue)
    {
      // "/Date(1338629604392+0800)/"
      Regex r = new Regex(@"^/Date\((\d+)\+(\d+)\)/$", RegexOptions.IgnoreCase);
      Match m = r.Match(propertyValue);
      if (m.Success)
      {
        return DateTimeHelper.ConvertJavaScriptTicksToDateTime(long.Parse(m.Groups[1].ToString(), CultureInfo.InvariantCulture)).ToLocalTime();
      }

      throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Cannot convert value {0} to date time.", propertyValue));
    }

    private static long CreateLong(string propertyValue)
    {
      long num = 0;
      bool neg = false;
      foreach (char cc in propertyValue)
      {
        if (cc == '-')
          neg = true;
        else if (cc == '+')
          neg = false;
        else
        {
          num *= 10;
          num += (int)(cc - '0');
        }
      }

      return neg ? -num : num;
    }

    private static object CreateValueType(object value, Type conversionType)
    {
      if (conversionType == typeof(int))
        return (int)CreateLong((string)value);
      else if (conversionType == typeof(long))
        return CreateLong((string)value);
      else if (conversionType == typeof(string))
        return (string)value;
      else if (conversionType == typeof(Guid))
        return CreateGuid((string)value);
      else
        return Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
    }

    #endregion
  }
}
