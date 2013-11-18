using System.ServiceModel;

namespace Gimela.ServiceModel.ChannelManagement.Factories
{
  /// <summary>
  /// 代理通道管理器工厂
  /// </summary>
  internal static class ProxyChannelManagerFactory
  {
    /// <summary>
    /// 创建单向代理通道管理器
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="contract">服务契约</param>
    /// <returns>代理通道管理器</returns>
    public static IProxyChannelManager<TContract> Create<TContract>(ContractInfo contract) where TContract : class
    {
      return new ProxyChannelManager<TContract>(contract);
    }

    /// <summary>
    /// 创建双向代理通道管理器
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="instanceContext">上下文实例</param>
    /// <param name="contract">服务契约</param>
    /// <returns>代理通道管理器</returns>
    public static IProxyChannelManager<TContract> CreateDuplex<TContract>(InstanceContext instanceContext, ContractInfo contract) where TContract : class
    {
      return new ProxyDuplexChannelManager<TContract>(instanceContext, contract);
    }

    /// <summary>
    /// 创建回调代理通道管理器
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="callbackInstanceContext">回调上下文实例</param>
    /// <param name="contract">服务契约</param>
    /// <returns>代理通道管理器</returns>
    public static IProxyChannelManager<TContract> CreateCallback<TContract>(TContract callbackInstanceContext, ContractInfo contract) where TContract : class
    {
      return new CallbackChannelManager<TContract>(callbackInstanceContext, contract);
    }
  }
}
