using System;

namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 消息发送器接口，允许对象间通过消息交互。
  /// </summary>
  public interface IMessenger
  {
    /// <summary>
    /// 注册一个TMessage类型的消息接收者。处理函数将在收到相应的消息时被调用。
    /// <para>注册接收者使用弱引用，如果接收者被删除不会引起内存泄漏。</para>
    /// </summary>
    /// <typeparam name="TMessage">接收者注册接收的消息类型</typeparam>
    /// <param name="recipient">注册的将要收到消息的接收者</param>
    /// <param name="action">处理函数将在收到相应的消息时被调用</param>
    IMessenger Register<TMessage>(object recipient, Action<TMessage> action);

    /// <summary>
    /// 注册一个TMessage类型的消息接收者。处理函数将在收到相应的消息时被调用。
    /// <para>注册接收者使用弱引用，如果接收者被删除不会引起内存泄漏。</para>
    /// </summary>
    /// <typeparam name="TMessage">接收者注册接收的消息类型</typeparam>
    /// <param name="recipient">处理函数将在收到相应的消息时被调用</param>
    /// <param name="token">如果接收者使用令牌注册，而发送者发送消息并附带相同令牌，则接收者方可接收到该消息。其他未使用相同令牌注册的接收者将不能收到消息。</param>
    /// <param name="action">处理函数将在收到相应的消息时被调用</param>
    IMessenger Register<TMessage>(object recipient, object token, Action<TMessage> action);

    /// <summary>
    /// 注册一个TMessage类型的消息接收者。处理函数将在收到相应的消息时被调用。
    /// <para>注册接收者使用弱引用，如果接收者被删除不会引起内存泄漏。</para>
    /// </summary>
    /// <typeparam name="TMessage">接收者注册接收的消息类型</typeparam>
    /// <param name="recipient">处理函数将在收到相应的消息时被调用</param>
    /// <param name="receiveDerivedMessagesToo">如果为真，则衍生自TMessage类型的子类消息也将被接收者接收。TMessage也可为接口类型。</param>
    /// <param name="action">处理函数将在收到相应的消息时被调用</param>
    IMessenger Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action);

    /// <summary>
    /// 注册一个TMessage类型的消息接收者。处理函数将在收到相应的消息时被调用。
    /// <para>注册接收者使用弱引用，如果接收者被删除不会引起内存泄漏。</para>
    /// </summary>
    /// <typeparam name="TMessage">接收者注册接收的消息类型</typeparam>
    /// <param name="recipient">处理函数将在收到相应的消息时被调用</param>
    /// <param name="token">如果接收者使用令牌注册，而发送者发送消息并附带相同令牌，则接收者方可接收到该消息。其他未使用相同令牌注册的接收者将不能收到消息。</param>
    /// <param name="receiveDerivedMessagesToo">如果为真，则衍生自TMessage类型的子类消息也将被接收者接收。TMessage也可为接口类型。</param>
    /// <param name="action">处理函数将在收到相应的消息时被调用</param>
    IMessenger Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action);

    /// <summary>
    /// 发送消息至消息接收者。该消息将被发送至所有注册接收该消息类型的消息接收者。
    /// </summary>
    /// <typeparam name="TMessage">将被发送的消息的类型</typeparam>
    /// <param name="message">被发送的消息</param>
    IMessenger Send<TMessage>(TMessage message);

    /// <summary>
    /// 发送消息至消息接收者。该消息将被发送至所有注册接收该消息类型的消息TTarget类型的接收者。
    /// </summary>
    /// <typeparam name="TMessage">将被发送的消息的类型</typeparam>
    /// <typeparam name="TTarget">指定消息接收者的类型，消息将不被发送至其他类型的接收者。</typeparam>
    /// <param name="message">被发送的消息</param>
    IMessenger Send<TMessage, TTarget>(TMessage message);

    /// <summary>
    /// 发送消息至消息接收者。该消息将被发送至所有注册接收该消息类型的消息接收者。
    /// </summary>
    /// <typeparam name="TMessage">将被发送的消息的类型</typeparam>
    /// <param name="message">被发送的消息</param>
    /// <param name="token">如果接收者使用令牌注册，而发送者发送消息并附带相同令牌，则接收者方可接收到该消息。其他未使用相同令牌注册的接收者将不能收到消息。</param>
    IMessenger Send<TMessage>(TMessage message, object token);

    /// <summary>
    /// 取消消息接收者的注册。该操作完成后，该消息接收者将不能接收到任何消息。
    /// </summary>
    /// <param name="recipient">需要取消注册的消息接收者</param>
    IMessenger Unregister(object recipient);

    /// <summary>
    /// 取消消息接收者对指定TMessage类型消息的注册。该操作完成后，该消息接收者将不能接收到TMessage类型的消息，但仍然能收到其他类型消息。
    /// </summary>
    /// <typeparam name="TMessage">消息接收者将要取消注册的TMessage消息类型</typeparam>
    /// <param name="recipient">需要取消注册的消息接收者</param>
    IMessenger Unregister<TMessage>(object recipient);

    /// <summary>
    /// 取消消息接收者对指定TMessage类型和指定Token令牌的消息的注册。
    /// </summary>
    /// <typeparam name="TMessage">消息接收者将要取消注册的TMessage消息类型</typeparam>
    /// <param name="recipient">需要取消注册的消息接收者</param>
    /// <param name="token">接收者需要取消注册的消息令牌</param>
    IMessenger Unregister<TMessage>(object recipient, object token);

    /// <summary>
    /// 取消消息接收者对指定TMessage类型和指定Action处理函数的消息的注册。
    /// </summary>
    /// <typeparam name="TMessage">消息接收者将要取消注册的TMessage消息类型</typeparam>
    /// <param name="recipient">需要取消注册的消息接收者</param>
    /// <param name="action">接收者需要取消注册的处理函数</param>
    IMessenger Unregister<TMessage>(object recipient, Action<TMessage> action);

    /// <summary>
    /// 取消消息接收者对指定TMessage类型、指定Token令牌和指定Action处理函数的消息的注册。
    /// </summary>
    /// <typeparam name="TMessage">消息接收者将要取消注册的TMessage消息类型</typeparam>
    /// <param name="recipient">需要取消注册的消息接收者</param>
    /// <param name="token">接收者需要取消注册的消息令牌</param>
    /// <param name="action">接收者需要取消注册的处理函数</param>
    IMessenger Unregister<TMessage>(object recipient, object token, Action<TMessage> action);
  }
}
