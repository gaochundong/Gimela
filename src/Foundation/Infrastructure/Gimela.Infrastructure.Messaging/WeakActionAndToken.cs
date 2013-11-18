using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Infrastructure.Patterns;

namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 结构体包含Action弱引用和Token令牌
  /// </summary>
  internal struct WeakActionAndToken
  {
    /// <summary>
    /// Token令牌
    /// </summary>
    public object Token;

    /// <summary>
    /// Action弱引用
    /// </summary>
    public WeakAction Action;
  }
}
