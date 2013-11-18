using System;

namespace Gimela.ServiceModel.ManagedService.Attributes
{
  /// <summary>
  /// 描述该类为托管服务类
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
  public sealed class ManagedServiceAttribute : Attribute
  {
  }
}
