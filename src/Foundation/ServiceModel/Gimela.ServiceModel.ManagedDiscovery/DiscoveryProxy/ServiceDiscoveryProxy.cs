using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 服务发现代理
  /// </summary>
  /// <typeparam name="T">指定类型的服务</typeparam>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class ServiceDiscoveryProxy<T> : DiscoveryProxy where T : class
  {
    private EndpointDiscoveryMetadataCollection cache;

    /// <summary>
    /// 服务发现代理
    /// </summary>
    /// <param name="cache">服务缓存</param>
    public ServiceDiscoveryProxy(EndpointDiscoveryMetadataCollection cache)
      : base()
    {
      if (cache == null)
        throw new ArgumentNullException("cache");
      this.cache = cache;
    }

    /// <summary>
    /// Override this method to handle an online announcement message.
    /// </summary>
    /// <param name="messageSequence">The discovery message sequence.</param>
    /// <param name="endpointDiscoveryMetadata">The endpoint discovery metadata.</param>
    /// <param name="callback">The callback delegate to call when the operation is completed.</param>
    /// <param name="state">The user-defined state data.</param>
    /// <returns>
    /// A reference to the pending asynchronous operation.
    /// </returns>
    protected override IAsyncResult OnBeginOnlineAnnouncement(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata, AsyncCallback callback, object state)
    {
      if (endpointDiscoveryMetadata == null)
      {
        throw new ArgumentNullException("endpointDiscoveryMetadata");
      }

      FindCriteria criteria = new FindCriteria(typeof(T));

      if (criteria.IsMatch(endpointDiscoveryMetadata))
      {
        // 指定类型的服务上线
        if (!cache.Contains(endpointDiscoveryMetadata.Address.Uri))
        {
          cache.Add(endpointDiscoveryMetadata);
          Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Discovery proxy endpoint online : {0} - {1}", endpointDiscoveryMetadata.GetSpecifiedName(), endpointDiscoveryMetadata.Address.Uri));
        }
      }

      return new CompletedAsyncResult(callback, state);
    }

    /// <summary>
    /// Override this method to handle an offline announcement message.
    /// </summary>
    /// <param name="messageSequence">The discovery message sequence.</param>
    /// <param name="endpointDiscoveryMetadata">The endpoint discovery metadata.</param>
    /// <param name="callback">The callback delegate to call when the operation is completed.</param>
    /// <param name="state">The user-defined state data.</param>
    /// <returns>
    /// A reference to the pending asynchronous operation.
    /// </returns>
    protected override IAsyncResult OnBeginOfflineAnnouncement(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata, AsyncCallback callback, object state)
    {
      if (endpointDiscoveryMetadata == null)
      {
        throw new ArgumentNullException("endpointDiscoveryMetadata");
      }

      FindCriteria criteria = new FindCriteria(typeof(T));

      if (criteria.IsMatch(endpointDiscoveryMetadata))
      {
        // 指定类型的服务下线
        if (cache.Contains(endpointDiscoveryMetadata.Address.Uri))
        {
          cache.Remove(endpointDiscoveryMetadata);
          Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Discovery proxy endpoint offline : {0} - {1}", endpointDiscoveryMetadata.GetSpecifiedName(), endpointDiscoveryMetadata.Address.Uri));
        }
      }

      return new CompletedAsyncResult(callback, state);
    }

    /// <summary>
    /// Override this method to handle a find operation.
    /// </summary>
    /// <param name="findRequestContext">The find request context that describes the service to discover.</param>
    /// <param name="callback">The callback delegate to call when the operation is completed.</param>
    /// <param name="state">The user-defined state data.</param>
    /// <returns>
    /// A reference to the pending asynchronous operation.
    /// </returns>
    protected override IAsyncResult OnBeginFind(FindRequestContext findRequestContext, AsyncCallback callback, object state)
    {
      if (findRequestContext == null)
      {
        throw new ArgumentNullException("findRequestContext");
      }

      Console.WriteLine("Find request for contract {0}", findRequestContext.Criteria.ContractTypeNames.FirstOrDefault());

      // query to find the matching endpoints
      var query = from service in cache
                  where findRequestContext.Criteria.IsMatch(service)
                  select service;

      // collection to contain the results of the query
      var matchingEndpoints = new Collection<EndpointDiscoveryMetadata>();

      // execute the query and add the matching endpoints
      foreach (EndpointDiscoveryMetadata metadata in query)
      {
        matchingEndpoints.Add(metadata);
        findRequestContext.AddMatchingEndpoint(metadata);
      }

      return new FindAsyncResult(matchingEndpoints, callback, state);
    }

    /// <summary>
    /// Override this method to handle the completion of a find operation.
    /// </summary>
    /// <param name="result">A reference to the completed asynchronous operation.</param>
    protected override void OnEndFind(IAsyncResult result)
    {
      FindAsyncResult.End(result);
    }

    /// <summary>
    /// Override this method to handle a resolve operation.
    /// </summary>
    /// <param name="resolveCriteria">The resolve criteria that describes the service to discover.</param>
    /// <param name="callback">The callback delegate to call when the operation is completed.</param>
    /// <param name="state">The user-defined state data.</param>
    /// <returns>
    /// A reference to the pending asynchronous operation.
    /// </returns>
    protected override IAsyncResult OnBeginResolve(ResolveCriteria resolveCriteria, AsyncCallback callback, object state)
    {
      return new CompletedAsyncResult(callback, state);
    }

    /// <summary>
    /// Override this method to handle the completion of a resolve operation.
    /// </summary>
    /// <param name="result">A reference to the completed asynchronous operation.</param>
    /// <returns>
    /// Endpoint discovery metadata for the resolved service.
    /// </returns>
    protected override EndpointDiscoveryMetadata OnEndResolve(IAsyncResult result)
    {
      return CompletedAsyncResult<EndpointDiscoveryMetadata>.End(result);
    }

    /// <summary>
    /// Override this method to handle the completion of an offline announcement message.
    /// </summary>
    /// <param name="result">A reference to the completed asynchronous operation.</param>
    protected override void OnEndOfflineAnnouncement(IAsyncResult result)
    {
      CompletedAsyncResult.End(result);
    }

    /// <summary>
    /// Override this method to handle the completion of an online announcement message.
    /// </summary>
    /// <param name="result">A reference to the completed asynchronous operation.</param>
    protected override void OnEndOnlineAnnouncement(IAsyncResult result)
    {
      CompletedAsyncResult.End(result);
    }
  }
}
