
namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// Command模型接口
  /// </summary>
  public interface ICommand
  {
    /// <summary>
    /// 执行Command
    /// </summary>
    /// <param name="data">数据对象</param>
    void Execute(object data);
  }

  /// <summary>
  /// Command模型接口
  /// </summary>
  /// <typeparam name="T">泛型类型</typeparam>
  public interface ICommand<T> : ICommand
  {
    /// <summary>
    /// 执行Command
    /// </summary>
    /// <param name="data">数据对象</param>
    void Execute(T data);
  }
}
