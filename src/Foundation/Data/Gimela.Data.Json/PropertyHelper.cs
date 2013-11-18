using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Gimela.Data.Json
{
  internal static class PropertyHelper
  {
    internal static IDictionary<string, PropertyGetter> GetTypePropertyGetters(Type type)
    {
      IDictionary<string, PropertyGetter> getters = new Dictionary<string, PropertyGetter>();

      PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
      foreach (PropertyInfo p in props)
      {
        if (!p.CanWrite) continue;

        object[] att = p.GetCustomAttributes(typeof(JsonIgnoreAttribute), false);
        if (att != null && att.Length > 0)
          continue;

        PropertyGetter g = CreatePeropertyGetter(p);
        if (g != null)
        {
          getters.Add(p.Name, g);
        }
      }

      return getters;
    }

    internal static IDictionary<string, Property> GetTypeProperties(Type type)
    {
      Dictionary<string, Property> dict = new Dictionary<string, Property>();

      PropertyInfo[] pr = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
      foreach (var info in pr)
      {
        Property property = CreateProperty(info.PropertyType, info.Name);
        property.Setter = CreatePropertySetter(info);
        property.Getter = CreatePeropertyGetter(info);
        dict.Add(info.Name, property);
      }

      return dict;
    }

    internal static Property ConvertTypeToProperty(Type type)
    {
      Property property = new Property();

      property.Type = type;
      property.Name = type.Name;

      property.IsClass = type.IsClass;
      property.IsEnum = type.IsEnum;

      property.IsByteArray = type == typeof(byte[]);
      property.IsGuid = (type == typeof(Guid) || type == typeof(Guid?));
      property.IsDateTime = type == typeof(DateTime) || type == typeof(DateTime?);
      property.IsInt = type == typeof(int) || type == typeof(int?);
      property.IsLong = type == typeof(long) || type == typeof(long?);
      property.IsString = type == typeof(string);
      property.IsBool = type == typeof(bool) || type == typeof(bool?);

      property.IsGenericType = type.IsGenericType;
      if (property.IsGenericType)
      {
        property.ElementType = type.GetGenericArguments()[0];
      }

      property.IsArray = type.IsArray;
      if (property.IsArray)
      {
        property.ElementType = type.GetElementType();
      }

      property.IsDictionary = type.Name.Contains("Dictionary");
      if (property.IsDictionary)
      {
        property.GenericTypes = type.GetGenericArguments();
      }

      if (property.IsDictionary
        && property.GenericTypes[0] == typeof(string)
        && property.GenericTypes[1] == typeof(string))
      {
        property.IsStringDictionary = true;
      }

      property.IsValueType = type.IsValueType;
      if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
      {
        property.IsNullable = true;
        property.ValueType = type.GetGenericArguments()[0];
      }
      else
      {
        property.ValueType = type;
      }

      return property;
    }

    private static Property CreateProperty(Type propertyType, string propertyName)
    {
      Property property = new Property();

      property.Type = propertyType;
      property.Name = propertyName;

      property.IsClass = propertyType.IsClass;
      property.IsEnum = propertyType.IsEnum;
      
      property.IsByteArray = propertyType == typeof(byte[]);
      property.IsGuid = (propertyType == typeof(Guid) || propertyType == typeof(Guid?));
      property.IsDateTime = propertyType == typeof(DateTime) || propertyType == typeof(DateTime?);
      property.IsInt = propertyType == typeof(int) || propertyType == typeof(int?);
      property.IsLong = propertyType == typeof(long) || propertyType == typeof(long?);
      property.IsString = propertyType == typeof(string);
      property.IsBool = propertyType == typeof(bool) || propertyType == typeof(bool?);

      property.IsGenericType = propertyType.IsGenericType;
      if (property.IsGenericType)
      {
        property.ElementType = propertyType.GetGenericArguments()[0];
      }

      property.IsArray = propertyType.IsArray;
      if (property.IsArray)
      {
        property.ElementType = propertyType.GetElementType();
      }

      property.IsDictionary = propertyType.Name.Contains("Dictionary");
      if (property.IsDictionary)
      {
        property.GenericTypes = propertyType.GetGenericArguments();
      }

      if (property.IsDictionary
        && property.GenericTypes[0] == typeof(string)
        && property.GenericTypes[1] == typeof(string))
      {
        property.IsStringDictionary = true;
      }

      property.IsValueType = propertyType.IsValueType;
      if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
      {
        property.IsNullable = true;
        property.ValueType = propertyType.GetGenericArguments()[0];
      }
      else
      {
        property.ValueType = propertyType;
      }

      return property;
    }

    private static PropertySetter CreatePropertySetter(PropertyInfo propertyInfo)
    {
      MethodInfo setMethod = propertyInfo.GetSetMethod();
      if (setMethod == null)
        return null;

      Type[] arguments = new Type[2];
      arguments[0] = arguments[1] = typeof(object);

      DynamicMethod setter = new DynamicMethod("_", typeof(void), arguments);
      ILGenerator il = setter.GetILGenerator();
      il.Emit(OpCodes.Ldarg_0);
      il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
      il.Emit(OpCodes.Ldarg_1);

      if (propertyInfo.PropertyType.IsClass)
        il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
      else
        il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);

      il.EmitCall(OpCodes.Callvirt, setMethod, null);
      il.Emit(OpCodes.Ret);

      return (PropertySetter)setter.CreateDelegate(typeof(PropertySetter));
    }

    private static PropertyGetter CreatePeropertyGetter(PropertyInfo propertyInfo)
    {
      MethodInfo getMethod = propertyInfo.GetGetMethod();
      if (getMethod == null)
        return null;

      Type[] arguments = new Type[1];
      arguments[0] = typeof(object);

      DynamicMethod getter = new DynamicMethod("_", typeof(object), arguments);
      ILGenerator il = getter.GetILGenerator();
      il.Emit(OpCodes.Ldarg_0);
      il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
      il.EmitCall(OpCodes.Callvirt, getMethod, null);

      if (!propertyInfo.PropertyType.IsClass)
        il.Emit(OpCodes.Box, propertyInfo.PropertyType);

      il.Emit(OpCodes.Ret);

      return (PropertyGetter)getter.CreateDelegate(typeof(PropertyGetter));
    }
  }
}
