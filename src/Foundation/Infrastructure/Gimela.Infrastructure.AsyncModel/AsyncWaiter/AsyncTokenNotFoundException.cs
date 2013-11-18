using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Gimela.Infrastructure.AsyncModel
{
  /// <summary>
  /// The async token can not be found.
  /// </summary>
  [Serializable]
  public class AsyncTokenNotFoundException<TToken> : Exception
  {
    /// <summary>
    /// Async Token
    /// </summary>
    public AsyncToken<TToken> Token { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncTokenNotFoundException&lt;TToken&gt;"/> class.
    /// </summary>
    public AsyncTokenNotFoundException()
      : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncTokenNotFoundException&lt;TToken&gt;"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public AsyncTokenNotFoundException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncTokenNotFoundException&lt;TToken&gt;"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AsyncTokenNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>
    /// The async token can not be found.
    /// </summary>
    /// <param name="token">Async Token</param>
    public AsyncTokenNotFoundException(AsyncToken<TToken> token)
      : this("Async token is not found.")
    {
      this.Token = token;
    }

    /// <summary>
    /// The async token can not be found.
    /// </summary>
    /// <param name="token">Async Token</param>
    /// <param name="innerException">Inner exception.</param>
    public AsyncTokenNotFoundException(AsyncToken<TToken> token, Exception innerException)
      : this("Async token is not found.", innerException)
    {
      this.Token = token;
    }

    /// <summary>
    /// 资源未找到异常
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
    /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
    protected AsyncTokenNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    /// <summary>
    /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception>
    ///   
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
    ///   </PermissionSet>
    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
    }
  }
}
