
namespace Gimela.Common.Logging
{
  /// <summary>
  /// Creates a file logger.
  /// </summary>
  public class FileLogFactory : ILogFactory
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FileLogFactory"/> class.
    /// </summary>
    public FileLogFactory()
    {
    }

    #region ILogFactory Members

    /// <summary>
    /// Create a new logger or get the default logger.
    /// </summary>
    /// <returns>Create a new logger or get the default logger.</returns>
    /// <remarks>
    /// MUST ALWAYS return a logger. Return NullLog if no logging
    /// should be used.
    /// </remarks>
    public ILogger CreateLogger()
    {
      return FileLogger.Instance;
    }

    #endregion
  }
}
