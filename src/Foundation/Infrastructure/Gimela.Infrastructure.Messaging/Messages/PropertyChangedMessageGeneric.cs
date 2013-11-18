
namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 属性值变化通知消息
  /// </summary>
  /// <typeparam name="T">属性类型</typeparam>
  public class PropertyChangedMessage<T> : PropertyChangedMessageBase
  {
    /// <summary>
    /// 属性值变化通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="oldValue">在属性值发生变化前的属性值</param>
    /// <param name="newValue">在属性值发生变化后的属性值</param>
    /// <param name="propertyName">属性值更改的属性名称</param>
    public PropertyChangedMessage(object sender, T oldValue, T newValue, string propertyName)
      : base(sender, propertyName)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }

    /// <summary>
    /// 属性值变化通知消息
    /// </summary>
    /// <param name="oldValue">在属性值发生变化前的属性值</param>
    /// <param name="newValue">在属性值发生变化后的属性值</param>
    /// <param name="propertyName">属性值更改的属性名称</param>
    public PropertyChangedMessage(T oldValue, T newValue, string propertyName)
      : base(propertyName)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }

    /// <summary>
    /// 属性值变化通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="target">指定的消息接收者</param>
    /// <param name="oldValue">在属性值发生变化前的属性值</param>
    /// <param name="newValue">在属性值发生变化后的属性值</param>
    /// <param name="propertyName">属性值更改的属性名称</param>
    public PropertyChangedMessage(object sender, object target, T oldValue, T newValue, string propertyName)
      : base(sender, target, propertyName)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }

    /// <summary>
    /// 获取在属性值发生变化前的属性值
    /// </summary>
    public T NewValue { get; protected set; }

    /// <summary>
    /// 获取在属性值发生变化后的属性值
    /// </summary>
    public T OldValue { get; protected set; }
  }
}
