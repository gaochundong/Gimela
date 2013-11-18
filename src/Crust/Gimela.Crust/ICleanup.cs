
namespace Gimela.Crust
{
  /// <summary>
  /// 通用清理接口。由于未实现IDisposable接口，可保证实例在未被Dispose和GC回收情况下被清理。
  /// </summary>
  public interface ICleanup
  {
    /// <summary>
    /// 清理实例，例如保存状态，删除资源等。
    /// </summary>
    void Cleanup();
  }
}
