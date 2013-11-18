using System;
using System.Threading;

namespace Gimela.ServiceModel.ManagedDiscovery
{
  /// <summary>
  /// 异步操作状态
  /// </summary>
  public abstract class AsyncResult : IAsyncResult
  {
    private AsyncCallback callback;
    private bool endCalled;
    private Exception completeException;
    private bool isCompleted;
    private ManualResetEvent manualResetEvent;
    private object state;
    private object operationLock;

    /// <summary>
    /// 异步操作状态
    /// </summary>
    /// <param name="callback">异步回调函数</param>
    /// <param name="state">异步操作参数</param>
    protected AsyncResult(AsyncCallback callback, object state)
    {
      this.callback = callback;
      this.state = state;
      this.operationLock = new object();
    }

    /// <summary>
    /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
    /// </summary>
    /// <returns>A user-defined object that qualifies or contains information about an asynchronous operation.</returns>
    public object AsyncState
    {
      get
      {
        return this.state;
      }
    }

    /// <summary>
    /// Gets a value that indicates whether the asynchronous operation completed synchronously.
    /// </summary>
    /// <returns>true if the asynchronous operation completed synchronously; otherwise, false.</returns>
    public bool CompletedSynchronously { get; private set; }

    /// <summary>
    /// Gets a <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
    /// </summary>
    /// <returns>A <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.</returns>
    public WaitHandle AsyncWaitHandle
    {
      get
      {
        if (this.manualResetEvent != null)
        {
          return this.manualResetEvent;
        }

        lock (this.operationLock)
        {
          if (this.manualResetEvent == null)
          {
            this.manualResetEvent = new ManualResetEvent(this.isCompleted);
          }
        }

        return this.manualResetEvent;
      }
    }

    /// <summary>
    /// Gets a value that indicates whether the asynchronous operation has completed.
    /// </summary>
    /// <returns>true if the operation is complete; otherwise, false.</returns>
    public bool IsCompleted
    {
      get
      {
        return this.isCompleted;
      }
    }

    /// <summary>
    /// Ends the specified result.
    /// End should be called when the End function for the asynchronous operation is complete.  It
    /// ensures the asynchronous operation is complete, and does some common validation.
    /// </summary>
    /// <typeparam name="TAsyncResult">The type of the async result.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    protected static TAsyncResult End<TAsyncResult>(IAsyncResult result)
        where TAsyncResult : AsyncResult
    {
      if (result == null)
      {
        throw new ArgumentNullException("result");
      }

      TAsyncResult asyncResult = result as TAsyncResult;

      if (asyncResult == null)
      {
        throw new ArgumentException("Invalid async result.", "result");
      }

      if (asyncResult.endCalled)
      {
        throw new InvalidOperationException("Async object already ended.");
      }

      asyncResult.endCalled = true;

      if (!asyncResult.isCompleted)
      {
        asyncResult.AsyncWaitHandle.WaitOne();
      }

      if (asyncResult.manualResetEvent != null)
      {
        asyncResult.manualResetEvent.Close();
      }

      if (asyncResult.completeException != null)
      {
        throw asyncResult.completeException;
      }

      return asyncResult;
    }

    /// <summary>
    /// Completes the specified completed synchronously.
    /// Call this version of complete when your asynchronous operation is complete.  This will update the state
    /// of the operation and notify the callback.
    /// </summary>
    /// <param name="completedSynchronously">if set to <c>true</c> [completed synchronously].</param>
    protected void Complete(bool completedSynchronously)
    {
      if (this.isCompleted)
      {
        // It's a bug to call Complete twice.
        throw new InvalidOperationException("This async result is already completed.");
      }

      this.CompletedSynchronously = completedSynchronously;

      if (completedSynchronously)
      {
        this.isCompleted = true;
      }
      else
      {
        lock (this.operationLock)
        {
          this.isCompleted = true;
          if (this.manualResetEvent != null)
          {
            this.manualResetEvent.Set();
          }
        }
      }

      // If the callback throws, there is a bug in the callback implementation
      if (this.callback != null)
      {
        this.callback(this);
      }
    }

    /// <summary>
    /// Completes the specified completed synchronously.
    /// Call this version of complete if you raise an exception during processing.  In addition to notifying
    /// the callback, it will capture the exception and store it to be thrown during AsyncResult.End.
    /// </summary>
    /// <param name="completedSynchronously">if set to <c>true</c> [completed synchronously].</param>
    /// <param name="exception">The exception.</param>
    protected void Complete(bool completedSynchronously, Exception exception)
    {
      this.completeException = exception;
      this.Complete(completedSynchronously);
    }
  }
}
