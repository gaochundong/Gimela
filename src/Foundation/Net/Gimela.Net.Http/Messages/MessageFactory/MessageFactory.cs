using System;
using System.Net;
using Gimela.Net.Http.Headers;
using Gimela.Net.Http.Messages.Parsers;
using Gimela.Infrastructure.Patterns;

namespace Gimela.Net.Http.Messages
{
  /// <summary>
  /// Parses and builds messages
  /// </summary>
  /// <remarks>
  /// <para>The message factory takes care of building messages
  /// from all end points.</para>
  /// <para>
  /// Since both message and packet protocols are used, the factory 
  /// hands out contexts to all end points. The context keeps a state
  /// to be able to parse partial messages properly.
  /// </para>
  /// <para>
  /// Each end point need to hand the context back to the message factory
  /// when the client disconnects (or a message have been parsed).
  /// </para>
  /// </remarks>
  public class MessageFactory
  {
    private readonly FlyweightObjectPool<MessageFactoryContext> _builders;
    private readonly HeaderFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageFactory"/> class.
    /// </summary>
    /// <param name="factory">Factory used to create headers.</param>
    public MessageFactory(HeaderFactory factory)
    {
      _factory = factory;
      _builders = new FlyweightObjectPool<MessageFactoryContext>(CreateBuilder);
    }

    private MessageFactoryContext CreateBuilder()
    {
      var mb = new MessageFactoryContext(this, _factory, new MessageParser());
      mb.RequestCompleted += OnRequest;
      mb.ResponseCompleted += OnResponse;
      return mb;
    }

    /// <summary>
    /// Create a new message factory context.
    /// </summary>
    /// <returns>A new context.</returns>
    /// <remarks>
    /// A context is used to parse messages from a specific endpoint.
    /// </remarks>
    internal MessageFactoryContext CreateNewContext()
    {
      return _builders.Dequeue();
    }

    internal IRequest CreateRequest(string method, string uri, string version)
    {
      return new Request(method, uri, version);
    }

    internal IResponse CreateResponse(string version, HttpStatusCode statusCode, string reason)
    {
      return new Response(version, statusCode, reason);
    }

    private void OnRequest(object sender, FactoryRequestEventArgs e)
    {
      RequestReceived(this, e);
    }

    private void OnResponse(object sender, FactoryResponseEventArgs e)
    {
      ResponseReceived(this, e);
    }

    /// <summary>
    /// Release a used factory context.
    /// </summary>
    /// <param name="factoryContext"></param>
    internal void Release(MessageFactoryContext factoryContext)
    {
      _builders.Enqueue(factoryContext);
    }

    /// <summary>
    /// A request have been received from one of the end points.
    /// </summary>
    public event EventHandler<FactoryRequestEventArgs> RequestReceived = delegate { };

    /// <summary>
    /// A response have been received from one of the end points.
    /// </summary>
    public event EventHandler<FactoryResponseEventArgs> ResponseReceived = delegate { };
  }
}