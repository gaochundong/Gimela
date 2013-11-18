using System;
using System.Collections.Generic;
using System.Linq;
using Gimela.Infrastructure.Patterns;

namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 消息发送器，允许对象间通过消息交互。
  /// </summary>
  public class Messenger : IMessenger
  {
    #region Fields
    
    private static Messenger _defaultInstance;
    private readonly object _registerLock = new object();
    /// <summary>
    /// 接收者指定的类型注册回调
    /// </summary>
    private Dictionary<Type, List<WeakActionAndToken>> _recipientsStrictAction;
    /// <summary>
    /// 接收者的衍生类注册回调
    /// </summary>
    private Dictionary<Type, List<WeakActionAndToken>> _recipientsOfSubclassesAction;

    #endregion

    #region Properties
    
    /// <summary>
    /// 获取消息发送器的默认实例
    /// </summary>
    public static Messenger Default
    {
      get
      {
        return _defaultInstance ?? (_defaultInstance = new Messenger());
      }
    }

    #endregion

    #region IMessenger Members

    /// <summary>
    /// 注册一个TMessage类型的消息接收者。处理函数将在收到相应的消息时被调用。
    /// <para>注册接收者使用弱引用，如果接收者被删除不会引起内存泄漏。</para>
    /// </summary>
    /// <typeparam name="TMessage">接收者注册接收的消息类型</typeparam>
    /// <param name="recipient">注册的将要收到消息的接收者</param>
    /// <param name="action">处理函数将在收到相应的消息时被调用</param>
    public virtual IMessenger Register<TMessage>(object recipient, Action<TMessage> action)
    {
      return Register(recipient, null, false, action);
    }

    /// <summary>
    /// 注册一个TMessage类型的消息接收者。处理函数将在收到相应的消息时被调用。
    /// <para>注册接收者使用弱引用，如果接收者被删除不会引起内存泄漏。</para>
    /// </summary>
    /// <typeparam name="TMessage">接收者注册接收的消息类型</typeparam>
    /// <param name="recipient">处理函数将在收到相应的消息时被调用</param>
    /// <param name="token">如果接收者使用令牌注册，而发送者发送消息并附带相同令牌，则接收者方可接收到该消息。其他未使用相同令牌注册的接收者将不能收到消息。</param>
    /// <param name="action">处理函数将在收到相应的消息时被调用</param>
    public virtual IMessenger Register<TMessage>(object recipient, object token, Action<TMessage> action)
    {
      return Register(recipient, token, false, action);
    }

    /// <summary>
    /// 注册一个TMessage类型的消息接收者。处理函数将在收到相应的消息时被调用。
    /// <para>注册接收者使用弱引用，如果接收者被删除不会引起内存泄漏。</para>
    /// </summary>
    /// <typeparam name="TMessage">接收者注册接收的消息类型</typeparam>
    /// <param name="recipient">处理函数将在收到相应的消息时被调用</param>
    /// <param name="receiveDerivedMessagesToo">如果为真，则衍生自TMessage类型的子类消息也将被接收者接收。TMessage也可为接口类型。</param>
    /// <param name="action">处理函数将在收到相应的消息时被调用</param>
    public virtual IMessenger Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action)
    {
      return Register(recipient, null, receiveDerivedMessagesToo, action);
    }

    /// <summary>
    /// 注册一个TMessage类型的消息接收者。处理函数将在收到相应的消息时被调用。
    /// <para>注册接收者使用弱引用，如果接收者被删除不会引起内存泄漏。</para>
    /// </summary>
    /// <typeparam name="TMessage">接收者注册接收的消息类型</typeparam>
    /// <param name="recipient">处理函数将在收到相应的消息时被调用</param>
    /// <param name="token">如果接收者使用令牌注册，而发送者发送消息并附带相同令牌，则接收者方可接收到该消息。其他未使用相同令牌注册的接收者将不能收到消息。</param>
    /// <param name="receiveDerivedMessagesToo">如果为真，则衍生自TMessage类型的子类消息也将被接收者接收。TMessage也可为接口类型。</param>
    /// <param name="action">处理函数将在收到相应的消息时被调用</param>
    public virtual IMessenger Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action)
    {
      lock (_registerLock)
      {
        Type messageType = typeof(TMessage);

        Dictionary<Type, List<WeakActionAndToken>> recipients;

        // 构造回调注册类型对象集合
        if (receiveDerivedMessagesToo)
        {
          if (_recipientsOfSubclassesAction == null)
          {
            _recipientsOfSubclassesAction = new Dictionary<Type, List<WeakActionAndToken>>();
          }

          recipients = _recipientsOfSubclassesAction;
        }
        else
        {
          if (_recipientsStrictAction == null)
          {
            _recipientsStrictAction = new Dictionary<Type, List<WeakActionAndToken>>();
          }

          recipients = _recipientsStrictAction;
        }

        // 添加键值对
        List<WeakActionAndToken> list;

        if (!recipients.ContainsKey(messageType))
        {
          list = new List<WeakActionAndToken>();
          recipients.Add(messageType, list);
        }
        else
        {
          list = recipients[messageType];
        }

        list.Add(new WeakActionAndToken()
        {
          Token = token,
          Action = new WeakAction<TMessage>(action)          
        });
      }

      Cleanup();

      return this;
    }

    /// <summary>
    /// 发送消息至消息接收者。该消息将被发送至所有注册接收该消息类型的消息接收者。
    /// </summary>
    /// <typeparam name="TMessage">将被发送的消息的类型</typeparam>
    /// <param name="message">被发送的消息</param>
    public virtual IMessenger Send<TMessage>(TMessage message)
    {
      SendToTargetOrType(message, null, null);
      return this;
    }

    /// <summary>
    /// 发送消息至消息接收者。该消息将被发送至所有注册接收该消息类型的消息TTarget类型的接收者。
    /// </summary>
    /// <typeparam name="TMessage">将被发送的消息的类型</typeparam>
    /// <typeparam name="TTarget">指定消息接收者的类型，消息将不被发送至其他类型的接收者。</typeparam>
    /// <param name="message">被发送的消息</param>
    public virtual IMessenger Send<TMessage, TTarget>(TMessage message)
    {
      SendToTargetOrType(message, typeof(TTarget), null);
      return this;
    }

    /// <summary>
    /// 发送消息至消息接收者。该消息将被发送至所有注册接收该消息类型的消息接收者。
    /// </summary>
    /// <typeparam name="TMessage">将被发送的消息的类型</typeparam>
    /// <param name="message">被发送的消息</param>
    /// <param name="token">如果接收者使用令牌注册，而发送者发送消息并附带相同令牌，则接收者方可接收到该消息。其他未使用相同令牌注册的接收者将不能收到消息。</param>
    public virtual IMessenger Send<TMessage>(TMessage message, object token)
    {
      SendToTargetOrType(message, null, token);
      return this;
    }

    /// <summary>
    /// 取消消息接收者的注册。该操作完成后，该消息接收者将不能接收到任何消息。
    /// </summary>
    /// <param name="recipient">需要取消注册的消息接收者</param>
    public virtual IMessenger Unregister(object recipient)
    {
      UnregisterFromLists(recipient, _recipientsOfSubclassesAction);
      UnregisterFromLists(recipient, _recipientsStrictAction);
      return this;
    }

    /// <summary>
    /// 取消消息接收者对指定TMessage类型消息的注册。该操作完成后，该消息接收者将不能接收到TMessage类型的消息，但仍然能收到其他类型消息。
    /// </summary>
    /// <typeparam name="TMessage">消息接收者将要取消注册的TMessage消息类型</typeparam>
    /// <param name="recipient">需要取消注册的消息接收者</param>
    public virtual IMessenger Unregister<TMessage>(object recipient)
    {
      return Unregister<TMessage>(recipient, null, null);
    }

    /// <summary>
    /// 取消消息接收者对指定TMessage类型和指定Token令牌的消息的注册。
    /// </summary>
    /// <typeparam name="TMessage">消息接收者将要取消注册的TMessage消息类型</typeparam>
    /// <param name="recipient">需要取消注册的消息接收者</param>
    /// <param name="token">接收者需要取消注册的消息令牌</param>
    public virtual IMessenger Unregister<TMessage>(object recipient, object token)
    {
      return Unregister<TMessage>(recipient, token, null);
    }

    /// <summary>
    /// 取消消息接收者对指定TMessage类型和指定Action处理函数的消息的注册。
    /// </summary>
    /// <typeparam name="TMessage">消息接收者将要取消注册的TMessage消息类型</typeparam>
    /// <param name="recipient">需要取消注册的消息接收者</param>
    /// <param name="action">接收者需要取消注册的处理函数</param>
    public virtual IMessenger Unregister<TMessage>(object recipient, Action<TMessage> action)
    {
      return Unregister(recipient, null, action);
    }

    /// <summary>
    /// 取消消息接收者对指定TMessage类型、指定Token令牌和指定Action处理函数的消息的注册。
    /// </summary>
    /// <typeparam name="TMessage">消息接收者将要取消注册的TMessage消息类型</typeparam>
    /// <param name="recipient">需要取消注册的消息接收者</param>
    /// <param name="token">接收者需要取消注册的消息令牌</param>
    /// <param name="action">接收者需要取消注册的处理函数</param>
    public virtual IMessenger Unregister<TMessage>(object recipient, object token, Action<TMessage> action)
    {
      UnregisterFromLists(recipient, token, action, _recipientsStrictAction);
      UnregisterFromLists(recipient, token, action, _recipientsOfSubclassesAction);
      Cleanup();
      return this;
    }

    #endregion

    #region Public Methods
    
    /// <summary>
    /// 使用指定的Messenger替换默认Messenger实例
    /// </summary>
    /// <param name="newMessenger">使用指定的Messenger替换默认Messenger实例</param>
    public static void OverrideDefault(Messenger newMessenger)
    {
      _defaultInstance = newMessenger;
    }

    /// <summary>
    /// 重置默认Messenger实例
    /// </summary>
    public static void Reset()
    {
      _defaultInstance = null;
    }

    #endregion

    #region Private Methods

    private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>> lists)
    {
      if (lists == null)
      {
        return;
      }

      lock (lists)
      {
        var listsToRemove = new List<Type>();
        foreach (var list in lists)
        {
          // 清除不活跃的对象
          var recipientsToRemove = new List<WeakActionAndToken>();
          foreach (WeakActionAndToken item in list.Value)
          {
            if (item.Action == null || !item.Action.IsAlive)
            {
              recipientsToRemove.Add(item);
            }
          }

          foreach (WeakActionAndToken recipient in recipientsToRemove)
          {
            list.Value.Remove(recipient);
          }

          if (list.Value.Count == 0)
          {
            listsToRemove.Add(list.Key);
          }
        }

        foreach (Type key in listsToRemove)
        {
          lists.Remove(key);
        }
      }
    }

    private static bool Implements(Type instanceType, Type interfaceType)
    {
      if (interfaceType == null || instanceType == null)
      {
        return false;
      }

      Type[] interfaces = instanceType.GetInterfaces();
      foreach (Type currentInterface in interfaces)
      {
        if (currentInterface == interfaceType)
        {
          return true;
        }
      }

      return false;
    }

    private static void SendToList<TMessage>(TMessage message, IEnumerable<WeakActionAndToken> list, Type messageTargetType, object token)
    {
      if (list != null)
      {
        List<WeakActionAndToken> listClone = list.Take(list.Count()).ToList();

        foreach (WeakActionAndToken item in listClone)
        {
          var executeAction = item.Action as IWeakActionExecuteWithObject;

          if (executeAction != null && item.Action.IsAlive)
          {
            if (item.Action.IsStatic)
            {
              executeAction.ExecuteWithObject(message);
            }
            else if (item.Action.Target != null
              && (messageTargetType == null
                  || item.Action.Target.GetType() == messageTargetType
                  || Implements(item.Action.Target.GetType(), messageTargetType))
              && ((item.Token == null && token == null)
                  || item.Token != null && item.Token.Equals(token)))
            {
              executeAction.ExecuteWithObject(message);
            }
          }
        }
      }
    }

    private static void UnregisterFromLists(object recipient, Dictionary<Type, List<WeakActionAndToken>> lists)
    {
      if (recipient == null || lists == null || lists.Count == 0)
      {
        return;
      }

      lock (lists)
      {
        foreach (Type messageType in lists.Keys)
        {
          foreach (WeakActionAndToken item in lists[messageType])
          {
            WeakAction weakAction = item.Action;

            if (weakAction != null
                && recipient == weakAction.Target)
            {
              weakAction.Destroy();
            }
          }
        }
      }
    }

    private static void UnregisterFromLists<TMessage>(object recipient, object token, Action<TMessage> action, Dictionary<Type, List<WeakActionAndToken>> lists)
    {
      Type messageType = typeof(TMessage);

      if (recipient == null
          || lists == null
          || lists.Count == 0
          || !lists.ContainsKey(messageType))
      {
        return;
      }

      lock (lists)
      {
        foreach (WeakActionAndToken item in lists[messageType])
        {
          var weakActionCasted = item.Action as WeakAction<TMessage>;

          if (weakActionCasted != null
              && recipient == weakActionCasted.Target
              && (action == null
                  || action.Method.Name == weakActionCasted.MethodName)
              && (token == null
                  || token.Equals(item.Token)))
          {
            item.Action.Destroy();
          }
        }
      }
    }

    private void Cleanup()
    {
      CleanupList(_recipientsOfSubclassesAction);
      CleanupList(_recipientsStrictAction);
    }

    private void SendToTargetOrType<TMessage>(TMessage message, Type messageTargetType, object token)
    {
      Type messageType = typeof(TMessage);

      if (_recipientsOfSubclassesAction != null)
      {
        List<Type> listClone = _recipientsOfSubclassesAction.Keys.Take(_recipientsOfSubclassesAction.Count()).ToList();

        foreach (Type type in listClone)
        {
          List<WeakActionAndToken> list = null;

          if (messageType == type
              || messageType.IsSubclassOf(type)
              || Implements(messageType, type))
          {
            list = _recipientsOfSubclassesAction[type];
          }

          SendToList(message, list, messageTargetType, token);
        }
      }

      if (_recipientsStrictAction != null)
      {
        if (_recipientsStrictAction.ContainsKey(messageType))
        {
          List<WeakActionAndToken> list = _recipientsStrictAction[messageType];
          SendToList(message, list, messageTargetType, token);
        }
      }

      Cleanup();
    }

    #endregion
  }
}
