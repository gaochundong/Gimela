
namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 通知消息
  /// </summary>
  public class NotificationMessage : NotificationMessage<string>
  {
    /// <summary>
    /// 通知消息
    /// </summary>
    /// <param name="notification">通知</param>
    public NotificationMessage(string notification)
      : base(null, null, notification, notification)
    {
    }

    /// <summary>
    /// 通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="notification">通知</param>
    public NotificationMessage(object sender, string notification)
      : base(sender, null, notification, notification)
    {
    }

    /// <summary>
    /// 通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="target">指定的接收者</param>
    /// <param name="notification">通知</param>
    public NotificationMessage(object sender, object target, string notification)
      : base(sender, target, notification, notification)
    {
    }
  }
}
