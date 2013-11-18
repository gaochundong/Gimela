
namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 通知消息
  /// </summary>
  public class NotificationMessage<T> : NotificationMessage<string, T>
  {
    /// <summary>
    /// 通知消息
    /// </summary>
    /// <param name="notification">通知</param>
    /// <param name="content">指定类型对象</param>
    public NotificationMessage(string notification, T content)
      : base(notification, content)
    {
    }

    /// <summary>
    /// 通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="notification">通知</param>
    /// <param name="content">指定类型对象</param>
    public NotificationMessage(object sender, string notification, T content)
      : base(sender, notification, content)
    {
    }

    /// <summary>
    /// 通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="target">指定的接收者</param>
    /// <param name="notification">通知</param>
    /// <param name="content">指定类型对象</param>
    public NotificationMessage(object sender, object target, string notification, T content)
      : base(sender, target, notification, content)
    {
    }
  }
}
