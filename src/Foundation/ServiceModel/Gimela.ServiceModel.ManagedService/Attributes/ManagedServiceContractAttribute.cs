using System;

namespace Gimela.ServiceModel.ManagedService.Attributes
{
  /// <summary>
  /// 描述该接口为托管服务契约
  /// </summary>
  [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
  public sealed class ManagedServiceContractAttribute : Attribute
  {
  }
}
