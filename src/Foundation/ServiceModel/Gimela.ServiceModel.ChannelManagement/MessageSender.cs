using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using Gimela.ServiceModel.ChannelManagement.Factories;
using Gimela.Common.ExceptionHandling;
using Gimela.Common.Logging;
using Gimela.ServiceModel.ManagedDiscovery;

namespace Gimela.ServiceModel.ChannelManagement
{
  /// <summary>
  /// 消息发送器
  /// </summary>
  public static class MessageSender
  {
    #region Variables

    /// <summary>
    /// 消息发送失败后重新尝试次数
    /// </summary>
    private const int RETRY_COUNT_FOR_FAILED = 5;
    /// <summary>
    /// 每次重新尝试前中断等待的时间
    /// </summary>
    private static readonly TimeSpan BREAK_TIME_BEFORE_RETRY = TimeSpan.FromMilliseconds(100);
    /// <summary>
    /// 传输通道管理器
    /// </summary>
    private static readonly ITransportManager transporter = TransportManagerFactory.Create();

    #endregion

    #region Send Message

    /// <summary>
    /// 发送单向消息
    /// </summary>
    /// <typeparam name="TContract">契约类型</typeparam>
    /// <typeparam name="TArgument">参数类型</typeparam>
    /// <param name="function">消息发送代理</param>
    /// <param name="argument">发送参数</param>
    public static void Send<TContract, TArgument>(SendOneWayMessage<TContract, TArgument> function, TArgument argument) where TContract : class
    {
      if (function == null)
        throw new ArgumentNullException("function");

      bool isSuccess = false;
      int retryCount = RETRY_COUNT_FOR_FAILED;
      Exception exception = null;

      while (!isSuccess && retryCount-- > 0)
      {
        try
        {
          TContract svc = transporter.GetChannel<TContract>();
          function(svc, argument);
          isSuccess = true;
        }
        catch (FaultException ex)
        {
          ExceptionHandler.Handle(ex);
          throw;
        }
        catch (Exception ex)
        {
          ExceptionHandler.Handle(ex);
          exception = ex;
          SendFailedRetryCounter<TContract>(retryCount, exception);
        }
      }

      if (!isSuccess) SendFailed<TContract>(exception);
    }

    /// <summary>
    /// 发送请求回复消息
    /// </summary>
    /// <typeparam name="TContract">契约类型</typeparam>
    /// <typeparam name="TReturnData">返回值类型</typeparam>
    /// <typeparam name="TArgument">参数类型</typeparam>
    /// <param name="function">消息发送代理</param>
    /// <param name="argument">发送参数</param>
    /// <returns>返回值</returns>
    public static TReturnData Send<TContract, TReturnData, TArgument>(SendRequestReplyMessage<TContract, TReturnData, TArgument> function, TArgument argument) where TContract : class
    {
      if (function == null)
        throw new ArgumentNullException("function");

      bool isSuccess = false;
      int retryCount = RETRY_COUNT_FOR_FAILED;
      Exception exception = null;
      TReturnData data = default(TReturnData);

      while (!isSuccess && retryCount-- > 0)
      {
        try
        {
          TContract svc = transporter.GetChannel<TContract>();
          data = function(svc, argument);
          isSuccess = true;
        }
        catch (FaultException ex)
        {
          ExceptionHandler.Handle(ex);
          throw;
        }
        catch (Exception ex)
        {
          ExceptionHandler.Handle(ex);
          exception = ex;
          SendFailedRetryCounter<TContract>(retryCount, exception);
        }
      }

      if (!isSuccess) SendFailed<TContract>(exception);

      return data;
    }

    /// <summary>
    /// 发送双向请求消息
    /// </summary>
    /// <typeparam name="TContract">契约类型</typeparam>
    /// <typeparam name="TReturnData">返回值类型</typeparam>
    /// <typeparam name="TArgument">参数类型</typeparam>
    /// <param name="context">服务实例上下文</param>
    /// <param name="function">消息发送代理</param>
    /// <param name="argument">发送参数</param>
    /// <returns>返回值</returns>
    public static TReturnData Send<TContract, TReturnData, TArgument>(InstanceContext context, SendDuplexMessage<TContract, TReturnData, TArgument> function, TArgument argument) where TContract : class
    {
      if (function == null)
        throw new ArgumentNullException("function");

      bool isSuccess = false;
      int retryCount = RETRY_COUNT_FOR_FAILED;
      Exception exception = null;
      TReturnData data = default(TReturnData);

      while (!isSuccess && retryCount-- > 0)
      {
        try
        {
          TContract svc = transporter.GetDuplexChannel<TContract>(context);
          data = function(svc, argument);
          isSuccess = true;
        }
        catch (FaultException ex)
        {
          ExceptionHandler.Handle(ex);
          throw;
        }
        catch (Exception ex)
        {
          ExceptionHandler.Handle(ex);
          exception = ex;
          SendFailedRetryCounter<TContract>(retryCount, exception);
        }
      }

      if (!isSuccess) SendFailed<TContract>(exception);

      return data;
    }

    #endregion

    #region Send Callback

    /// <summary>
    /// 发送双向回调消息
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <typeparam name="TArgument">参数类型</typeparam>
    /// <param name="function">消息发送代理</param>
    /// <param name="argument">发送参数</param>
    /// <param name="clientHostName">客户端主机唯一识别名</param>
    public static void SendCallback<TContract, TArgument>(SendCallbackMessage<TContract, TArgument> function, TArgument argument, string clientHostName) where TContract : class
    {
      if (function == null)
        throw new ArgumentNullException("function");

      bool isSuccess = false;
      int retryCount = RETRY_COUNT_FOR_FAILED;
      Exception exception = null;

      while (!isSuccess && retryCount-- > 0)
      {
        try
        {
          TContract svc = transporter.GetCallbackChannel<TContract>(clientHostName);
          if (svc == null)
            throw new ContractNotFoundException(typeof(TContract));
          function(svc, argument);
          isSuccess = true;
        }
        catch (FaultException ex)
        {
          ExceptionHandler.Handle(ex);
          throw;
        }
        catch (ContractNotFoundException ex)
        {
          ExceptionHandler.Handle(ex);
          throw;
        }
        catch (Exception ex)
        {
          ExceptionHandler.Handle(ex);
          exception = ex;
          SendFailedRetryCounter<TContract>(retryCount, exception);
        }
      }

      if (!isSuccess) SendFailed<TContract>(exception);
    }

    /// <summary>
    /// 注册双向回调实例
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <param name="callbackInstanceContext">回调契约通道连接</param>
    /// <param name="clientHostName">客户端主机唯一识别名</param>
    public static void RegisterCallback<TContract>(TContract callbackInstanceContext, string clientHostName) where TContract : class
    {
      if (callbackInstanceContext == null)
        throw new ArgumentNullException("callbackInstanceContext");

      transporter.RegisterCallback<TContract>(callbackInstanceContext, clientHostName);
    }

    /// <summary>
    /// 取消注册双向回调实例
    /// </summary>
    /// <typeparam name="TContract">回调契约类型</typeparam>
    /// <param name="clientHostName">客户端主机唯一识别名</param>
    public static void UnregisterCallback<TContract>(string clientHostName) where TContract : class
    {
      transporter.UnregisterCallback<TContract>(clientHostName);
    }

    #endregion

    #region Send Failed

    /// <summary>
    /// 发送失败时重新尝试计数器
    /// </summary>
    /// <typeparam name="TContract">契约类型</typeparam>
    /// <param name="retryCount">重试次数</param>
    /// <param name="exception">发送失败时产生的异常</param>
    private static void SendFailedRetryCounter<TContract>(int retryCount, Exception exception) where TContract : class
    {
      Logger.Debug(string.Format(CultureInfo.InvariantCulture, "Send message retrying with count {1}. Reason: {1}", (RETRY_COUNT_FOR_FAILED - retryCount), exception.Message));
      if (retryCount <= 0)
      {
        SendFailed<TContract>(exception);
      }

      Thread.Sleep(BREAK_TIME_BEFORE_RETRY);
    }

    /// <summary>
    /// 发送失败处理方法
    /// </summary>
    /// <typeparam name="TContract">契约类型</typeparam>
    /// <param name="exception">发送失败时产生的异常</param>
    private static void SendFailed<TContract>(Exception exception) where TContract : class
    {
      string msg = string.Format(CultureInfo.InvariantCulture, "Unable to send message to remote service [{0}] after number of retries. Reason: {1}",
        typeof(TContract), exception.Message);
      Logger.Error(msg);

      throw new SendMessageFailedException(msg, exception);
    }

    #endregion
  }
}
