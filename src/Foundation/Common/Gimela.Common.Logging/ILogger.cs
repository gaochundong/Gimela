using System;
using System.Diagnostics.CodeAnalysis;

namespace Gimela.Common.Logging
{
  /// <summary>
  /// Interface used to write to log files.
  /// </summary>
  public interface ILogger
  {
    /// <summary>
    /// Write a entry that helps when trying to find hard to find bugs.
    /// </summary>
    /// <param name="message">Log message</param>
    void Trace(string message);

    /// <summary>
    /// Write an entry that helps when debugging code.
    /// </summary>
    /// <param name="message">Log message</param>
    void Debug(string message);

    /// <summary>
    /// Informational message, needed when helping customer to find a problem.
    /// </summary>
    /// <param name="message">Log message</param>
    void Info(string message);

    /// <summary>
    /// Something is not as we expect, but the code can continue to run without any changes.
    /// </summary>
    /// <param name="message">Log message</param>
    void Warning(string message);

    /// <summary>
    /// Something went wrong, but the application do not need to die. The current thread/request
    /// cannot continue as expected.
    /// </summary>
    /// <param name="message">Log message</param>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error")]
    void Error(string message);

    /// <summary>
    /// Something went very wrong, application might not recover.
    /// </summary>
    /// <param name="message">Log message</param>
    void Fatal(string message);

    /// <summary>
    /// Something thrown exception, put it in log.
    /// </summary>
    /// <param name="ex">Thrown exception to log.</param>
    void Exception(Exception ex);

    /// <summary>
    /// Something thrown exception, put it in log.
    /// </summary>
    /// <param name="message">Log message</param>
    /// <param name="ex">Thrown exception to log.</param>
    void Exception(string message, Exception ex);
  }
}
