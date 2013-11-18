using System;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 异步操作完成状态
  /// </summary>
  public class CompletedAsyncResult : AsyncResult
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CompletedAsyncResult"/> class.
    /// </summary>
    /// <param name="callback">异步回调函数</param>
    /// <param name="state">异步操作参数</param>
    public CompletedAsyncResult(AsyncCallback callback, object state)
      : base(callback, state)
    {
      Complete(true);
    }

    /// <summary>
    /// Ends the specified result.
    /// </summary>
    /// <param name="result">The result.</param>
    public static void End(IAsyncResult result)
    {
      AsyncResult.End<CompletedAsyncResult>(result);
    }
  }

  /// <summary>
  /// 异步操作完成状态
  /// </summary>
  /// <typeparam name="T">数据类型</typeparam>
  public class CompletedAsyncResult<T> : AsyncResult
  {
    private T data;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompletedAsyncResult&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="callback">异步回调函数</param>
    /// <param name="state">异步操作参数</param>
    public CompletedAsyncResult(T data, AsyncCallback callback, object state)
      : base(callback, state)
    {
      this.data = data;
      base.Complete(true);
    }

    /// <summary>
    /// Ends the specified result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    public static T End(IAsyncResult result)
    {
      return AsyncResult.End<CompletedAsyncResult<T>>(result).data;
    }
  }
}
