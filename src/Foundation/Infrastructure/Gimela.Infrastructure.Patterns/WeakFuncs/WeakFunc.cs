using System;
using System.Reflection;

namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 弱Func模式
  /// </summary>
  /// <remarks>存放一个Func而不会引起对Func所有者的强引用，该所有者可在任何时刻被GC回收。</remarks>
  /// <typeparam name="TResult">The type of the result.</typeparam>
  public class WeakFunc<TResult>
  {
    /// <summary>
    /// 弱Func模式
    /// </summary>
    protected WeakFunc()
    {
    }

    /// <summary>
    /// 弱Func模式
    /// </summary>
    /// <param name="func">指定关联的Func</param>
    /// <remarks>存放一个Func而不会引起对Func所有者的强引用，该所有者可在任何时刻被GC回收。</remarks>
    public WeakFunc(Func<TResult> func)
      : this()
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
    protected Func<TResult> StaticFunc { get; set; }

    /// <summary>
    /// 获取指定的Func是否为静态或Lambda表达式
    /// </summary>
    public virtual bool IsStatic
    {
      get
      {
        return StaticFunc != null;
      }
    }

    /// <summary>
    /// 指定的Func方法
    /// </summary>
    protected MethodInfo Method { get; set; }
    /// <summary>
    /// 指定的Func方法的引用宿主
    /// </summary>
    protected WeakReference Reference { get; set; }

    /// <summary>
    /// 指定的Func方法名称
    /// </summary>
    public virtual string MethodName
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
    public virtual bool IsAlive
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
    /// 获取关联的Func的所有者。
    /// </summary>
    public object Target
    {
      get
      {
        if (Reference == null)
        {
          return null;
        }

        return Reference.Target;
      }
    }

    /// <summary>
    /// 执行关联的Func，该操作只在Func的所有者仍然存活时有效。
    /// </summary>
    public TResult Execute()
    {
      if (StaticFunc != null)
      {
        return StaticFunc();
      }

      if (Method != null && IsAlive)
      {
        var target = Target;
        if (target != null)
        {
          return (TResult)Method.Invoke(target, null);
        }
      }

      return default(TResult);
    }

    /// <summary>
    /// 设置该对象可删除
    /// </summary>
    public virtual void Destroy()
    {
      Reference = null;
      Method = null;
      StaticFunc = null;
    }
  }
}
