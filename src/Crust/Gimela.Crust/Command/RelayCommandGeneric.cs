using System;
using System.Windows.Input;
using Gimela.Infrastructure.Patterns;

namespace Gimela.Crust
{
  /// <summary>
  /// 中继Command，用于调用代理方法传递其功能至其他对象中。
  /// </summary>
  /// <typeparam name="T">执行方法中的参数类型</typeparam>
  public class RelayCommand<T> : System.Windows.Input.ICommand
  {
    private readonly WeakAction<T> _execute;
    private readonly WeakFunc<T, bool> _canExecute;

    /// <summary>
    /// 中继Command
    /// </summary>
    /// <param name="execute">将被执行的逻辑方法</param>
    /// <exception cref="ArgumentNullException">如果参数为空则引发异常</exception>
    public RelayCommand(Action<T> execute)
      : this(execute, null)
    {
    }

    /// <summary>
    /// 中继Command
    /// </summary>
    /// <param name="execute">将被执行的逻辑方法</param>
    /// <param name="canExecute">判断是否可被执行的逻辑方法</param>
    /// <exception cref="ArgumentNullException">如果参数为空则引发异常</exception>
    public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
    {
      if (execute == null)
      {
        throw new ArgumentNullException("execute");
      }

      _execute = new WeakAction<T>(execute);
      if (canExecute != null)
      {
        _canExecute = new WeakFunc<T, bool>(canExecute);
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
    /// <param name="parameter">执行参数，如果该中继Command中执行无需参数，则可将该参数置为空。</param>
    /// <returns>如果中继Command可被执行，则返回真。</returns>
    public bool CanExecute(object parameter)
    {
      if (_canExecute == null)
      {
        return true;
      }

      if (_canExecute.IsStatic || _canExecute.IsAlive)
      {
        if (parameter == null && typeof(T).IsValueType)
        {
          return _canExecute.Execute(default(T));
        }

        return _canExecute.Execute((T)parameter);
      }

      return false;
    }

    /// <summary>
    /// 执行中继Command
    /// </summary>
    /// <param name="parameter">执行参数，如果该中继Command中执行无需参数，则可将该参数置为空。</param>
    public virtual void Execute(object parameter)
    {
      var val = parameter;

      if (parameter != null
          && parameter.GetType() != typeof(T))
      {
        if (parameter is IConvertible)
        {
          val = Convert.ChangeType(parameter, typeof(T), null);
        }
      }

      if (CanExecute(val)
          && _execute != null
          && (_execute.IsStatic || _execute.IsAlive))
      {
        if (val == null)
        {
          if (typeof(T).IsValueType)
          {
            _execute.Execute(default(T));
          }
          else
          {
            _execute.Execute((T)val);
          }
        }
        else
        {
          _execute.Execute((T)val);
        }
      }
    }
  }
}
