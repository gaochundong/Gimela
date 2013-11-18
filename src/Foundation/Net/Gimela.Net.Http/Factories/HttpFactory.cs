using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Gimela.Net.Http.Headers;
using Gimela.Net.Http.Messages;
using Gimela.Common.Logging;

namespace Gimela.Net.Http
{
  /// <summary>
  /// 用于该HTTP协议栈的依赖注入器
  /// </summary>
  /// <remarks>
  /// <para>Should have factory methods at least for the following types:
  /// <see cref="IRequest"/>, <see cref="IResponse"/>, 
  /// <see cref="HeaderFactory"/>, <see cref="MessageFactory"/>,
  /// <see cref="HttpContext"/>, <see cref="SecureHttpContext"/>,
  /// <see cref="IResponse"/>, <see cref="IRequest"/>,
  /// <see cref="ResponseWriter"/>.
  /// </para>
  /// <para>Check the default implementations to see which constructor 
  /// parameters you will get.</para>
  /// </remarks>
  /// <example>
  /// HttpFactory.Add(typeof(IRequest), (type, args) => new MyRequest((string)args[0]));
  /// </example>
  public class HttpFactory : IHttpFactory
  {
    [ThreadStatic]
    private static IHttpFactory _current;
    private readonly Dictionary<Type, FactoryMethod> _methods = new Dictionary<Type, FactoryMethod>();
    private HeaderFactory _headerFactory;
    private MessageFactory _messageFactory;

    /// <summary>
    /// 用于该HTTP协议栈的依赖注入器
    /// </summary>
    public HttpFactory()
    {
      _current = this;
      AddDefaultCreators();
    }

    /// <summary>
    /// 获取当前线程的HTTP工厂
    /// </summary>
    public static IHttpFactory Current
    {
      get { return _current; }
      set { _current = value; }
    }

    /// <summary>
    /// Add a factory method for a type.
    /// </summary>
    /// <param name="type">Type to create</param>
    /// <param name="handler">Method creating the type.</param>
    public void Add(Type type, FactoryMethod handler)
    {
      _methods[type] = handler;
    }

    private void AddDefault(Type type, FactoryMethod handler)
    {
      if (_methods.ContainsKey(type))
        return;

      _methods[type] = handler;
    }

    private void AddDefaultCreators()
    {
      AddDefault(typeof(HeaderFactory), OnSetupHeaderFactory);
      AddDefault(typeof(MessageFactory), OnSetupMessageFactory);
      AddDefault(typeof(HttpContext), CreateHttpContext);
      AddDefault(typeof(SecureHttpContext), CreateSecureHttpContext);
      AddDefault(typeof(IResponse), CreateResponse);
      AddDefault(typeof(IRequest), CreateRequest);
      AddDefault(typeof(ResponseWriter), CreateResponseGenerator);
    }

    /// <summary>
    /// Setup our singleton.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    /// <remarks>
    /// We want to use a singleton, but we also want to be able
    /// to let the developer to setup his own header factory.
    /// Therefore we use this method to create our own factory only if the user
    /// have not specified one.
    /// </remarks>
    private object OnSetupHeaderFactory(Type type, object[] arguments)
    {
      _headerFactory = new HeaderFactory();
      _headerFactory.AddDefaultParsers();
      _methods[typeof(HeaderFactory)] = (type2, args) => _headerFactory;
      return _headerFactory;
    }

    /// <summary>
    /// Small method to create a message factory singleton and replace then default delegate method.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    private object OnSetupMessageFactory(Type type, object[] arguments)
    {
      _messageFactory = new MessageFactory(Get<HeaderFactory>());
      _methods[type] = (type2, args) => _messageFactory;
      return _messageFactory;
    }

    private object CreateHttpContext(Type type, object[] arguments)
    {
      MessageFactoryContext context = Get<MessageFactory>().CreateNewContext();
      var httpContext = new HttpContext((Socket)arguments[0], context);
      httpContext.Disconnected += OnContextDisconnected;
      return httpContext;
    }

    private object CreateSecureHttpContext(Type type, object[] arguments)
    {
      MessageFactoryContext context = Get<MessageFactory>().CreateNewContext();
      var certificate = (X509Certificate)arguments[0];
      var protocols = (SslProtocols)arguments[1];
      var httpContext = new SecureHttpContext(certificate, protocols, (Socket)arguments[2], context);
      httpContext.Disconnected += OnContextDisconnected;
      return httpContext;
    }

    private object CreateRequest(Type type, object[] arguments)
    {
      return new Request((string)arguments[0], (string)arguments[1], (string)arguments[1]);
    }

    private object CreateResponse(Type type, object[] arguments)
    {
      return new Response((IHttpContext)arguments[0], (IRequest)arguments[1]);
    }

    private object CreateResponseGenerator(Type type, object[] arguments)
    {
      return new ResponseWriter();
    }

    /// <summary>
    /// Finds the factory method.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    protected virtual FactoryMethod FindFactoryMethod(Type type)
    {
      FactoryMethod method;
      if (!_methods.TryGetValue(type, out method))
      {
        Logger.Warning("Failed to find factory method for " + type.FullName);
        return null;
      }

      return method;
    }

    private void OnContextDisconnected(object sender, EventArgs e)
    {
      var context = (HttpContext)sender;
      context.Disconnected -= OnContextDisconnected;
      _messageFactory.Release(context.MessageFactoryContext);
    }

    #region IHttpFactory Members

    /// <summary>
    /// Create a type.
    /// </summary>
    /// <typeparam name="T">Type to create</typeparam>
    /// <returns>Created type.</returns>
    public T Get<T>(params object[] constructorArguments) where T : class
    {
      Type type = typeof(T);

      FactoryMethod method = FindFactoryMethod(type);
      if (method == null)
      {
        Logger.Error("No factory method is associated with '" + type.FullName + "'.");
        return null;
      }

      object createdType = method(type, constructorArguments);
      if (createdType == null)
      {
        Logger.Error("Factory method failed to create type '" + type.FullName + "'.");
        return null;
      }

      var instance = createdType as T;
      if (instance == null)
      {
        Logger.Error("Factory method assigned to '" + type.FullName + "' created a incompatible type '" +
                      createdType.GetType().FullName);
        return null;
      }

      return instance;
    }

    #endregion
  }
}