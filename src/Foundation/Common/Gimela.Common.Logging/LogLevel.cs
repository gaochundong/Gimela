
namespace Gimela.Common.Logging
{
  /// <summary>
  /// Log级别
  /// </summary>
  public enum LogLevel
  {
    /// <summary>
    /// Very detailed logs to be able to follow the flow of the program.
    /// </summary>
    Trace,

    /// <summary>
    /// Log to help debug errors in the application
    /// </summary>
    Debug,

    /// <summary>
    /// Information to be able to keep track of state changes etc.
    /// </summary>
    Info,

    /// <summary>
    /// Something did not go as we expected, but it's no problem.
    /// </summary>
    Warning,

    /// <summary>
    /// Something that should not fail failed, but we can still keep
    /// on going.
    /// </summary>
    Error,

    /// <summary>
    /// Something failed, and we cannot handle it properly.
    /// </summary>
    Fatal,

    /// <summary>
    /// Something thrown exception.
    /// </summary>
    Exception,
  }
}
