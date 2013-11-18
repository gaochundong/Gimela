using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using Gimela.Infrastructure.ResourceLocation;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 终结点提供器
  /// </summary>
  public static class EndpointProvider
  {
    /// <summary>
    /// 获取指定服务契约类型的终结点
    /// </summary>
    /// <typeparam name="TContractType">服务契约类型</typeparam>
    /// <param name="hostName">主机名称</param>
    /// <returns>指定服务契约类型的终结点</returns>
    public static ServiceEndpoint GetEndpoint<TContractType>(string hostName) where TContractType : class
    {
      return GetEndpoint(typeof(TContractType), hostName);
    }

    /// <summary>
    /// 获取指定服务契约类型的终结点
    /// </summary>
    /// <param name="contractType">服务契约类型</param>
    /// <param name="hostName">主机名称</param>
    /// <returns>指定服务契约类型的终结点</returns>
    public static ServiceEndpoint GetEndpoint(Type contractType, string hostName)
    {
      foreach (EndpointDiscoveryMetadata metadata in Locator.Get<EndpointDiscoveryMetadataCollection>())
      {
        if (string.IsNullOrEmpty(hostName))
        {
          if (metadata.GetSpecifiedName().Split('#')[0] == contractType.FullName)
          {
            ServiceEndpoint endpoint = new ServiceEndpoint(ContractDescription.GetContract(contractType), new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName), metadata.Address);
            endpoint.Name = contractType.Name;
            return endpoint;
          }
        }
        else
        {
          if (metadata.GetSpecifiedName().Split('#')[0] == contractType.FullName && metadata.GetSpecifiedName().Split('#')[1] == hostName)
          {
            ServiceEndpoint endpoint = new ServiceEndpoint(ContractDescription.GetContract(contractType), new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName), metadata.Address);
            endpoint.Name = contractType.Name;
            return endpoint;
          }
        }
      }

      throw new ContractNotFoundException("Cannot find contract type : " + contractType.FullName);
    }

    /// <summary>
    /// 获取指定服务契约类型的所有终结点
    /// </summary>
    /// <typeparam name="TContractType">服务契约类型</typeparam>
    /// <returns>指定服务契约类型的所有终结点</returns>
    public static IList<ServiceEndpoint> GetEndpoints<TContractType>() where TContractType : class
    {
      return GetEndpoints(typeof(TContractType));
    }

    /// <summary>
    /// 获取指定服务契约类型的所有终结点
    /// </summary>
    /// <param name="contractType">服务契约类型</param>
    /// <returns>指定服务契约类型的所有终结点</returns>
    public static IList<ServiceEndpoint> GetEndpoints(Type contractType)
    {
      List<ServiceEndpoint> endpoints = new List<ServiceEndpoint>();

      foreach (EndpointDiscoveryMetadata metadata in Locator.Get<EndpointDiscoveryMetadataCollection>())
      {
        if (metadata.GetSpecifiedName().Split('#')[0] == contractType.FullName)
        {
          ServiceEndpoint endpoint = new ServiceEndpoint(ContractDescription.GetContract(contractType), new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName), metadata.Address);
          endpoint.Name = contractType.Name;
          endpoints.Add(endpoint);
        }
      }

      return endpoints;
    }
  }
}
