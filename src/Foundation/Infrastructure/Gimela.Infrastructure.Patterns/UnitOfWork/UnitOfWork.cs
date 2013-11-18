using System;

namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 工作单元模式
  /// </summary>
  public static class UnitOfWork
  {
    private static IUnitOfWorkFactory unitOfWorkFactory;
    private static IUnitOfWork unitOfWork;

    /// <summary>
    /// 指定工作单元工厂类
    /// </summary>
    /// <param name="factory">工作单元工厂类</param>
    public static void AssignUnitOfWorkFactory(IUnitOfWorkFactory factory)
    {
      unitOfWorkFactory = factory;
    }

    /// <summary>
    /// 获取或创建工作单元
    /// </summary>
    /// <returns>工作单元</returns>
    private static IUnitOfWork GetUnitOfWork()
    {
      if (unitOfWorkFactory == null)
        throw new InvalidProgramException("A unit of work factory must be assign firstly.");

      return unitOfWorkFactory.Create();
    }

    /// <summary>
    /// 当前工作单元
    /// </summary>
    public static IUnitOfWork Current
    {
      get
      {
        if (unitOfWork == null)
        {
          unitOfWork = GetUnitOfWork();
        }

        return unitOfWork;
      }
    }

    /// <summary>
    /// 提交工作单元
    /// </summary>
    public static void Commit()
    {
      if (unitOfWork != null)
      {
        unitOfWork.Commit();
      }
    }

    /// <summary>
    /// 回滚工作单元
    /// </summary>
    public static void Rollback()
    {
      if (unitOfWork != null)
      {
        unitOfWork.Rollback();
      }
    }
  }
}
