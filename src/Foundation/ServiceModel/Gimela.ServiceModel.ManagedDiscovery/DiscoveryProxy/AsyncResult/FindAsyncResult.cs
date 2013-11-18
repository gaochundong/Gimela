using System;
using System.Collections.ObjectModel;
using System.ServiceModel.Discovery;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 异步查找操作状态
  /// </summary>
  public class FindAsyncResult : CompletedAsyncResult
  {
    private Collection<EndpointDiscoveryMetadata> matchingEndpoints;

    /// <summary>
    /// Initializes a new instance of the <see cref="FindAsyncResult"/> class.
    /// </summary>
    /// <param name="matchingEndpoints">The matching endpoints.</param>
    /// <param name="callback">异步回调函数</param>
    /// <param name="state">异步操作参数</param>
    public FindAsyncResult(Collection<EndpointDiscoveryMetadata> matchingEndpoints, AsyncCallback callback, object state)
      : base(callback, state)
    {
      this.matchingEndpoints = matchingEndpoints;
    }

    /// <summary>
    /// Ends the specified result.
    /// Hides the inherited End from CompletedAsyncResult
    /// This method returns a collection of metadata
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    public static new Collection<EndpointDiscoveryMetadata> End(IAsyncResult result)
    {
      FindAsyncResult thisPtr = AsyncResult.End<FindAsyncResult>(result);
      return thisPtr.matchingEndpoints;
    }
  }
}
