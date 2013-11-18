
namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 消息基类。可通过扩展该类来创建自定义的消息类型。
  /// </summary>
  public abstract class MessageBase : IMessage
  {
    /// <summary>
    /// 消息基类
    /// </summary>
    protected MessageBase()
    {
    }

    /// <summary>
    /// 消息基类
    /// </summary>
    /// <param name="sender">消息的发送者</param>
    protected MessageBase(object sender)
    {
      Sender = sender;
    }

    /// <summary>
    /// 消息基类
    /// </summary>
    /// <param name="sender">消息的发送者</param>
    /// <param name="target">指定的消息接收者</param>
    protected MessageBase(object sender, object target)
      : this(sender)
    {
      Target = target;
    }

    /// <summary>
    /// 获取或设置消息的发送者
    /// </summary>
    public object Sender { get; protected set; }

    /// <summary>
    /// 获取或设置消息的接收者
    /// </summary>
    public object Target { get; protected set; }
  }
}
