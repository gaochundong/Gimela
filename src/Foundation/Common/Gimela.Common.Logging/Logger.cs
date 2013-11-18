using System;

namespace Gimela.Common.Logging
{
  /// <summary>
  /// Write log to console or file, depend on your assigned log factory.
  /// </summary>
  public static class Logger
  {
    /// <summary>
    /// Write a entry that helps when trying to find hard to find bugs.
    /// </summary>
    /// <param name="message">Log message</param>
    public static void Trace(string message)
    {
      LogFactory.CreateLogger().Trace(message);
    }

    /// <summary>
    /// Write an entry that helps when debugging code.
    /// </summary>
    /// <param name="message">Log message</param>
    public static void Debug(string message)
    {
      LogFactory.CreateLogger().Debug(message);
    }

    /// <summary>
    /// Informational message, needed when helping customer to find a problem.
    /// </summary>
    /// <param name="message">Log message</param>
    public static void Info(string message)
    {
      LogFactory.CreateLogger().Info(message);
    }

    /// <summary>
    /// Something is not as we expect, but the code can continue to run without any changes.
    /// </summary>
    /// <param name="message">Log message</param>
    public static void Warning(string message)
    {
      LogFactory.CreateLogger().Warning(message);
    }

    /// <summary>
    /// Something went wrong, but the application do not need to die. The current thread/request
    /// cannot continue as expected.
    /// </summary>
    /// <param name="message">Log message</param>
    public static void Error(string message)
    {
      LogFactory.CreateLogger().Error(message);
    }

    /// <summary>
    /// Something went very wrong, application might not recover.
    /// </summary>
    /// <param name="message">Log message</param>
    public static void Fatal(string message)
    {
      LogFactory.CreateLogger().Fatal(message);
    }

    /// <summary>
    /// Something thrown exception, put it in log.
    /// </summary>
    /// <param name="ex">Thrown exception to log.</param>
    public static void Exception(Exception ex)
    {
      LogFactory.CreateLogger().Exception(ex);
    }

    /// <summary>
    /// Something thrown exception, put it in log.
    /// </summary>
    /// <param name="message">Log message</param>
    /// <param name="ex">Thrown exception to log.</param>
    public static void Exception(string message, Exception ex)
    {
      LogFactory.CreateLogger().Exception(message, ex);
    }
  }
}
