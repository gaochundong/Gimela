using System;
using System.Collections.Generic;
using System.ServiceModel.Discovery;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 终结点发现元数据集合
  /// </summary>
  public class EndpointDiscoveryMetadataCollection : SynchronizedKeyedCollection<Uri, EndpointDiscoveryMetadata>
  {
    /// <summary>
    /// 获取终结点地址URI
    /// </summary>
    /// <param name="item">终结点发现元数据</param>
    /// <returns>终结点地址URI</returns>
    protected override Uri GetKeyForItem(EndpointDiscoveryMetadata item)
    {
      if (item == null)
      {
        throw new ArgumentNullException("item");
      }

      return item.Address.Uri;
    }
  }
}
