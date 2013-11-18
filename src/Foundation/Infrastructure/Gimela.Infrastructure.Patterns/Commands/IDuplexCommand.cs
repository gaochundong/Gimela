
namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 双向Command模型接口
  /// </summary>
  public interface IDuplexCommand
  {
    /// <summary>
    /// 执行Command
    /// </summary>
    /// <param name="data">数据对象</param>
    /// <returns>结果值</returns>
    object Execute(object data);
  }

  /// <summary>
  /// 双向Command模型接口
  /// </summary>
  /// <typeparam name="TRequest">执行请求</typeparam>
  /// <typeparam name="TResponse">执行结果</typeparam>
  public interface IDuplexCommand<TRequest, TResponse> : IDuplexCommand
  {
    /// <summary>
    /// 执行Command
    /// </summary>
    /// <param name="data">执行请求</param>
    /// <returns>执行结果</returns>
    TResponse Execute(TRequest data);
  }
}
