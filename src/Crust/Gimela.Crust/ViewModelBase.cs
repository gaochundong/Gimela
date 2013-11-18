using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Gimela.Infrastructure.Messaging;

namespace Gimela.Crust
{
  /// <summary>
  /// 在MVVM模式中的ViewModel基类。
  /// </summary>
  public abstract class ViewModelBase : ObservableObject, IViewModel
  {
    private IMessenger _messengerInstance;

    /// <summary>
    /// 在MVVM模式中的ViewModel基类。
    /// </summary>
    public ViewModelBase()
      : this(null)
    {
    }

    /// <summary>
    /// 在MVVM模式中的ViewModel基类。
    /// </summary>
    /// <param name="messenger">消息发送器实例，用于广播消息到其他对象中。如果为NULL，则将使用Messenger的默认实例。</param>
    public ViewModelBase(IMessenger messenger)
    {
      MessengerInstance = messenger;
    }

    /// <summary>
    /// 获取或设置消息发送器实例，用于广播消息到其他对象中。
    /// 如果为NULL，则将使用Messenger的默认实例。
    /// </summary>
    protected IMessenger MessengerInstance
    {
      get
      {
        return _messengerInstance ?? Messenger.Default;
      }
      set
      {
        _messengerInstance = value;
      }
    }

    /// <summary>
    /// 清理实例。如取消注册该实例对消息发送器的订阅。
    /// </summary>
    public virtual void Cleanup()
    {
      Messenger.Default.Unregister(this);
    }

    /// <summary>
    /// 使用消息发送器广播一个PropertyChangedMessage消息。
    /// </summary>
    /// <typeparam name="T">属性值变化的属性类型</typeparam>
    /// <param name="propertyName">属性名称</param>
    /// <param name="oldValue">属性变化前的值</param>
    /// <param name="newValue">属性变化后的值</param>
    protected virtual void Broadcast<T>(string propertyName, T oldValue, T newValue)
    {
      var message = new PropertyChangedMessage<T>(this, oldValue, newValue, propertyName);
      MessengerInstance.Send(message);
    }

    /// <summary>
    /// 触发PropertyChanged事件通知。
    /// 如果需要通知到其他对象中，可通过广播一个PropertyChangedMessage消息来完成。
    /// </summary>
    /// <typeparam name="T">属性值变化的属性类型</typeparam>
    /// <param name="propertyName">属性名称</param>
    /// <param name="oldValue">属性变化前的值</param>
    /// <param name="newValue">属性变化后的值</param>
    /// <param name="broadcast">如果为真，则消息PropertyChangedMessage将被广播至其他对象中。</param>
    protected virtual void RaisePropertyChanged<T>(string propertyName, T oldValue, T newValue, bool broadcast)
    {
      RaisePropertyChanged(propertyName);

      if (broadcast)
      {
        Broadcast<T>(propertyName, oldValue, newValue);
      }
    }

    /// <summary>
    /// 触发PropertyChanged事件通知。
    /// 如果需要通知到其他对象中，可通过广播一个PropertyChangedMessage消息来完成。
    /// </summary>
    /// <typeparam name="T">属性值变化的属性类型</typeparam>
    /// <param name="propertyExpression">属性值变化的属性表达式</param>
    /// <param name="oldValue">属性变化前的值</param>
    /// <param name="newValue">属性变化后的值</param>
    /// <param name="broadcast">如果为真，则消息PropertyChangedMessage将被广播至其他对象中。</param>
    protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression, T oldValue, T newValue, bool broadcast)
    {
      if (propertyExpression == null)
      {
        return;
      }

      var handler = PropertyChangedHandler;
      if (handler != null || broadcast)
      {
        var body = propertyExpression.Body as MemberExpression;
        var expression = body.Expression as ConstantExpression;

        if (handler != null)
        {
          handler(expression.Value, new PropertyChangedEventArgs(body.Member.Name));
        }

        if (broadcast)
        {
          Broadcast<T>(body.Member.Name, oldValue, newValue);
        }
      }
    }
  }
}
