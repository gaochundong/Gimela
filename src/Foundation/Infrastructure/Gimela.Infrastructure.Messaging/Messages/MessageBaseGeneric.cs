
namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 消息基类泛型
  /// </summary>
  /// <typeparam name="T">消息内容类型</typeparam>
  public abstract class MessageBase<T> : MessageBase
  {
    /// <summary>
    /// 消息基类泛型
    /// </summary>
    /// <param name="content">消息的内容</param>
    protected MessageBase(T content)
    {
      Content = content;
    }

    /// <summary>
    /// 消息基类泛型
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="content">消息的内容</param>
    protected MessageBase(object sender, T content)
      : base(sender)
    {
      Content = content;
    }

    /// <summary>
    /// 消息基类泛型
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="target">指定的消息接收者</param>
    /// <param name="content">消息的内容</param>
    protected MessageBase(object sender, object target, T content)
      : base(sender, target)
    {
      Content = content;
    }

    /// <summary>
    /// 获取或设置消息的内容，内容类型由泛型指定。
    /// </summary>
    public T Content { get; protected set; }
  }
}
