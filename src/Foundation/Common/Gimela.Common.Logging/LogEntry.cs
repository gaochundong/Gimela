using System;
using System.Text;

namespace Gimela.Common.Logging
{
  /// <summary>
  /// Log记录
  /// </summary>
  public class LogEntry
  {
    #region Data members

    private string _text;
    private string _time;
    private string _thread;
    private LogLevel _level;

    #endregion

    #region Properties

    /// <summary>
    /// Log文本
    /// </summary>
    public string Text
    {
      get { return _text; }
    }

    /// <summary>
    /// Log时间
    /// </summary>
    public string Time
    {
      get { return _time; }
    }

    /// <summary>
    /// 线程描述
    /// </summary>
    public string Thread
    {
      get { return _thread; }
    }

    /// <summary>
    /// Log级别
    /// </summary>
    public LogLevel Level
    {
      get { return _level; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Log记录
    /// </summary>
    /// <param name="text">Log文本</param>
    /// <param name="time">Log时间</param>
    /// <param name="thread">线程描述</param>
    /// <param name="level">Log级别</param>
    public LogEntry(string text, string time, string thread, LogLevel level)
    {
      _text = text;
      _time = time;
      _thread = thread;
      _level = level;
    }

    #endregion

    #region Override

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append(Time);
      sb.Append(" ");
      sb.Append(Thread);
      sb.Append(" ");
      sb.Append(Level.ToString());
      sb.Append(" ");
      sb.Append(Text);
      sb.Append(Environment.NewLine);

      return sb.ToString();
    }

    #endregion
  }
}
