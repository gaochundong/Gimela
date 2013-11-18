using System;

namespace Gimela.Infrastructure.AsyncModel
{
  /// <summary>
  /// 异步工作器回调事件通知
  /// </summary>
  /// <typeparam name="T">工作器结果数据泛型</typeparam>
  public class AsyncWorkerCallbackEventArgs<T> : EventArgs
  {
    private readonly AsyncWorkerHandle<T> handle;
    private readonly T data;
    private readonly Exception exception;

    /// <summary>
    /// 异步工作器回调事件通知
    /// </summary>
    /// <param name="handle">异步工作器句柄</param>
    /// <param name="data">工作器获取的数据，例如异步获取到的数据集合。</param>
    /// <param name="exception">工作器内引发的异常</param>
    public AsyncWorkerCallbackEventArgs(AsyncWorkerHandle<T> handle, T data, Exception exception)
      : base()
    {
      this.handle = handle;
      this.data = data;
      this.exception = exception;
    }

    /// <summary>
    /// 异步工作器句柄
    /// </summary>
    public AsyncWorkerHandle<T> Handle
    {
      get { return this.handle; }
    }

    /// <summary>
    /// 工作器获取的数据，例如异步获取到的数据集合。
    /// </summary>
    public T Data
    {
      get { return this.data; }
    }

    /// <summary>
    /// 工作器内引发的异常
    /// </summary>
    public Exception Exception
    {
      get { return this.exception; }
    }
  }
}
