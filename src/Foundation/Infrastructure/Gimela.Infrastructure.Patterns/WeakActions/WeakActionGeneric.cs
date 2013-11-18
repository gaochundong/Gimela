using System;

namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 弱Action模式
  /// </summary>
  /// <remarks>存放一个Action而不会引起对Action所有者的强引用，该所有者可在任何时刻被GC回收。</remarks>
  /// <typeparam name="T">Action参数的类型</typeparam>
  public class WeakAction<T> : WeakAction, IWeakActionExecuteWithObject
  {
    /// <summary>
    /// 弱Action模式
    /// </summary>
    /// <remarks>存放一个Action而不会引起对Action所有者的强引用，该所有者可在任何时刻被GC回收。</remarks>
    /// <param name="action">指定关联的Action</param>
    public WeakAction(Action<T> action)
    {
      if (action == null)
        throw new ArgumentNullException("action");

      // 判断是否为静态或Lambda表达式，为无宿主状态
      if (action.Method.IsStatic)
      {
        StaticAction = action;
      }
      else
      {
        Method = action.Method;
      }

      if (action.Target != null)
      {
        Reference = new WeakReference(action.Target);
      }
    }

    /// <summary>
    /// 指定的静态Action方法或Lambda表达式
    /// </summary>
    protected new Action<T> StaticAction { get; set; }

    /// <summary>
    /// 获取指定的Action是否为静态或Lambda表达式
    /// </summary>
    public override bool IsStatic
    {
      get
      {
        return StaticAction != null;
      }
    }

    /// <summary>
    /// 指定的Action方法名称
    /// </summary>
    public override string MethodName
    {
      get
      {
        if (StaticAction != null)
        {
          return StaticAction.Method.Name;
        }
        else
        {
          return Method.Name;
        }
      }
    }

    /// <summary>
    /// 获取关联的Action的所有者是否依然存活。
    /// </summary>
    public override bool IsAlive
    {
      get
      {
        if (StaticAction == null && Reference == null)
        {
          return false;
        }

        if (StaticAction != null)
        {
          if (Reference != null)
          {
            return Reference.IsAlive;
          }

          return true;
        }

        return Reference.IsAlive;
      }
    }

    /// <summary>
    /// 执行关联的Action，该操作只在Action的所有者仍然存活时有效。Action参数将被设置为泛型T的默认值。
    /// </summary>
    public new void Execute()
    {
      Execute(default(T));
    }

    /// <summary>
    /// 执行关联的Action，该操作只在Action的所有者仍然存活时有效。
    /// </summary>
    /// <param name="parameter">Action参数</param>
    public void Execute(T parameter)
    {
      if (StaticAction != null)
      {
        StaticAction(parameter);
        return;
      }

      if (Method != null && IsAlive)
      {
        var target = Target;
        if (target != null)
        {
          Method.Invoke(target, new object[] { parameter });
        }
      }
    }

    /// <summary>
    /// 执行关联的Action使用参数。该参数将被转换成泛型T类型。
    /// </summary>
    /// <param name="parameter">该参数将被转换成泛型T类型</param>
    public void ExecuteWithObject(object parameter)
    {
      var parameterCasted = (T)parameter;
      Execute(parameterCasted);
    }

    /// <summary>
    /// 设置该对象可删除
    /// </summary>
    public override void Destroy()
    {
      base.Destroy();
      Reference = null;
      Method = null;
      StaticAction = null;
    }
  }
}
