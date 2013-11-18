using System;

namespace Gimela.Data.Json
{
  /// <summary>
  /// 描述指定的属性将在序列化过程中被忽略
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public sealed class JsonIgnoreAttribute : Attribute
  {
  }
}
