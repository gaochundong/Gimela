using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Gimela.Data.JsonExtension
{
  /// <summary>
  /// Dynamic JSON
  /// </summary>
  public class DynamicJson : DynamicObject
  {
    #region Fields
    
    readonly XElement xml;
    readonly DynamicJsonType jsonType;

    #endregion

    #region Ctors

    /// <summary>
    /// Dynamic JSON
    /// </summary>
    public DynamicJson()
    {
      xml = new XElement("root", DynamicJsonHelper.CreateTypeAttr(DynamicJsonType.@object));
      jsonType = DynamicJsonType.@object;
    }

    internal DynamicJson(XElement element, DynamicJsonType type)
    {
      Debug.Assert(type == DynamicJsonType.array || type == DynamicJsonType.@object);

      xml = element;
      jsonType = type;
    }

    #endregion

    #region Property Methods

    /// <summary>
    /// Gets a value indicating whether this instance is object.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is object; otherwise, <c>false</c>.
    /// </value>
    public bool IsObject { get { return jsonType == DynamicJsonType.@object; } }

    /// <summary>
    /// Gets a value indicating whether this instance is array.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is array; otherwise, <c>false</c>.
    /// </value>
    public bool IsArray { get { return jsonType == DynamicJsonType.array; } }

    /// <summary>
    /// 是否定义了指定名称的属性
    /// </summary>
    public bool IsDefined(string name)
    {
      return IsObject && (xml.Element(name) != null);
    }

    /// <summary>
    /// 是否定义了指定索引的属性
    /// </summary>
    public bool IsDefined(int index)
    {
      return IsArray && (xml.Elements().ElementAtOrDefault(index) != null);
    }

    /// <summary>
    /// 删除属性
    /// </summary>
    public bool Delete(string name)
    {
      var elem = xml.Element(name);
      if (elem != null)
      {
        elem.Remove();
        return true;
      }
      else return false;
    }

    /// <summary>
    /// 删除属性
    /// </summary>
    public bool Delete(int index)
    {
      var elem = xml.Elements().ElementAtOrDefault(index);
      if (elem != null)
      {
        elem.Remove();
        return true;
      }
      else return false;
    }

    #endregion

    #region Serialize

    /// <summary>
    /// 序列化至字符串
    /// </summary>
    /// <returns>字符串</returns>
    public string Serialize()
    {
      return this.ToString();
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      // Serialize to JsonString
      // <foo type="null"></foo> is can't serialize. replace to <foo type="null" />
      foreach (var elem in xml.Descendants().Where(x => x.Attribute("type").Value == "null"))
      {
        elem.RemoveNodes();
      }
      return DynamicJsonHelper.CreateJsonString(new XStreamingElement("root", DynamicJsonHelper.CreateTypeAttr(jsonType), xml.Elements()));
    }

    #endregion

    #region Deserialize
    
    /// <summary>
    /// 将JSON反序列化成对象
    /// </summary>
    public T Deserialize<T>()
    {
      return (T)Deserialize(typeof(T));
    }

    private object Deserialize(Type type)
    {
      return (IsArray) ? DeserializeArray(type) : DeserializeObject(type);
    }

    private dynamic DeserializeValue(XElement element, Type elementType)
    {
      var value = DynamicJsonHelper.ConvertElementToValue(element);
      if (value is DynamicJson)
      {
        value = ((DynamicJson)value).Deserialize(elementType);
      }
      return Convert.ChangeType(value, elementType);
    }

    private object DeserializeObject(Type targetType)
    {
      var result = Activator.CreateInstance(targetType);
      var dict = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
          .Where(p => p.CanWrite)
          .ToDictionary(pi => pi.Name, pi => pi);
      foreach (var item in xml.Elements())
      {
        PropertyInfo propertyInfo;
        if (!dict.TryGetValue(item.Name.LocalName, out propertyInfo)) continue;
        var value = DeserializeValue(item, propertyInfo.PropertyType);
        propertyInfo.SetValue(result, value, null);
      }
      return result;
    }

    private object DeserializeArray(Type targetType)
    {
      if (targetType.IsArray) // Foo[]
      {
        var elemType = targetType.GetElementType();
        dynamic array = Array.CreateInstance(elemType, xml.Elements().Count());
        var index = 0;
        foreach (var item in xml.Elements())
        {
          array[index++] = DeserializeValue(item, elemType);
        }
        return array;
      }
      else // List<Foo>
      {
        var elemType = targetType.GetGenericArguments()[0];
        dynamic list = Activator.CreateInstance(targetType);
        foreach (var item in xml.Elements())
        {
          list.Add(DeserializeValue(item, elemType));
        }
        return list;
      }
    }

    #endregion

    #region DynamicObject Overrides

    /// <summary>
    /// Provides the implementation for operations that invoke an object. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as invoking an object or a delegate.
    /// </summary>
    /// <param name="binder">Provides information about the invoke operation.</param>
    /// <param name="args">The arguments that are passed to the object during the invoke operation. For example, for the sampleObject(100) operation, where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, args[0] is equal to 100.</param>
    /// <param name="result">The result of the object invocation.</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.
    /// </returns>
    public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
    {
      // Delete
      result = (IsArray)
          ? Delete((int)args[0])
          : Delete((string)args[0]);
      return true;
    }

    /// <summary>
    /// Provides the implementation for operations that invoke a member. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as calling a method.
    /// </summary>
    /// <param name="binder">Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="args">The arguments that are passed to the object member during the invoke operation. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, args[0] is equal to 100.</param>
    /// <param name="result">The result of the member invocation.</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
    /// </returns>
    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    {
      // IsDefined, if has args then TryGetMember
      if (args.Length > 0)
      {
        result = null;
        return false;
      }

      result = IsDefined(binder.Name);
      return true;
    }

    /// <summary>
    /// Provides implementation for type conversion operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations that convert an object from one type to another.
    /// </summary>
    /// <param name="binder">Provides information about the conversion operation. The binder.Type property provides the type to which the object must be converted. For example, for the statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Type returns the <see cref="T:System.String"/> type. The binder.Explicit property provides information about the kind of conversion that occurs. It returns true for explicit conversion and false for implicit conversion.</param>
    /// <param name="result">The result of the type conversion operation.</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
    /// </returns>
    public override bool TryConvert(ConvertBinder binder, out object result)
    {
      // Deserialize or foreach(IEnumerable)
      if (binder.Type == typeof(IEnumerable) || binder.Type == typeof(object[]))
      {
        var ie = (IsArray)
            ? xml.Elements().Select(x => DynamicJsonHelper.ConvertElementToValue(x))
            : xml.Elements().Select(x => (dynamic)new KeyValuePair<string, object>(x.Name.LocalName, DynamicJsonHelper.ConvertElementToValue(x)));
        result = (binder.Type == typeof(object[])) ? ie.ToArray() : ie;
      }
      else
      {
        result = Deserialize(binder.Type);
      }
      return true;
    }

    /// <summary>
    /// Provides the implementation for operations that get a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for indexing operations.
    /// </summary>
    /// <param name="binder">Provides information about the operation.</param>
    /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] operation in C# (sampleObject(3) in Visual Basic), where sampleObject is derived from the DynamicObject class, indexes[0] is equal to 3.</param>
    /// <param name="result">The result of the index operation.</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
    /// </returns>
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
    {
      return (IsArray)
          ? TryGet(xml.Elements().ElementAtOrDefault((int)indexes[0]), out result)
          : TryGet(xml.Element((string)indexes[0]), out result);
    }

    /// <summary>
    /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
    /// </returns>
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      return (IsArray)
          ? TryGet(xml.Elements().ElementAtOrDefault(int.Parse(binder.Name)), out result)
          : TryGet(xml.Element(binder.Name), out result);
    }

    /// <summary>
    /// Provides the implementation for operations that set a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations that access objects by a specified index.
    /// </summary>
    /// <param name="binder">Provides information about the operation.</param>
    /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, indexes[0] is equal to 3.</param>
    /// <param name="value">The value to set to the object that has the specified index. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="value"/> is equal to 10.</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.
    /// </returns>
    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
    {
      return (IsArray)
          ? TrySet((int)indexes[0], value)
          : TrySet((string)indexes[0], value);
    }

    /// <summary>
    /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
    /// </summary>
    /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
    /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".</param>
    /// <returns>
    /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
    /// </returns>
    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      return (IsArray)
          ? TrySet(int.Parse(binder.Name), value)
          : TrySet(binder.Name, value);
    }

    /// <summary>
    /// Returns the enumeration of all dynamic member names.
    /// </summary>
    /// <returns>
    /// A sequence that contains dynamic member names.
    /// </returns>
    public override IEnumerable<string> GetDynamicMemberNames()
    {
      return (IsArray)
          ? xml.Elements().Select((x, i) => i.ToString())
          : xml.Elements().Select(x => x.Name.LocalName);
    }

    #endregion

    #region Private Methods

    private bool TryGet(XElement element, out object result)
    {
      if (element == null)
      {
        result = null;
        return false;
      }

      result = DynamicJsonHelper.ConvertElementToValue(element);
      return true;
    }

    private bool TrySet(string name, object value)
    {
      var type = DynamicJsonHelper.GetDynamicJsonType(value);
      var element = xml.Element(name);
      if (element == null)
      {
        xml.Add(new XElement(name, DynamicJsonHelper.CreateTypeAttr(type), DynamicJsonHelper.CreateJsonNode(value)));
      }
      else
      {
        element.Attribute("type").Value = type.ToString();
        element.ReplaceNodes(DynamicJsonHelper.CreateJsonNode(value));
      }

      return true;
    }

    private bool TrySet(int index, object value)
    {
      var type = DynamicJsonHelper.GetDynamicJsonType(value);
      var e = xml.Elements().ElementAtOrDefault(index);
      if (e == null)
      {
        xml.Add(new XElement("item", DynamicJsonHelper.CreateTypeAttr(type), DynamicJsonHelper.CreateJsonNode(value)));
      }
      else
      {
        e.Attribute("type").Value = type.ToString();
        e.ReplaceNodes(DynamicJsonHelper.CreateJsonNode(value));
      }

      return true;
    }

    #endregion
  }
}
