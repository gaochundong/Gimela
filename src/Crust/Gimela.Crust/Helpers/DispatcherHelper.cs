using System;
using System.Windows.Threading;

namespace Gimela.Crust
{
  /// <summary>
  /// 在UI线程上执行调用的帮助类
  /// </summary>
  public static class DispatcherHelper
  {
    /// <summary>
    /// 获取当前UI线程的派遣器
    /// </summary>
    public static Dispatcher UIDispatcher
    {
      get;
      private set;
    }

    /// <summary>
    /// 派遣器初始化方法。
    /// <para>In a Silverlight application, call this method in the
    /// Application_Startup event handler, after the MainPage is constructed.</para>
    /// <para>In WPF, call this method on the static App() constructor.</para>
    /// </summary>
    public static void Initialize()
    {
      if (UIDispatcher == null)
      {
        UIDispatcher = Dispatcher.CurrentDispatcher;
      }
    }

    /// <summary>
    /// 在UI线程上执行操作。
    /// <param name="action">将在UI线程上执行的操作</param>
    public static void InvokeOnUI(Action action)
    {
      if (action == null)
        throw new ArgumentNullException("action");

      if (UIDispatcher.CheckAccess())
      {
        action();
      }
      else
      {
        UIDispatcher.BeginInvoke(action);
      }
    }
  }
}
