using System.ServiceModel;
using Gimela.ServiceModel.ChannelManagement.Factories;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 双向通道管理器
  /// </summary>
  /// <typeparam name="TContract">服务契约类型</typeparam>
  internal class ProxyDuplexChannelManager<TContract> : ProxyChannelManager<TContract> where TContract : class
  {
    #region Fields

    private InstanceContext _instanceContext;

    #endregion

    #region Constructors

    /// <summary>
    /// 双向通道管理器
    /// </summary>
    /// <param name="instanceContext">服务契约实例上下文</param>
    /// <param name="contract">服务契约描述</param>
    public ProxyDuplexChannelManager(InstanceContext instanceContext, ContractInfo contract)
      : base(contract)
    {
      this._instanceContext = instanceContext;
    }

    #endregion

    #region Create Channel

    /// <summary>
    /// 创建通道
    /// </summary>
    protected override void CreateChannel()
    {
      this.ProxyChannel = ProxyChannelFactory.CreateDuplexProxyChannel<TContract>(_instanceContext);
    }

    #endregion
  }
}
