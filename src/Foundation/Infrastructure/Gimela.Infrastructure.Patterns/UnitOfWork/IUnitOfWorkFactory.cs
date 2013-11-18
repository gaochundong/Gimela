
namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 工作单元工厂接口
  /// </summary>
  public interface IUnitOfWorkFactory
  {
    /// <summary>
    /// 创建工作单元
    /// </summary>
    /// <returns>工作单元</returns>
    IUnitOfWork Create();
  }
}
