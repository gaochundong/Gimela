using System;

namespace Gimela.Infrastructure.AsyncModel
{
  /// <summary>
  /// 异步工作器句柄
  /// </summary>
  /// <typeparam name="T">工作器结果数据泛型</typeparam>
  public class AsyncWorkerHandle<T> : IDisposable
  {
    private AsyncWorker<T> worker;

    /// <summary>
    /// 异步工作器句柄
    /// </summary>
    /// <param name="worker">异步工作器</param>
    public AsyncWorkerHandle(AsyncWorker<T> worker)
    {
      if (worker == null)
        throw new ArgumentNullException("worker");

      this.worker = worker;
    }

    /// <summary>
    /// 是否已经请求取消操作
    /// </summary>
    public bool CancellationPending
    {
      get
      {
        bool isPending = false;

        if (this.worker != null)
        {
          isPending = this.worker.Worker.CancellationPending;
        }

        return isPending;
      }
    }

    /// <summary>
    /// 取消异步工作器工作
    /// </summary>
    public void CancelWork()
    {
      if (this.worker != null)
      {
        this.worker.CancelWork();
      }
    }

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.worker != null)
        {
          this.worker.Dispose();
          this.worker = null;
        }
      }
    }

    #endregion
  }
}
