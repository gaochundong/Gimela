namespace Gimela.Net.Http
{
  /// <summary>
  /// Result of processing.
  /// </summary>
  public enum ProcessingResult
  {
    /// <summary>
    /// 请求需要继续被处理
    /// Continue with the next handler
    /// </summary>
    Continue,

    /// <summary>
    /// 请求已被处理，并且已发送响应消息至客户端
    /// </summary>
    /// <remarks>
    /// The server will process the response object and
    /// generate a HTTP response from it.
    /// </remarks>
    SendResponse,

    /// <summary>
    /// 响应消息已被发送至客户端
    /// </summary>
    /// <remarks>
    /// This option should only be used if you are streaming
    /// something or sending back a custom result. The server will
    /// not process the response object or send anything back
    /// to the client.
    /// </remarks>
    Abort
  }
}