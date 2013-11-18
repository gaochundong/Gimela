
namespace Gimela.Common.Logging
{
  /// <summary>
  /// Log Factory implementation used to create logs.
  /// </summary>
  public interface ILogFactory
  {
    /// <summary>
    /// Create a new logger or get the default logger.
    /// </summary>
    /// <returns>Create a new logger or get the default logger.</returns>
    /// <remarks>
    /// MUST ALWAYS return a logger. Return NullLog if no logging
    /// should be used.
    /// </remarks>
    ILogger CreateLogger();
  }
}
