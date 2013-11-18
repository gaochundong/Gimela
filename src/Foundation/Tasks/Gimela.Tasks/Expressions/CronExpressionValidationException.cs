using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Gimela.Tasks.Expressions
{
  /// <summary>
  /// 时程表达式验证异常
  /// </summary>
  [Serializable]
  public class CronExpressionValidationException : Exception
  {
    /// <summary>
    /// 时程表达式验证异常
    /// </summary>
    public CronExpressionValidationException()
      : this("The CronExpression Validation Error")
    {
    }

    /// <summary>
    /// 时程表达式验证异常
    /// </summary>
    /// <param name="message">异常信息</param>
    public CronExpressionValidationException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// 时程表达式验证异常
    /// </summary>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">异常信息</param>
    public CronExpressionValidationException(string paramName, string message)
      : base(string.Format(CultureInfo.InvariantCulture, @"Parameter Name : {0}, Error : {1}", paramName, message))
    {
    }

    /// <summary>
    /// 时程表达式验证异常
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="innerException">内部异常</param>
    public CronExpressionValidationException(string message, Exception innerException) :
      base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CronExpressionValidationException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
    ///   
    /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
    protected CronExpressionValidationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    /// <summary>
    /// 抛出时程表达式验证异常
    /// </summary>
    public static void Throw()
    {
      throw new CronExpressionValidationException();
    }

    /// <summary>
    /// 抛出时程表达式验证异常
    /// </summary>
    /// <param name="message">异常信息</param>
    public static void Throw(string message)
    {
      throw new CronExpressionValidationException(message);
    }

    /// <summary>
    /// 抛出时程表达式验证异常
    /// </summary>
    /// <param name="paramName">参数名称</param>
    /// <param name="message">异常信息</param>
    public static void Throw(string paramName, string message)
    {
      throw new CronExpressionValidationException(paramName, message);
    }
  }
}
