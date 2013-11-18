using System;

namespace Gimela.Net.Http
{
  /// <summary>
  /// 用于创建类型实例的工厂方法代理
  /// </summary>
  /// <returns>被创建的类型实例</returns>
  /// <remarks>
  /// Method must never fail.
  /// </remarks>
  public delegate object FactoryMethod(Type type, object[] arguments);
}
