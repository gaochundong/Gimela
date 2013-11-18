using System;

namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 弱Func模式
  /// </summary>
  /// <remarks>存放一个Func而不会引起对Func所有者的强引用，该所有者可在任何时刻被GC回收。</remarks>
  /// <typeparam name="T">Func参数的类型</typeparam>
  /// <typeparam name="TResult">The type of the result.</typeparam>
  public class WeakFunc<T, TResult> : WeakFunc<TResult>, IWeakFuncExecuteWithObjectAndResult
  {
    /// <summary>
    /// 弱Func模式
    /// </summary>
    /// <param name="func">指定关联的Func</param>
    /// <remarks>存放一个Func而不会引起对Func所有者的强引用，该所有者可在任何时刻被GC回收。</remarks>
    public WeakFunc(Func<T, TResult> func)
    {
      if (func == null)
        throw new ArgumentNullException("func");

      // 判断是否为静态或Lambda表达式，为无宿主状态
      if (func.Method.IsStatic)
      {
        StaticFunc = func;
      }
      else
      {
        Method = func.Method;
      }

      if (func.Target != null)
      {
        Reference = new WeakReference(func.Target);
      }
    }

    /// <summary>
    /// 指定的静态Func方法或Lambda表达式
    /// </summary>
    protected new Func<T, TResult> StaticFunc { get; set; }

    /// <summary>
    /// 获取指定的Func是否为静态或Lambda表达式
    /// </summary>
    public override bool IsStatic
    {
      get
      {
        return StaticFunc != null;
      }
    }

    /// <summary>
    /// 指定的Func方法名称
    /// </summary>
    public override string MethodName
    {
      get
      {
        if (StaticFunc != null)
        {
          return StaticFunc.Method.Name;
        }
        else
        {
          return Method.Name;
        }
      }
    }

    /// <summary>
    /// 获取关联的Func的所有者是否依然存活。
    /// </summary>
    public override bool IsAlive
    {
      get
      {
        if (StaticFunc == null && Reference == null)
        {
          return false;
        }

        if (StaticFunc != null)
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
    /// 执行关联的Func，该操作只在Func的所有者仍然存活时有效。Func参数将被设置为泛型T的默认值。
    /// </summary>
    public new void Execute()
    {
      Execute(default(T));
    }

    /// <summary>
    /// 执行关联的Func，该操作只在Func的所有者仍然存活时有效。
    /// </summary>
    /// <param name="parameter">Func参数</param>
    public TResult Execute(T parameter)
    {
      if (StaticFunc != null)
      {
        return StaticFunc(parameter);
      }

      if (Method != null && IsAlive)
      {
        var target = Target;
        if (target != null)
        {
          return (TResult)Method.Invoke(target, new object[] { parameter });
        }
      }

      return default(TResult);
    }

    /// <summary>
    /// 执行关联的Func使用参数。该参数将被转换成泛型T类型。
    /// </summary>
    /// <param name="parameter">该参数将被转换成泛型T类型</param>
    /// <returns>
    /// 执行体的返回值
    /// </returns>
    public object ExecuteWithObject(object parameter)
    {
      var parameterCasted = (T)parameter;
      return Execute(parameterCasted);
    }

    /// <summary>
    /// 设置该对象可删除
    /// </summary>
    public override void Destroy()
    {
      base.Destroy();
      Reference = null;
      Method = null;
      StaticFunc = null;
    }
  }
}
