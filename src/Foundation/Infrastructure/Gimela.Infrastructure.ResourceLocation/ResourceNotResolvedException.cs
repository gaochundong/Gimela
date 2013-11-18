using System;
using System.Runtime.Serialization;

namespace Gimela.Infrastructure.ResourceLocation
{
  /// <summary>
  /// 无法解决资源实例异常
  /// </summary>
  [Serializable]
  public class ResourceNotResolvedException : Exception
  {
    /// <summary>
    /// 无法解决资源实例异常
    /// </summary>
    public ResourceNotResolvedException()
    {
    }

    /// <summary>
    /// 无法解决资源实例异常
    /// </summary>
    /// <param name="message">The message.</param>
    public ResourceNotResolvedException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// 无法解决资源实例异常
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ResourceNotResolvedException(string message, Exception innerException) :
      base(message, innerException)
    {
    }

    /// <summary>
    /// 无法解决资源实例异常
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
    ///   
    /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
    protected ResourceNotResolvedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
