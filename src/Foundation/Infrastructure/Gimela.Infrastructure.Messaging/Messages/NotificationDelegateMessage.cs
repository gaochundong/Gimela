using System;

namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 内置回调机制的消息类。当消息接收者处理完毕消息后可调用回调函数通知发送者该消息已经被处理。
  /// </summary>
  public class NotificationDelegateMessage : NotificationDelegateMessage<string>
  {
    /// <summary>
    /// 内置回调机制的消息类
    /// </summary>
    /// <param name="notification">通知</param>
    /// <param name="callback">当消息接收者处理完毕消息后可调用回调函数通知发送者该消息已经被处理</param>
    public NotificationDelegateMessage(string notification, Delegate callback)
      : base(notification, notification, callback)
    {
    }

    /// <summary>
    /// 内置回调机制的消息类
    /// </summary>
    /// <param name="sender">消息的发送者</param>
    /// <param name="notification">通知</param>
    /// <param name="callback">当消息接收者处理完毕消息后可调用回调函数通知发送者该消息已经被处理</param>
    public NotificationDelegateMessage(object sender, string notification, Delegate callback)
      : base(sender, notification, notification, callback)
    {
    }

    /// <summary>
    /// 内置回调机制的消息类
    /// </summary>
    /// <param name="sender">消息的发送者</param>
    /// <param name="target">指定的接收者</param>
    /// <param name="notification">通知</param>
    /// <param name="callback">当消息接收者处理完毕消息后可调用回调函数通知发送者该消息已经被处理</param>
    public NotificationDelegateMessage(object sender, object target, string notification, Delegate callback)
      : base(sender, target, notification, notification, callback)
    {
    }
  }
}
