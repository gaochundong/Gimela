using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using Gimela.ServiceModel.ChannelManagement.Channels;
using Gimela.ServiceModel.ChannelManagement.ServiceIdentity;
using Gimela.ServiceModel.ManagedDiscovery;

namespace Gimela.ServiceModel.ChannelManagement.Factories
{
  internal static class ProxyChannelFactory
  {
    public static IProxyChannel<TContractType> CreateProxyChannel<TContractType>(string hostName) where TContractType : class
    {
      IProxyChannel<TContractType> factory = CreateProxyChannel<TContractType>(null, hostName);
      return factory;
    }

    public static IList<IProxyChannel<TContractType>> CreateProxyChannels<TContractType>() where TContractType : class
    {
      List<IProxyChannel<TContractType>> list = null;

      IList<ServiceEndpoint> endpoints = EndpointProvider.GetEndpoints<TContractType>();
      if (endpoints != null && endpoints.Count > 0)
      {
        list = new List<IProxyChannel<TContractType>>();
        foreach (ServiceEndpoint endpoint in endpoints)
        {
          list.Add(new ProxyChannel<TContractType>(endpoint, null));
        }
      }

      return list;
    }

    public static IProxyChannel<TContractType> CreateDuplexProxyChannel<TContractType>(InstanceContext context) where TContractType : class
    {
      IProxyChannel<TContractType> factory = CreateProxyDuplexChannel<TContractType>(context, null, null);
      return factory;
    }

    public static void CloseChannel(ICommunicationObject channel)
    {
      try
      {
        if (channel != null && channel.State != CommunicationState.Closed && channel.State != CommunicationState.Faulted)
          channel.Close(TimeSpan.FromMilliseconds(1));
        else if (channel != null && channel.State == CommunicationState.Faulted)
          channel.Abort();
      }
      catch (TimeoutException)
      {
        channel.Abort();
      }
      catch (CommunicationException)
      {
        channel.Abort();
      }
      finally
      {
        channel = null;
      }
    }

    #region Private Methods

    private static IProxyChannel<TContractType> CreateProxyChannel<TContractType>(CustomizedMessageHeaderData pHeaderData, string hostName) where TContractType : class
    {
      ServiceEndpoint endpoint = EndpointProvider.GetEndpoint<TContractType>(hostName);
      IProxyChannel<TContractType> channel = new ProxyChannel<TContractType>(endpoint, pHeaderData);
      return channel;
    }

    private static IProxyChannel<TContractType> CreateProxyDuplexChannel<TContractType>(InstanceContext context, CustomizedMessageHeaderData pHeaderData, string hostName) where TContractType : class
    {
      ServiceEndpoint endpoint = EndpointProvider.GetEndpoint<TContractType>(hostName);
      IProxyChannel<TContractType> factory = new ProxyDuplexChannel<TContractType>(context, endpoint, pHeaderData);
      return factory;
    }

    #endregion
  }
}
