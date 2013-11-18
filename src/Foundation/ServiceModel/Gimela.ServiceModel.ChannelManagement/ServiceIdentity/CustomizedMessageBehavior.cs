using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Gimela.ServiceModel.ChannelManagement.ServiceIdentity
{
  /// <summary>
  /// 定制化消息行为
  /// </summary>
  public class CustomizedMessageBehavior : IClientMessageInspector, IEndpointBehavior
  {
    #region Fields
    
    private CustomizedMessageHeaderData messageHeaderData = null;

    #endregion

    #region Ctors

    /// <summary>
    /// 定制化消息行为
    /// </summary>
    public CustomizedMessageBehavior()
    {
    }

    /// <summary>
    /// 定制化消息行为
    /// </summary>
    /// <param name="messageHeader">定制化消息头数据</param>
    public CustomizedMessageBehavior(CustomizedMessageHeaderData messageHeader)
    {
      messageHeaderData = messageHeader;
    }

    #endregion

    #region Properties
    
    /// <summary>
    /// 定制化消息头数据
    /// </summary>
    public CustomizedMessageHeaderData MessageHeaderData
    {
      get { return messageHeaderData; }
    }

    #endregion

    #region IClientMessageInspector Members

    /// <summary>
    /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
    /// </summary>
    /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
    /// <param name="correlationState">Correlation state data.</param>
    public void AfterReceiveReply(ref Message reply, object correlationState)
    {
    }

    /// <summary>
    /// Enables inspection or modification of a message before a request message is sent to a service.
    /// </summary>
    /// <param name="request">The message to be sent to the service.</param>
    /// <param name="channel">The  client object channel.</param>
    /// <returns>
    /// The object that is returned as the  correlationState argument of 
    /// the <see cref="M:System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)"/> 
    /// method. This is null if no correlation state is used.The best practice is to make this a <see cref="T:System.Guid"/> 
    /// to ensure that no two correlationState objects are the same.
    /// </returns>
    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
      if (MessageHeaderData == null)
      {
        // custermize the message header data
        messageHeaderData = new CustomizedMessageHeaderData();
      }

      int index = request.Headers.FindHeader(CustomizedMessageHeaderConstants.MessageHeaderDataName, CustomizedMessageHeaderConstants.MessageHeaderDataNamespace);
      if (index < 0)
      {
        // modify the message headers before the request message is sent out
        MessageHeader header = MessageHeader.CreateHeader(CustomizedMessageHeaderConstants.MessageHeaderDataName, CustomizedMessageHeaderConstants.MessageHeaderDataNamespace, MessageHeaderData);
        request.Headers.Add(header);
      }

      return null;
    }

    #endregion

    #region IEndpointBehavior Members

    /// <summary>
    /// Implement to pass data at runtime to bindings to support custom behavior.
    /// </summary>
    /// <param name="endpoint">The endpoint to modify.</param>
    /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
    }

    /// <summary>
    /// Implements a modification or extension of the client across an endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint that is to be customized.</param>
    /// <param name="clientRuntime">The client runtime to be customized.</param>
    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
      clientRuntime.MessageInspectors.Add(this);
    }

    /// <summary>
    /// Implements a modification or extension of the service across an endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint that exposes the contract.</param>
    /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
    }

    /// <summary>
    /// Implement to confirm that the endpoint meets some intended criteria.
    /// </summary>
    /// <param name="endpoint">The endpoint to validate.</param>
    public void Validate(ServiceEndpoint endpoint)
    {
    }

    #endregion
  }
}
