using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 指定的契约类型服务无法找到
  /// </summary>
  [Serializable]
  public class ContractNotFoundException : Exception
  {
    /// <summary>
    /// 指定的契约类型服务无法找到
    /// </summary>
    public ContractNotFoundException()
    {
    }

    /// <summary>
    /// 指定的契约类型服务无法找到
    /// </summary>
    /// <param name="message">异常信息</param>
    public ContractNotFoundException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// 指定的契约类型服务无法找到
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="innerException">内部异常</param>
    public ContractNotFoundException(string message, Exception innerException) :
      base(message, innerException)
    {
    }

    /// <summary>
    /// 指定的契约类型服务无法找到
    /// </summary>
    /// <param name="contractType">契约类型</param>
    public ContractNotFoundException(Type contractType)
      : base(string.Format(CultureInfo.InvariantCulture, "Cannot find {0}", contractType.FullName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContractNotFoundException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
    ///   
    /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
    protected ContractNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
