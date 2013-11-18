using System;

namespace Gimela.Data.Json
{
  internal delegate void PropertySetter(object target, object value);
  internal delegate object PropertyGetter(object obj);

  internal struct Property
  {
    public Type Type;
    public string Name;

    public bool IsClass;
    public bool IsEnum;
    public bool IsValueType;
    public bool IsNullable;

    public bool IsByteArray;
    public bool IsGuid;
    public bool IsDateTime;
    public bool IsInt;
    public bool IsLong;
    public bool IsString;
    public bool IsBool;

    public bool IsGenericType;
    public bool IsArray;
    public bool IsDictionary;
    public bool IsStringDictionary;

    public Type[] GenericTypes;
    public Type ElementType;
    public Type ValueType;

    public PropertySetter Setter;
    public PropertyGetter Getter;
  }
}
