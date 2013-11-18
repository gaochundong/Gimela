using System;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Xml.Linq;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 终结点发现元数据扩展
  /// </summary>
  public static class EndpointDiscoveryMetadataExtensions
  {
    /// <summary>
    /// 获取终结点发现元数据扩展的特定名称
    /// </summary>
    /// <param name="metadata">终结点发现元数据</param>
    /// <returns>扩展的特定名称</returns>
    public static string GetSpecifiedName(this EndpointDiscoveryMetadata metadata)
    {
      if (metadata == null)
      {
        throw new ArgumentNullException("metadata");
      }

      XElement nameElement = metadata.Extensions.Elements("SpecifiedName").FirstOrDefault();
      string name = null;
      if (nameElement != null)
      {
        name = nameElement.Value;
      }

      return name;
    }
  }
}
