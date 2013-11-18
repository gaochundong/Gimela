using System;
using System.Runtime.Serialization;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 发送消息失败异常
  /// </summary>
  [Serializable]
  public class SendMessageFailedException : Exception
  {
    /// <summary>
    /// 发送消息失败异常
    /// </summary>
    public SendMessageFailedException()
    {
    }

    /// <summary>
    /// 发送消息失败异常
    /// </summary>
    /// <param name="message">异常信息</param>
    public SendMessageFailedException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// 发送消息失败异常
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="innerException">内部异常</param>
    public SendMessageFailedException(string message, Exception innerException) :
      base(message, innerException)
    {
    }

    /// <summary>
    /// 发送消息失败异常
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected SendMessageFailedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
