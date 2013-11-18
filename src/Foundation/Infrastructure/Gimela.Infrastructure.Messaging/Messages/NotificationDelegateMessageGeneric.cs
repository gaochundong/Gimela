using System;

namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 内置回调机制的消息类。当消息接收者处理完毕消息后可调用回调函数通知发送者该消息已经被处理。
  /// </summary>
  public class NotificationDelegateMessage<T> : NotificationMessage<T>
  {
    private readonly Delegate _callback;

    /// <summary>
    /// 内置回调机制的消息类
    /// </summary>
    /// <param name="notification">通知</param>
    /// <param name="content">指定的对象</param>
    /// <param name="callback">当消息接收者处理完毕消息后可调用回调函数通知发送者该消息已经被处理</param>
    public NotificationDelegateMessage(string notification, T content, Delegate callback)
      : base(notification, content)
    {
      if (callback == null)
      {
        throw new ArgumentNullException("callback");
      }
      _callback = callback;
    }

    /// <summary>
    /// 内置回调机制的消息类
    /// </summary>
    /// <param name="sender">消息的发送者</param>
    /// <param name="notification">通知</param>
    /// <param name="content">指定的对象</param>
    /// <param name="callback">当消息接收者处理完毕消息后可调用回调函数通知发送者该消息已经被处理</param>
    public NotificationDelegateMessage(object sender, string notification, T content, Delegate callback)
      : base(sender, notification, content)
    {
      if (callback == null)
      {
        throw new ArgumentNullException("callback");
      }
      _callback = callback;
    }

    /// <summary>
    /// 内置回调机制的消息类
    /// </summary>
    /// <param name="sender">消息的发送者</param>
    /// <param name="target">指定的接收者</param>
    /// <param name="notification">通知</param>
    /// <param name="content">指定的对象</param>
    /// <param name="callback">当消息接收者处理完毕消息后可调用回调函数通知发送者该消息已经被处理</param>
    public NotificationDelegateMessage(object sender, object target, string notification, T content, Delegate callback)
      : base(sender, target, notification, content)
    {
      if (callback == null)
      {
        throw new ArgumentNullException("callback");
      }
      _callback = callback;
    }

    /// <summary>
    /// 执行回调函数
    /// </summary>
    /// <param name="arguments">回调函数的参数</param>
    /// <returns>回调函数的执行结果返回值</returns>
    public virtual object Execute(params object[] arguments)
    {
      return _callback.DynamicInvoke(arguments);
    }
  }
}
