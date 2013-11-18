using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Infrastructure.Messaging
{
  /// <summary>
  /// 消息接口
  /// </summary>
  public interface IMessage
  {
    /// <summary>
    /// 获取或设置消息的发送者
    /// </summary>
    object Sender { get; }

    /// <summary>
    /// 获取或设置消息的接收者
    /// </summary>
    object Target { get; }
  }
}
