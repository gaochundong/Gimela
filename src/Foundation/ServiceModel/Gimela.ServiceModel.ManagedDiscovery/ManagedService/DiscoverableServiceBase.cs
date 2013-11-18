using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Xml.Linq;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 可被发现服务基类，这是一个抽象类。
  /// </summary>
  public abstract class DiscoverableServiceBase : IDiscoverableService
  {
  }
}
