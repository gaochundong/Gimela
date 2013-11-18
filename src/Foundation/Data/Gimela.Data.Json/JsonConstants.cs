using System;

namespace Gimela.Data.Json
{
  /// <summary>
  /// JSON常量
  /// </summary>
  public static class JsonConstants
  {
    /// <summary>
    /// 常量True
    /// </summary>
    public static readonly string True = "true";
    /// <summary>
    /// 常量False
    /// </summary>
    public static readonly string False = "false";
    /// <summary>
    /// 常量Null
    /// </summary>
    public static readonly string Null = "null";
    /// <summary>
    /// 常量Undefined
    /// </summary>
    public static readonly string Undefined = "undefined";
    /// <summary>
    /// 常量NaN
    /// </summary>
    public static readonly string NaN = "NaN";
    /// <summary>
    /// 常量纪元时间, 1970-01-01 00:00:00
    /// </summary>
    public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
  }
}
