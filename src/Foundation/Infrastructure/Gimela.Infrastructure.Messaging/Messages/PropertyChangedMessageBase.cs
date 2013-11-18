
namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 该类为PropertyChangedMessage泛型类的基类。该类允许一个接收者注册PropertyChangedMessage消息而不必指定T类型。
  /// </summary>
  public abstract class PropertyChangedMessageBase : MessageBase
  {
    /// <summary>
    /// 该类为PropertyChangedMessage泛型类的基类
    /// </summary>
    /// <param name="sender">消息的发送者</param>
    /// <param name="propertyName">属性值更改的属性名称</param>
    protected PropertyChangedMessageBase(object sender, string propertyName)
      : base(sender)
    {
      PropertyName = propertyName;
    }

    /// <summary>
    /// 该类为PropertyChangedMessage泛型类的基类
    /// </summary>
    /// <param name="sender">消息的发送者</param>
    /// <param name="target">指定的消息接收者</param>
    /// <param name="propertyName">属性值更改的属性名称</param>
    protected PropertyChangedMessageBase(object sender, object target, string propertyName)
      : base(sender, target)
    {
      PropertyName = propertyName;
    }

    /// <summary>
    /// 该类为PropertyChangedMessage泛型类的基类
    /// </summary>
    /// <param name="propertyName">属性值更改的属性名称</param>
    protected PropertyChangedMessageBase(string propertyName)
    {
      PropertyName = propertyName;
    }

    /// <summary>
    /// 获取或设置属性值更改的属性名称
    /// </summary>
    public string PropertyName { get; protected set; }
  }
}
