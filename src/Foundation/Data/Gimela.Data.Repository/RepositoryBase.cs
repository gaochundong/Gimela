using System.Collections.Generic;

namespace Gimela.Data.Repository
{
  /// <summary>
  /// 数据仓库基类
  /// </summary>
  /// <typeparam name="TEntity">数据实体类型</typeparam>
  public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
    where TEntity : class, new()
  {
    #region IRepository<TEntity> Members

    /// <summary>
    /// 数据仓库全名称
    /// </summary>
    public virtual string FullName { get { return typeof(TEntity).FullName; } }

    /// <summary>
    /// 数据仓库名称
    /// </summary>
    public virtual string Name { get { return typeof(TEntity).Name; } }

    /// <summary>
    /// 实体数量
    /// </summary>
    /// <returns>实体数量</returns>
    public abstract int Count();

    /// <summary>
    /// 根据指定ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>指定ID的实体</returns>
    public abstract TEntity Find(string id);

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <returns>所有实体</returns>
    public abstract IEnumerable<TEntity> FindAll();

    /// <summary>
    /// 保存实体
    /// </summary>
    /// <param name="entity">指定实体</param>
    public abstract void Save(TEntity entity);

    /// <summary>
    /// 删除指定ID的实体
    /// </summary>
    /// <param name="id">实体ID</param>
    public abstract void Remove(string id);

    /// <summary>
    /// 删除所有实体
    /// </summary>
    public abstract void RemoveAll();

    #endregion
  }
}
