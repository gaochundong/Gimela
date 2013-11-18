using System;
using System.Reflection;

namespace Gimela.Infrastructure.Patterns
{
  /// <summary>
  /// 弱Action模式
  /// </summary>
  /// <remarks>存放一个Action而不会引起对Action所有者的强引用，该所有者可在任何时刻被GC回收。</remarks>
  public class WeakAction
  {
    /// <summary>
    /// 弱Action模式
    /// </summary>
    protected WeakAction()
    {
    }

    /// <summary>
    /// 弱Action模式
    /// </summary>
    /// <param name="action">指定关联的Action</param>
    /// <remarks>存放一个Action而不会引起对Action所有者的强引用，该所有者可在任何时刻被GC回收。</remarks>
    public WeakAction(Action action)
      : this()
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
    protected Action StaticAction { get; set; }

    /// <summary>
    /// 获取指定的Action是否为静态或Lambda表达式
    /// </summary>
    public virtual bool IsStatic
    {
      get
      {
        return StaticAction != null;
      }
    }

    /// <summary>
    /// 指定的Action方法
    /// </summary>
    protected MethodInfo Method { get; set; }
    /// <summary>
    /// 指定的Action方法的引用宿主
    /// </summary>
    protected WeakReference Reference { get; set; }

    /// <summary>
    /// 指定的Action方法名称
    /// </summary>
    public virtual string MethodName
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
    public virtual bool IsAlive
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
    /// 获取关联的Action的所有者。
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
    /// 执行关联的Action，该操作只在Action的所有者仍然存活时有效。
    /// </summary>
    public void Execute()
    {
      if (StaticAction != null)
      {
        StaticAction();
        return;
      }

      if (Method != null && IsAlive)
      {
        var target = Target;
        if (target != null)
        {
          Method.Invoke(target, null);
        }
      }
    }

    /// <summary>
    /// 设置该对象可删除
    /// </summary>
    public virtual void Destroy()
    {
      Reference = null;
      Method = null;
      StaticAction = null;
    }
  }
}
