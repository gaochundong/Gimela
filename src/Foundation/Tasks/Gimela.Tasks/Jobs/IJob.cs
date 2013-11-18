
namespace Gimela.Tasks
{
  /// <summary>
  /// 作业接口
  /// </summary>
  public interface IJob
  {
    /// <summary>
    /// 作业Id
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// 作业名称
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// 执行作业
    /// </summary>
    void Run();
  }
}
