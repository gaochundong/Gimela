
namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 发送单向消息
  /// </summary>
  /// <typeparam name="TContract">服务契约类型</typeparam>
  /// <typeparam name="TArgument">消息参数类型</typeparam>
  /// <param name="contract">服务实例</param>
  /// <param name="argument">消息参数</param>
  public delegate void SendOneWayMessage<TContract, TArgument>(TContract contract, TArgument argument);

  /// <summary>
  /// 发送请求回复消息
  /// </summary>
  /// <typeparam name="TContract">服务契约类型</typeparam>
  /// <typeparam name="TReturnData">返回值类型</typeparam>
  /// <typeparam name="TArgument">消息参数类型</typeparam>
  /// <param name="contract">服务实例</param>
  /// <param name="argument">消息参数</param>
  /// <returns>返回值</returns>
  public delegate TReturnData SendRequestReplyMessage<TContract, TReturnData, TArgument>(TContract contract, TArgument argument);

  /// <summary>
  /// 发送双向请求消息
  /// </summary>
  /// <typeparam name="TContract">服务契约类型</typeparam>
  /// <typeparam name="TReturnData">返回值类型</typeparam>
  /// <typeparam name="TArgument">消息参数类型</typeparam>
  /// <param name="contract">服务实例</param>
  /// <param name="argument">消息参数</param>
  /// <returns>返回值</returns>
  public delegate TReturnData SendDuplexMessage<TContract, TReturnData, TArgument>(TContract contract, TArgument argument);

  /// <summary>
  /// 发送双向回调消息
  /// </summary>
  /// <typeparam name="TContract">回调契约类型</typeparam>
  /// <typeparam name="TArgument">消息参数类型</typeparam>
  /// <param name="contract">回调服务实例</param>
  /// <param name="argument">消息参数</param>
  public delegate void SendCallbackMessage<TContract, TArgument>(TContract contract, TArgument argument);
}
