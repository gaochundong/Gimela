using System.ServiceModel;
using System.ServiceModel.Description;

namespace Gimela.ServiceModel.ChannelManagement.ServiceIdentity
{
  /// <summary>
  /// 消息修补器，负责将定制的消息头内容填入至消息中。
  /// </summary>
  internal static class MessageFixer
  {
    /// <summary>
    /// 将定制的消息头内容填入至消息中
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="factory">服务通道创建工厂</param>
    public static void Fix<TContract>(ChannelFactory<TContract> factory) where TContract : class
    {
      FixInternal<TContract>(factory, null, null);
    }

    /// <summary>
    /// 将定制的消息头内容填入至消息中
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="factory">服务通道创建工厂</param>
    /// <param name="messageHeader">定制的消息头</param>
    public static void Fix<TContract>(ChannelFactory<TContract> factory, CustomizedMessageHeaderData messageHeader) where TContract : class
    {
      FixInternal<TContract>(factory, null, messageHeader);
    }

    /// <summary>
    /// 将定制的消息头内容填入至消息中
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="factory">服务通道创建工厂</param>
    /// <param name="endpoint">服务通道终结点</param>
    public static void Fix<TContract>(ChannelFactory<TContract> factory, ServiceEndpoint endpoint) where TContract : class
    {
      FixInternal<TContract>(factory, endpoint, null);
    }

    /// <summary>
    /// 将定制的消息头内容填入至消息中
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="factory">服务通道创建工厂</param>
    /// <param name="endpoint">服务通道终结点</param>
    /// <param name="messageHeader">定制的消息头</param>
    public static void Fix<TContract>(ChannelFactory<TContract> factory, ServiceEndpoint endpoint, CustomizedMessageHeaderData messageHeader) where TContract : class
    {
      FixInternal<TContract>(factory, endpoint, messageHeader);
    }

    /// <summary>
    /// 将定制的消息头内容填入至消息中
    /// </summary>
    /// <typeparam name="TContract">服务契约类型</typeparam>
    /// <param name="factory">服务通道创建工厂</param>
    /// <param name="endpoint">服务通道终结点</param>
    /// <param name="messageHeader">定制的消息头</param>
    private static void FixInternal<TContract>(ChannelFactory<TContract> factory, ServiceEndpoint endpoint, CustomizedMessageHeaderData messageHeader) where TContract : class
    {
      if (endpoint != null)
      {
        foreach (OperationDescription operation in factory.Endpoint.Contract.Operations)
        {
          DataContractSerializerOperationBehavior behavior;
          if (!operation.Behaviors.Contains(typeof(DataContractSerializerOperationBehavior)))
          {
            behavior = new DataContractSerializerOperationBehavior(operation);
            operation.Behaviors.Add(behavior);
          }
          else
          {
            behavior = operation.Behaviors[typeof(DataContractSerializerOperationBehavior)] as DataContractSerializerOperationBehavior;
          }

          foreach (OperationDescription originalOperation in endpoint.Contract.Operations)
          {
            if (operation.Name == originalOperation.Name)
            {
              DataContractSerializerOperationBehavior originalBehavior = originalOperation.Behaviors[typeof(DataContractSerializerOperationBehavior)] as DataContractSerializerOperationBehavior;
              if (behavior != null && originalBehavior != null)
              {
                behavior.IgnoreExtensionDataObject = originalBehavior.IgnoreExtensionDataObject;
                behavior.MaxItemsInObjectGraph = originalBehavior.MaxItemsInObjectGraph;
              }
              break;
            }
          }
        }
      }

      if (!factory.Endpoint.Behaviors.Contains(typeof(CustomizedMessageBehavior)))
      {
        if (messageHeader == null)
        {
          factory.Endpoint.Behaviors.Add(new CustomizedMessageBehavior());
        }
        else
        {
          factory.Endpoint.Behaviors.Add(new CustomizedMessageBehavior(messageHeader));
        }
      }
    }
  }
}
