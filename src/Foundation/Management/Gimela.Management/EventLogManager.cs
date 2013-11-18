using System;
using System.ComponentModel;
using System.Diagnostics;
using Gimela.Common.ExceptionHandling;

namespace Gimela.Management
{
  /// <summary>
  /// Windows事件日志管理器
  /// </summary>
  public static class EventLogManager
  {
    /// <summary>
    /// 获取事件日志
    /// </summary>
    /// <param name="machine">主机名称</param>
    /// <param name="logName">日志名称</param>
    /// <returns>事件日志</returns>
    public static EventLog GetEventLog(string machine, string logName)
    {
      EventLog retLog = null;

      EventLog[] logs = EventLog.GetEventLogs(machine);
      foreach (EventLog log in logs)
      {
        if (log.Log == logName)
          retLog = log;
      }

      return retLog;
    }

    /// <summary>
    /// 创建事件日志
    /// </summary>
    /// <param name="machine">主机名称</param>
    /// <param name="logName">日志名称</param>
    /// <param name="source">事件源描述</param>
    /// <returns>事件日志</returns>
    public static EventLog CreateEventLog(string machine, string logName, string source)
    {
      EventLog log = GetEventLog(machine, logName);

      if (log == null)
      {
        EventSourceCreationData data = new EventSourceCreationData(source, logName);
        EventLog.CreateEventSource(data);
        log = GetEventLog(machine, logName);
        log.Source = source;
      }
      else
      {
        log.Source = source;
      }

      return log;
    }

    /// <summary>
    /// 删除事件日志
    /// </summary>
    /// <param name="machine">主机名称</param>
    /// <param name="logName">日志名称</param>
    /// <param name="source">事件源描述</param>
    public static void DeleteEventLog(string machine, string logName, string source)
    {
      if (EventLog.Exists(logName, machine))
      {
        EventLog.DeleteEventSource(source, machine);
        EventLog.Delete(logName, machine);
      }
    }

    /// <summary>
    /// 写入应用事件日志
    /// </summary>
    /// <param name="source">事件源描述</param>
    /// <param name="message">事件日志信息</param>
    public static void WriteApplicationEventLog(string source, string message)
    {
      try
      {
        using (EventLog log = CreateEventLog(Environment.MachineName, "Application", source))
        {
          log.WriteEntry(message, EventLogEntryType.Error);
        }
      }
      catch (InvalidEnumArgumentException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (InvalidOperationException ex)
      {
        ExceptionHandler.Handle(ex);
      }
      catch (Win32Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }
  }
}
