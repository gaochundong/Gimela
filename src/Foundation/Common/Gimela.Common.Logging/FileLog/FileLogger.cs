using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;

namespace Gimela.Common.Logging
{
  /// <summary>
  /// 文件日志生成器
  /// </summary>
  public class FileLogger : ILogger, IDisposable
  {
    #region Fields

    private const int MAX_FILE_SIZE = 1024 * 1024 * 2; // 2 MB
    private const int DEFAULT_WAIT_TIMEOUT = -1;

    private static LogLevel _appSettingLogLevel = LogLevel.Info;

    private Queue<LogEntry> _logQueue;
    private readonly object _lockQueue = new object();
    private AutoResetEvent _signalQueue;
    private Thread _logThreadHandler;

    private StreamWriter _writer;
    private readonly object _lockerFlush = new object();
    private int _iterativeFlushCount = 0;       // 每新增一条自动加1
    private int _iterativeFlushPeriod = 1;      // 每隔n个日志刷新缓冲区一次 默认32， 1为每次都刷新
    private int _iterativeBackupFilePeriod = 0; // 每隔一定次数再判断是否备份文件，减少文件读时间
    private DispatcherTimer _timerOfLogClean;

    #endregion

    #region Singleton

    private static readonly object _locker = new Object();
    private static FileLogger _uniqueInstance;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileLogger"/> class.
    /// </summary>
    private FileLogger()
    {
      // 日志级别
      InitializeLogLevel();

      // 文本日志写入器
      _writer = new StreamWriter(FilePath, true);

      // 日志处理线程
      _logQueue = new Queue<LogEntry>();
      _signalQueue = new AutoResetEvent(false);
      _logThreadHandler = new Thread(LogHandler);
      _logThreadHandler.IsBackground = true;
      _logThreadHandler.Name = "LogHandlingThread";
      _logThreadHandler.Start();

      // 日志清理
      InitializeTimerOfLogClean();
    }

    /// <summary>
    /// Get the file logger static instance.
    /// </summary>
    /// <returns>FileLogger</returns>
    public static FileLogger Instance
    {
      get
      {
        if (_uniqueInstance == null)
        {
          lock (_locker)
          {
            if (_uniqueInstance == null)
            {
              _uniqueInstance = new FileLogger();
            }
          }
        }

        return _uniqueInstance;
      }
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Gets the log file directory.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public static string GetLogFileDirectory()
    {
      string dir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

      if (string.IsNullOrEmpty(dir))
      {
        dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      }

      dir = System.IO.Path.Combine(dir, @"log");
      if (!Directory.Exists(dir))
      {
        Directory.CreateDirectory(dir);
      }

      return dir;
    }

    /// <summary>
    /// Gets the log file name prefix.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public static string GetLogFileNamePrefix()
    {
      Assembly assem = Assembly.GetEntryAssembly();
      return assem == null ? "logfile" : assem.GetName().Name;
    }

    /// <summary>
    /// Gets the log file name suffix.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public static string GetLogFileNameSuffix()
    {
      return @"log";
    }

    /// <summary>
    /// Gets the log file name.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public static string GetLogFileName()
    {
      return string.Format(CultureInfo.InvariantCulture, @"{0}.{1}",
        GetLogFileNamePrefix(),
        GetLogFileNameSuffix());
    }

    /// <summary>
    /// Gets the log file temp name.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public static string GetLogFileTempName()
    {
      return string.Format(CultureInfo.InvariantCulture, @"{0}.temp.{1}",
        GetLogFileNamePrefix(),
        GetLogFileNameSuffix());
    }

    /// <summary>
    /// Gets the log file backup name.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public static string GetLogFileBackupName()
    {
      return string.Format(CultureInfo.InvariantCulture, @"{0}.{1}.{2}",
        GetLogFileNamePrefix(),
        DateTime.Now.ToString(@"yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        GetLogFileNameSuffix());
    }

    /// <summary>
    /// Gets the log file path.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public static string GetLogFilePath()
    {
      string dir = GetLogFileDirectory();
      return System.IO.Path.Combine(dir, GetLogFileName());
    }

    /// <summary>
    /// Gets the temp log file path.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public static string GetLogFileTempPath()
    {
      string dir = GetLogFileDirectory();
      return System.IO.Path.Combine(dir, GetLogFileTempName());
    }

    /// <summary>
    /// Gets the log file backup path.
    /// </summary>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public static string GetLogFileBackupPath()
    {
      string dir = GetLogFileDirectory();
      return System.IO.Path.Combine(dir, GetLogFileBackupName());
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the file directory.
    /// </summary>
    /// <value>The file directory.</value>
    public static string FileDirectory
    {
      get
      {
        return GetLogFileDirectory();
      }
    }

    /// <summary>
    /// Gets the file path.
    /// </summary>
    /// <value>The file path.</value>
    public static string FilePath
    {
      get
      {
        return GetLogFilePath();
      }
    }

    /// <summary>
    /// Gets the file temp path.
    /// </summary>
    /// <value>The file temp path.</value>
    public static string FileTempPath
    {
      get
      {
        return GetLogFileTempPath();
      }
    }

    #endregion

    #region Writter

    /// <summary>
    /// Write the specified log.
    /// </summary>
    /// <param name="log">log</param>
    private void Write(LogEntry log)
    {
      if (log != null)
      {
        lock (_lockQueue)
        {
          _logQueue.Enqueue(log);
        }

        _signalQueue.Set();
      }
    }

    /// <summary>
    /// Writes the specified level logs.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="message">The message.</param>
    private void Write(LogLevel level, string message)
    {
      if (level.CompareTo(_appSettingLogLevel) < 0) return;
      if (_writer == null) return;
      if (string.IsNullOrWhiteSpace(message)) return;

      Write(new LogEntry(
          message,
          DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture),
          Thread.CurrentThread.ManagedThreadId.ToString("000", CultureInfo.InvariantCulture),
          level));
    }

    /// <summary>
    /// Writes the specified level logs.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">Thrown exception to log.</param>
    private void Write(LogLevel level, string message, Exception exception)
    {
      string msg = string.Empty;
      msg = msg + message + Environment.NewLine;

      if (exception != null)
      {
        if (exception.StackTrace != null)
        {
          msg = string.Format(CultureInfo.InvariantCulture, @"{0}{1}{2}{3}",
              exception.Message.ToString(), Environment.NewLine,
              exception.StackTrace.ToString(), Environment.NewLine);
        }
        else
        {
          msg = string.Format(CultureInfo.InvariantCulture, @"{0}{1}",
              exception.Message.ToString(), Environment.NewLine);
        }
      }

      Write(level, msg);
    }

    /// <summary>
    /// Writes the specified level logs.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="exception">Thrown exception to log.</param>
    private void Write(LogLevel level, Exception exception)
    {
      string msg = string.Empty;
      if (exception != null)
      {
        if (exception.StackTrace != null)
        {
          msg = string.Format(CultureInfo.InvariantCulture, @"{0}{1}{2}{3}", 
              exception.Message.ToString(), Environment.NewLine,
              exception.StackTrace.ToString(), Environment.NewLine);
        }
        else
        {
          msg = string.Format(CultureInfo.InvariantCulture, @"{0}{1}",
              exception.Message.ToString(), Environment.NewLine);
        }
      }

      Write(level, msg);
    }

    #endregion

    #region Methods

    /// <summary>
    /// 初始化日志级别
    /// </summary>
    private static void InitializeLogLevel()
    {
      try
      {
        string loglevel = ConfigurationManager.AppSettings["LogLevel"];

        if (string.IsNullOrEmpty(loglevel))
        {
          loglevel = @"Trace";
        }

        if (loglevel.ToUpperInvariant() == @"TRACE")
        {
          _appSettingLogLevel = LogLevel.Trace;
        }
        else if (loglevel.ToUpperInvariant() == @"DEBUG")
        {
          _appSettingLogLevel = LogLevel.Debug;
        }
        else if (loglevel.ToUpperInvariant() == @"INFO")
        {
          _appSettingLogLevel = LogLevel.Info;
        }
        else if (loglevel.ToUpperInvariant() == @"WARNING")
        {
          _appSettingLogLevel = LogLevel.Warning;
        }
        else if (loglevel.ToUpperInvariant() == @"ERROR")
        {
          _appSettingLogLevel = LogLevel.Error;
        }
        else if (loglevel.ToUpperInvariant() == @"FATAL")
        {
          _appSettingLogLevel = LogLevel.Fatal;
        }
        else
        {
          _appSettingLogLevel = LogLevel.Info;
        }
      }
      catch (ConfigurationErrorsException)
      {
        _appSettingLogLevel = LogLevel.Info;
      }
    }

    /// <summary>
    /// 刷新缓冲区并检测文件大小
    /// </summary>
    private void FlushCheck()
    {
      lock (_lockerFlush)
      {
        // This is to flush the writer periodically after every N number of times.
        if (_iterativeFlushCount % _iterativeFlushPeriod == 0)
        {
          _writer.Flush();
          _iterativeFlushCount = 1;
        }
        else
        {
          _iterativeFlushCount++;
          return; // 不是每次都检测文件 加快日志速度
        }

        _iterativeBackupFilePeriod++;

        // 在指定周期后再判断文件大小
        if (_iterativeBackupFilePeriod > 12000)
        {
          BackupLogFile();
        }
      }
    }

    /// <summary>
    /// 备份日志文件
    /// </summary>
    private void BackupLogFile()
    {
      FileInfo fileInfo = new FileInfo(FilePath); // bytes

      // 检测日志文件大小，如超过设定大小，转移文件 1024*1024=1M
      if (fileInfo.Length > MAX_FILE_SIZE)
      {
        // 在首次判断文件成功时置为0
        _iterativeBackupFilePeriod = 0;

        try
        {
          _writer.Flush();
          _writer.Close();

          string bak = GetLogFileBackupPath();
          File.Move(FilePath, bak);

          _writer = new StreamWriter(FilePath);
        }
        catch (ObjectDisposedException ex)
        {
          System.Diagnostics.Trace.WriteLine(ex.Message);
        }
        catch (EncoderFallbackException ex)
        {
          System.Diagnostics.Trace.WriteLine(ex.Message);
        }
        catch (IOException ex)
        {
          System.Diagnostics.Trace.WriteLine(ex.Message);
        }
      }
    }

    /// <summary>
    /// 刷新缓冲区，将缓存内容写入文件中
    /// </summary>
    public static void Flush()
    {
      FileLogger.Instance.FlushWriter();
    }

    /// <summary>
    /// 刷新缓冲区，将缓存内容写入文件中
    /// </summary>
    public void FlushWriter()
    {
      if (_writer != null)
      {
        _writer.Flush();
      }
    }

    /// <summary>
    /// 在线程中处理日志队列
    /// </summary>
    private void LogHandler()
    {
      List<LogEntry> logs;

      while (true)
      {
        _signalQueue.WaitOne(DEFAULT_WAIT_TIMEOUT, false);

        logs = new List<LogEntry>();

        lock (_lockQueue)
        {
          while (_logQueue.Count > 0)
          {
            logs.Add(_logQueue.Dequeue());
          }
        }

        foreach (LogEntry log in logs)
        {
          _writer.Write(log.ToString());
          FlushCheck();
        }

        logs.Clear();
      }
    }

    #endregion

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        try
        {
          if (_signalQueue != null)
          {
            _signalQueue.Close();
          }
          if (_writer != null)
          {
            _writer.Flush();
            _writer.Close();
            _writer = null;
          }
        }
        catch (ObjectDisposedException ex)
        {
          System.Diagnostics.Trace.WriteLine(ex.Message);
        }
        catch (EncoderFallbackException ex)
        {
          System.Diagnostics.Trace.WriteLine(ex.Message);
        }
        catch (IOException ex)
        {
          System.Diagnostics.Trace.WriteLine(ex.Message);
        }
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

    #region Log Timer

    /// <summary>
    /// 初始化日志清理计时器
    /// </summary>
    private void InitializeTimerOfLogClean()
    {
      CleanHistoryLog(); // 启动时先清理

      _timerOfLogClean = new DispatcherTimer();
      _timerOfLogClean.Tick += new EventHandler(OnTimerOfLogCleanTick);

      // 计时器刻度之间的时间段 hours/minutes/seconds
      _timerOfLogClean.Interval = new TimeSpan(1, 0, 0);

      // 直接启动
      _timerOfLogClean.Start();
    }

    /// <summary>
    /// 超过计时器间隔时发生
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> 
    /// instance containing the event data.</param>
    void OnTimerOfLogCleanTick(object sender, EventArgs e)
    {
      CleanHistoryLog();
    }

    #endregion

    #region Clean Log

    /// <summary>
    /// 清理历史日志
    /// </summary>
    private static void CleanHistoryLog()
    {
      try
      {
        string[] files = Directory.GetFiles(FileDirectory);

        if (files.Length > 0)
        {
          // 保持前三天的日志
          string dayBefore0 = DateTime.Now.ToString(@"yyyyMMdd", CultureInfo.InvariantCulture);
          string dayBefore1 = DateTime.Now.AddDays(-1).ToString(@"yyyyMMdd", CultureInfo.InvariantCulture);
          string dayBefore2 = DateTime.Now.AddDays(-2).ToString(@"yyyyMMdd", CultureInfo.InvariantCulture);
          string dayBefore3 = DateTime.Now.AddDays(-3).ToString(@"yyyyMMdd", CultureInfo.InvariantCulture);

          // format : dce.yyyyMMddHHmmss.txt
          Regex regex = null;
          string pattern = null;
          string logFilePrefix = string.Format(CultureInfo.InvariantCulture, @"{0}.", GetLogFileNamePrefix());
          string logFileSuffix = string.Format(CultureInfo.InvariantCulture, @".{0}", GetLogFileNameSuffix());

          foreach (string file in files)
          {
            pattern = logFilePrefix + @"\d{14}" + logFileSuffix + @"$";
            regex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (!regex.IsMatch(file))
            {
              continue; // 不符合日期文件名的文件不处理
            }

            pattern = logFilePrefix + dayBefore0 + @"\d{6}" + logFileSuffix + @"$";
            regex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(file))
            {
              continue;
            }

            pattern = logFilePrefix + dayBefore1 + @"\d{6}" + logFileSuffix + @"$";
            regex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(file))
            {
              continue;
            }

            pattern = logFilePrefix + dayBefore2 + @"\d{6}" + logFileSuffix + @"$";
            regex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(file))
            {
              continue;
            }

            pattern = logFilePrefix + dayBefore3 + @"\d{6}" + logFileSuffix + @"$";
            regex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(file))
            {
              continue;
            }

            // 存在需要删除的文件
            File.Delete(file);
          }
        }
      }
      catch (ArgumentException ex)
      {
        System.Diagnostics.Trace.WriteLine(ex.Message);
      }
      catch (DirectoryNotFoundException ex)
      {
        System.Diagnostics.Trace.WriteLine(ex.Message);
      }
      catch (NotSupportedException ex)
      {
        System.Diagnostics.Trace.WriteLine(ex.Message);
      }
      catch (PathTooLongException ex)
      {
        System.Diagnostics.Trace.WriteLine(ex.Message);
      }
      catch (UnauthorizedAccessException ex)
      {
        System.Diagnostics.Trace.WriteLine(ex.Message);
      }
      catch (IOException ex)
      {
        System.Diagnostics.Trace.WriteLine(ex.Message);
      }
    }

    #endregion
  }
}
