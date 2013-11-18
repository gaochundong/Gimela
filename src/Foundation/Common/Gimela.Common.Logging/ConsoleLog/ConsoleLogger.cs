using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Gimela.Common.Logging
{
  /// <summary>
  /// This class writes logs to the console. 
  /// </summary>
  /// <remarks>
  /// It colors the output depending on the log level 
  /// and includes a 3-level stack trace (in debug mode)
  /// </remarks>
  /// <seealso cref="ILogger"/>
  public sealed class ConsoleLogger : ILogger
  {
    #region Singleton

    private static readonly object _locker = new Object();
    private static ConsoleLogger _uniqueInstance;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
    /// </summary>
    private ConsoleLogger()
    {
    }

    /// <summary>
    /// Get the console logger static instance.
    /// </summary>
    /// <returns>ConsoleLogger</returns>
    public static ConsoleLogger Instance
    {
      get
      {
        if (_uniqueInstance == null)
        {
          lock (_locker)
          {
            if (_uniqueInstance == null)
            {
              _uniqueInstance = new ConsoleLogger();
            }
          }
        }

        return _uniqueInstance;
      }
    }

    #endregion

    #region Private Methods
    
    private static ConsoleColor GetLogLevelColor(LogLevel level)
    {
      switch (level)
      {
        case LogLevel.Trace:
          return ConsoleColor.DarkGray;
        case LogLevel.Debug:
          return ConsoleColor.Gray;
        case LogLevel.Info:
          return ConsoleColor.White;
        case LogLevel.Warning:
          return ConsoleColor.DarkMagenta;
        case LogLevel.Error:
          return ConsoleColor.Magenta;
        case LogLevel.Fatal:
          return ConsoleColor.Red;
      }

      return ConsoleColor.Yellow;
    }

    private static void Write(LogLevel level, string message)
    {
      var sb = new StringBuilder();
      AppendMessage(level, message, sb);

      Console.ForegroundColor = GetLogLevelColor(level);
      Console.WriteLine(sb.ToString());
      Console.ForegroundColor = ConsoleColor.Gray;
    }

    private static void Write(LogLevel level, string message, Exception exception)
    {
      var sb = new StringBuilder();
      AppendMessage(level, message, sb);
      sb.Append(Environment.NewLine);
      sb.Append(exception);

      Console.ForegroundColor = GetLogLevelColor(level);
      Console.WriteLine(sb.ToString());
      Console.ForegroundColor = ConsoleColor.Gray;
    }

    private static void Write(LogLevel level, Exception exception)
    {
      var sb = new StringBuilder();
      AppendMessage(level, "", sb);
      sb.Append(Environment.NewLine);
      sb.Append(exception);

      Console.ForegroundColor = GetLogLevelColor(level);
      Console.WriteLine(sb.ToString());
      Console.ForegroundColor = ConsoleColor.Gray;
    }

    private static void AppendMessage(LogLevel level, string message, StringBuilder sb)
    {
      sb.Append(DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture));
      sb.Append(" ");
      sb.Append(Thread.CurrentThread.ManagedThreadId.ToString("000", CultureInfo.InvariantCulture));
      sb.Append(" ");
      sb.Append(level.ToString());
      sb.Append(" ");
#if DEBUG
      var trace = new StackTrace();
      StackFrame[] frames = trace.GetFrames();
      if (frames != null && frames.Length > 4)
      {
        string frame = frames[4].GetMethod().ReflectedType.Name + "." + frames[4].GetMethod().Name;
        sb.Append(frame);
      }
#endif
      sb.Append(" ");
      sb.Append(message);
    }

    #endregion

    #region ILogger Members

    /// <summary>
    /// Write a entry needed when following through code during hard to find bugs.
    /// </summary>
    /// <param name="message">Log message</param>
    public void Trace(string message)
    {
      Write(LogLevel.Trace, message);
    }

    /// <summary>
    /// Write an entry that helps when debugging code.
    /// </summary>
    /// <param name="message">Log message</param>
    public void Debug(string message)
    {
      Write(LogLevel.Debug, message);
    }

    /// <summary>
    /// Informational message, needed when helping customer to find a problem.
    /// </summary>
    /// <param name="message">Log message</param>
    public void Info(string message)
    {
      Write(LogLevel.Info, message);
    }

    /// <summary>
    /// Something is not as we expect, but the code can continue to run without any changes.
    /// </summary>
    /// <param name="message">Log message</param>
    public void Warning(string message)
    {
      Write(LogLevel.Warning, message);
    }

    /// <summary>
    /// Something went wrong, but the application do not need to die. The current thread/request
    /// cannot continue as expected.
    /// </summary>
    /// <param name="message">Log message</param>
    public void Error(string message)
    {
      Write(LogLevel.Error, message);
    }

    /// <summary>
    /// Something went very wrong, application might not recover.
    /// </summary>
    /// <param name="message">Log message</param>
    public void Fatal(string message)
    {
      Write(LogLevel.Fatal, message);
    }

    /// <summary>
    /// Something thrown exception, put it in log.
    /// </summary>
    /// <param name="ex">Thrown exception to log.</param>
    public void Exception(Exception ex)
    {
      Write(LogLevel.Exception, ex);
    }

    /// <summary>
    /// Something thrown exception, put it in log.
    /// </summary>
    /// <param name="message">Log message</param>
    /// <param name="ex">Thrown exception to log.</param>
    public void Exception(string message, Exception ex)
    {
      Write(LogLevel.Exception, message, ex);
    }

    #endregion
  }
}
