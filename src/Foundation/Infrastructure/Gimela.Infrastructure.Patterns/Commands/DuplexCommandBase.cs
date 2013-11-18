
namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 双向Command模型基类
  /// </summary>
  /// <typeparam name="TRequest">执行请求</typeparam>
  /// <typeparam name="TResponse">执行结果</typeparam>
  public abstract class DuplexCommandBase<TRequest, TResponse> : IDuplexCommand<TRequest, TResponse>
  {
    /// <summary>
    /// 执行Command
    /// </summary>
    /// <param name="data">数据对象</param>
    /// <returns>结果值</returns>
    public abstract TResponse Execute(TRequest data);

    /// <summary>
    /// 执行Command
    /// </summary>
    /// <param name="data">数据对象</param>
    /// <returns>
    /// 结果值
    /// </returns>
    public object Execute(object data)
    {
      return (TResponse)Execute((TRequest)data);
    }
  }
}
