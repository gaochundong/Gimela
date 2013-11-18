
namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 该接口为WeakAction泛型设计，该接口可用于不能预先知道参数具体类型时。
  /// </summary>
  public interface IWeakActionExecuteWithObject
  {
    /// <summary>
    /// 执行关联的Action使用参数。该参数将被转换成泛型T类型。
    /// </summary>
    /// <param name="parameter">该参数将被转换成泛型T类型</param>
    void ExecuteWithObject(object parameter);
  }
}
