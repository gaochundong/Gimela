
namespace Gimela.Common.Logging
{
  /// <summary>
  /// Factory creating null logger.
  /// </summary>
  public class NullLogFactory : ILogFactory
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NullLogFactory"/> class.
    /// </summary>
    public NullLogFactory()
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
      return NullLogger.Instance;
    }

    #endregion
  }
}
