using System.Collections.Generic;

namespace Gimela.Data.Repository
{
  /// <summary>
  /// 数据仓库
  /// </summary>
  /// <typeparam name="TEntity">数据实体类型</typeparam>
  public interface IRepository<TEntity> where TEntity : class, new()
  {
    /// <summary>
    /// 数据仓库全名称
    /// </summary>
    string FullName { get; }

    /// <summary>
    /// 数据仓库名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 实体数量
    /// </summary>
    /// <returns>实体数量</returns>
    int Count();

    /// <summary>
    /// 根据指定ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>指定ID的实体</returns>
    TEntity Find(string id);

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <returns>所有实体</returns>
    IEnumerable<TEntity> FindAll();

    /// <summary>
    /// 保存实体
    /// </summary>
    /// <param name="entity">指定实体</param>
    void Save(TEntity entity);

    /// <summary>
    /// 删除指定ID的实体
    /// </summary>
    /// <param name="id">实体ID</param>
    void Remove(string id);

    /// <summary>
    /// 删除所有实体
    /// </summary>
    void RemoveAll();
  }
}
