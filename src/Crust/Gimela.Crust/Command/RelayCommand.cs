using System;
using System.Diagnostics;
using System.Windows.Input;
using Gimela.Infrastructure.Patterns;

namespace Gimela.Crust
{
  /// <summary>
  /// 中继Command，用于调用代理方法传递其功能至其他对象中。
  /// </summary>
  public class RelayCommand : System.Windows.Input.ICommand
  {
    private readonly WeakAction _execute;
    private readonly WeakFunc<bool> _canExecute;

    /// <summary>
    /// 中继Command
    /// </summary>
    /// <param name="execute">将被执行的逻辑方法</param>
    /// <exception cref="ArgumentNullException">如果参数为空则引发异常</exception>
    public RelayCommand(Action execute)
      : this(execute, null)
    {
    }

    /// <summary>
    /// 中继Command
    /// </summary>
    /// <param name="execute">将被执行的逻辑方法</param>
    /// <param name="canExecute">判断是否可被执行的逻辑方法</param>
    /// <exception cref="ArgumentNullException">如果参数为空则引发异常</exception>
    public RelayCommand(Action execute, Func<bool> canExecute)
    {
      if (execute == null)
      {
        throw new ArgumentNullException("execute");
      }

      _execute = new WeakAction(execute);
      if (canExecute != null)
      {
        _canExecute = new WeakFunc<bool>(canExecute);
      }
    }

    /// <summary>
    /// 当Command是否可执行的状态更改时发生。
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// 触发CanExecuteChanged事件
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
      var handler = CanExecuteChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// 判断此中继Command是否可被执行
    /// </summary>
    /// <param name="parameter">执行参数，该参数将直接被忽略。</param>
    /// <returns>如果中继Command可被执行，则返回真。</returns>
    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
      return _canExecute == null
          ? true
          : (_canExecute.IsStatic || _canExecute.IsAlive)
              ? _canExecute.Execute()
              : false;
    }

    /// <summary>
    /// 执行中继Command
    /// </summary>
    /// <param name="parameter">执行参数，该参数将直接被忽略。</param>
    public virtual void Execute(object parameter)
    {
      if (CanExecute(parameter)
          && _execute != null
          && (_execute.IsStatic || _execute.IsAlive))
      {
        _execute.Execute();
      }
    }
  }
}
