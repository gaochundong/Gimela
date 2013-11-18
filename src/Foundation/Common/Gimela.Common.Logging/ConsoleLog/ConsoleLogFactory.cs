
namespace Gimela.Common.Logging
{
  /// <summary>
  /// Creates a console logger.
  /// </summary>
  public class ConsoleLogFactory : ILogFactory
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleLogFactory"/> class.
    /// </summary>
    public ConsoleLogFactory()
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
      return ConsoleLogger.Instance;
    }

    #endregion
  }
}
