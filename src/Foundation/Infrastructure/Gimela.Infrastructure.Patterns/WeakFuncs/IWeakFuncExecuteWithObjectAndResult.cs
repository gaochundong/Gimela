
namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 该接口为WeakFunc泛型设计，该接口可用于不能预先知道参数具体类型时。
  /// </summary>
  public interface IWeakFuncExecuteWithObjectAndResult
  {
    /// <summary>
    /// 执行关联的Func使用参数。该参数将被转换成泛型T类型。
    /// </summary>
    /// <param name="parameter">该参数将被转换成泛型T类型</param>
    /// <returns>执行体的返回值</returns>
    object ExecuteWithObject(object parameter);
  }
}
