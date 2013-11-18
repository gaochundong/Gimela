
namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// Command模型基类
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class CommandBase<T> : ICommand<T>
  {
    /// <summary>
    /// 执行Command
    /// </summary>
    /// <param name="data">数据对象</param>
    public abstract void Execute(T data);

    /// <summary>
    /// 执行Command
    /// </summary>
    /// <param name="data">数据对象</param>
    public void Execute(object data)
    {
      Execute((T)data);
    }
  }
}
