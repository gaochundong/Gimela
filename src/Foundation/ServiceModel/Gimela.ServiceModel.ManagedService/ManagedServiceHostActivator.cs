using System;
using Gimela.ServiceModel.ManagedHosting;

namespace Gimela.ServiceModel.ManagedService
{
  /// <summary>
  /// 托管服务宿主激活器
  /// </summary>
  /// <typeparam name="TServiceContract">服务契约类型</typeparam>
  public class ManagedServiceHostActivator<TServiceContract> : ServiceHostActivator<TServiceContract>
    where TServiceContract : class
  {
    /// <summary>
    /// 托管服务宿主激活器
    /// </summary>
    /// <param name="info">服务宿主信息</param>
    public ManagedServiceHostActivator(ServiceHostInfo info)
      : base(info)
    {
      if (info.Service.GetInterface(typeof(IManagedService).ToString()) == null)
        throw new ArgumentException("The service does not implement the target interface.");
    }

    /// <summary>
    /// 启动服务
    /// </summary>
    public override void Start()
    {
      base.Start();
      (ServiceInstance as IManagedService).Start();
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    public override void Stop()
    {
      base.Stop();
      (ServiceInstance as IManagedService).Stop();      
    }
  }
}
