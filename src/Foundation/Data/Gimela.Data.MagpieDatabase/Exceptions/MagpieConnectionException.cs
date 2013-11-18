using System;
using System.Runtime.Serialization;

namespace Gimela.Data.Magpie
{
  /// <summary>
  /// 数据库连接异常
  /// </summary>
  [Serializable]
  public class MagpieConnectionException : MagpieException
  {
    /// <summary>
    /// 数据库连接异常
    /// </summary>
    public MagpieConnectionException()
      : base()
    {
    }

    /// <summary>
    /// 数据库连接异常
    /// </summary>
    /// <param name="message">异常信息</param>
    public MagpieConnectionException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// 数据库连接异常
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="innerException">内部异常</param>
    public MagpieConnectionException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>
    /// 数据库连接异常
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
    ///   
    /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
    protected MagpieConnectionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
