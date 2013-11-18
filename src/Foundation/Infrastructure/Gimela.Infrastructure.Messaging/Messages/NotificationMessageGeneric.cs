
namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 通知消息
  /// </summary>
  public class NotificationMessage<N, T> : MessageBase<T>
  {
    /// <summary>
    /// 通知消息
    /// </summary>
    /// <param name="notification">通知</param>
    /// <param name="content">指定类型对象</param>
    public NotificationMessage(N notification, T content)
      : base(content)
    {
      Notification = notification;
    }

    /// <summary>
    /// 通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="notification">通知</param>
    /// <param name="content">指定类型对象</param>
    public NotificationMessage(object sender, N notification, T content)
      : base(sender, content)
    {
      Notification = notification;
    }

    /// <summary>
    /// 通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="target">指定的接收者</param>
    /// <param name="notification">通知</param>
    /// <param name="content">指定类型对象</param>
    public NotificationMessage(object sender, object target, N notification, T content)
      : base(sender, target, content)
    {
      Notification = notification;
    }

    /// <summary>
    /// 获取通知内容
    /// </summary>
    public N Notification { get; protected set; }
  }
}
