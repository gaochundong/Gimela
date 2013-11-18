using System;

namespace Gimela.Common.Logging
{
  /// <summary>
  /// Log Factory is used to create new logs in the system.
  /// </summary>
  public static class LogFactory
  {
    private static ILogFactory _factory = new NullLogFactory();
    private static bool _isAssigned;

    /// <summary>
    /// Assigns log factory being used.
    /// </summary>
    /// <param name="logFactory">The log factory.</param>
    /// <exception cref="InvalidOperationException">A factory has already been assigned.</exception>
    public static void Assign(ILogFactory logFactory)
    {
      if (logFactory == _factory)
        return;
      if (_isAssigned)
        throw new InvalidOperationException("A log factory has already been assigned.");

      _isAssigned = true;
      _factory = logFactory;
    }

    /// <summary>
    /// Create a new logger or get the default logger.
    /// </summary>
    /// <returns>Create a new logger or get the default logger.</returns>
    public static ILogger CreateLogger()
    {
      return _factory.CreateLogger();
    }
  }
}
