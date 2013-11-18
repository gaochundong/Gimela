
namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 静态单例
  /// </summary>
  /// <typeparam name="TClass">单例类型</typeparam>
  public static class Singleton<TClass> where TClass : class, new()
  {
    private static readonly object _lock = new object();
    private static TClass _instance = default(TClass);

    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static TClass GetInstance()
    {
      return Instance;
    }

    /// <summary>
    /// 单例实例
    /// </summary>
    public static TClass Instance
    {
      get
      {
        if (_instance == null)
        {
          lock (_lock)
          {
            if (_instance == null)
            {
              _instance = new TClass(); // must be public constructor
            }
          }
        }

        return _instance;
      }
    }

    /// <summary>
    /// 设置单例实例
    /// </summary>
    /// <param name="instance">单例实例</param>
    public static void Set(TClass instance)
    {
      lock (_lock)
      {
        _instance = instance;
      }
    }

    /// <summary>
    /// 重置单例实例
    /// </summary>
    public static void Reset()
    {
      lock (_lock)
      {
        _instance = default(TClass);
      }
    }
  }
}
