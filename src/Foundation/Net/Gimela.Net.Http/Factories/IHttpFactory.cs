namespace Gimela.Net.Http
{
  /// <summary>
  /// 用于该HTTP协议栈的依赖注入器
  /// </summary>
  public interface IHttpFactory
  {
    /// <summary>
    /// 获取或创建一个类型
    /// </summary>
    /// <typeparam name="T">Type to create</typeparam>
    /// <returns>Created type.</returns>
    /// <remarks>
    /// Gets or creates types in the framework. 
    /// Check <see cref="HttpFactory"/> for more information on which
    /// types the factory should contain.
    /// </remarks>
    T Get<T>(params object[] constructorArguments) where T : class;
  }
}