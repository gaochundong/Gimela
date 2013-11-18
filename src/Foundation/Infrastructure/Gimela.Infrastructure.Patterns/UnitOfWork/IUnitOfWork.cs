using System;

namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 工作单元模式接口
  /// </summary>
  public interface IUnitOfWork : IDisposable
  {
    /// <summary>
    /// 提交更改
    /// </summary>
    void Commit();

    /// <summary>
    /// 失败回滚
    /// </summary>
    void Rollback();
  }
}
