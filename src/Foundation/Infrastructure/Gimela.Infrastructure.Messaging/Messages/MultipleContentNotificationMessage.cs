
namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 通知消息
  /// </summary>
  public class MultipleContentNotificationMessage<TFirst, TSecond> : NotificationMessage
  {
    /// <summary>
    /// 通知消息
    /// </summary>
    /// <param name="notification">通知</param>
    public MultipleContentNotificationMessage(string notification)
      : base(notification)
    {
    }

    /// <summary>
    /// 通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="notification">通知</param>
    public MultipleContentNotificationMessage(object sender, string notification)
      : base(sender, notification)
    {
    }

    /// <summary>
    /// 通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="target">指定的接收者</param>
    /// <param name="notification">通知</param>
    public MultipleContentNotificationMessage(object sender, object target, string notification)
      : base(sender, target, notification)
    {
    }

    /// <summary>
    /// 获取或设置第一个内容对象
    /// </summary>
    public TFirst FirstContent { get; set; }

    /// <summary>
    /// 获取或设置第二个内容对象
    /// </summary>
    public TSecond SecondContent { get; set; }
  }
}
