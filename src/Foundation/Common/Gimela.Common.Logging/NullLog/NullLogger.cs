using System;

namespace Gimela.Common.Logging
{
  /// <summary>
  /// Default log writer, writes everything to void (nowhere).
  /// </summary>
  /// <seealso cref="ILogger"/>
  public sealed class NullLogger : ILogger
  {
    #region Singleton

    private static readonly object _locker = new Object();
    private static NullLogger _uniqueInstance;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullLogger"/> class.
    /// </summary>
    private NullLogger()
    {
    }

    /// <summary>
    /// Get the null logger static instance.
    /// </summary>
    /// <returns>NullLogger</returns>
    public static NullLogger Instance
    {
      get
      {
        if (_uniqueInstance == null)
        {
          lock (_locker)
          {
            if (_uniqueInstance == null)
            {
              _uniqueInstance = new NullLogger();
            }
          }
        }

        return _uniqueInstance;
      }
    }

    #endregion

    #region ILogger Members
    
    /// <summary>
    /// Write a entry needed when following through code during hard to find bugs.
    /// </summary>
    /// <param name="message">Log message</param>
    public void Trace(string message)
    {
    }

    /// <summary>
    /// Write an entry that helps when debugging code.
    /// </summary>
    /// <param name="message">Log message</param>
    public void Debug(string message)
    {
    }

    /// <summary>
    /// Informational message, needed when helping customer to find a problem.
    /// </summary>
    /// <param name="message">Log message</param>
    public void Info(string message)
    {
    }

    /// <summary>
    /// Something is not as we expect, but the code can continue to run without any changes.
    /// </summary>
    /// <param name="message">Log message</param>
    public void Warning(string message)
    {
    }

    /// <summary>
    /// Something went wrong, but the application do not need to die. The current thread/request
    /// cannot continue as expected.
    /// </summary>
    /// <param name="message">Log message</param>
    public void Error(string message)
    {
    }

    /// <summary>
    /// Something went very wrong, application might not recover.
    /// </summary>
    /// <param name="message">Log message</param>
    public void Fatal(string message)
    {
    }

    /// <summary>
    /// Something thrown exception, put it in log.
    /// </summary>
    /// <param name="ex">Thrown exception to log.</param>
    public void Exception(Exception ex)
    {
    }

    /// <summary>
    /// Something thrown exception, put it in log.
    /// </summary>
    /// <param name="message">Log message</param>
    /// <param name="ex">Thrown exception to log.</param>
    public void Exception(string message, Exception ex)
    {
    }

    #endregion
  }
}
