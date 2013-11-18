using System;

namespace Gimela.Net.Http
{
  /// <summary>
  /// An exception that can't be handled by the library have been thrown.
  /// </summary>
  public class ExceptionEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionEventArgs"/> class.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public ExceptionEventArgs(Exception exception)
    {
      Exception = exception;
    }

    /// <summary>
    /// Gets caught exception.
    /// </summary>
    public Exception Exception { get; private set; }
  }
}