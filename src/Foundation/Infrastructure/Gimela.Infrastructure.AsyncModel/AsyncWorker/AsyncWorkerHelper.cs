using System;
using System.ComponentModel;

namespace Gimela.Infrastructure.AsyncModel
{
  /// <summary>
  /// 异步工作器帮助类
  /// </summary>
  public static class AsyncWorkerHelper
  {
    /// <summary>
    /// 异步工作方法
    /// </summary>
    /// <typeparam name="T">异步工作结果数据泛型</typeparam>
    /// <param name="workerMethod">异步工作方法</param>
    /// <returns>异步工作器句柄</returns>
    public static AsyncWorkerHandle<T> DoWork<T>(
      DoWorkEventHandler workerMethod)
    {
      return AsyncWorkerHelper.DoWork<T>(workerMethod, 0, null, null);
    }

    /// <summary>
    /// 异步工作方法
    /// </summary>
    /// <typeparam name="T">异步工作结果数据泛型</typeparam>
    /// <param name="workerMethod">异步工作方法</param>
    /// <param name="workerMethodCompletedCallback">异步工作方法已完成回调函数</param>
    /// <returns>异步工作器句柄</returns>
    public static AsyncWorkerHandle<T> DoWork<T>(
      DoWorkEventHandler workerMethod,
      EventHandler<AsyncWorkerCallbackEventArgs<T>> workerMethodCompletedCallback)
    {
      return AsyncWorkerHelper.DoWork<T>(workerMethod, 0, null, workerMethodCompletedCallback);
    }

    /// <summary>
    /// 异步工作方法
    /// </summary>
    /// <typeparam name="T">异步工作结果数据泛型</typeparam>
    /// <param name="workerMethod">异步工作方法</param>
    /// <param name="workerProgressChangedCallback">异步工作方法进度回调函数</param>
    /// <param name="workerMethodCompletedCallback">异步工作方法已完成回调函数</param>
    /// <returns>异步工作器句柄</returns>
    public static AsyncWorkerHandle<T> DoWork<T>(
      DoWorkEventHandler workerMethod,
      ProgressChangedEventHandler workerProgressChangedCallback,
      EventHandler<AsyncWorkerCallbackEventArgs<T>> workerMethodCompletedCallback)
    {
      return AsyncWorkerHelper.DoWork<T>(workerMethod, 0, workerProgressChangedCallback, workerMethodCompletedCallback);
    }

    /// <summary>
    /// 异步工作方法
    /// </summary>
    /// <typeparam name="T">异步工作结果数据泛型</typeparam>
    /// <param name="workerMethod">异步工作方法</param>
    /// <param name="workerMethodTimeoutSeconds">异步工作方法超时时长，0为永不超时</param>
    /// <param name="workerProgressChangedCallback">异步工作方法进度回调函数</param>
    /// <param name="workerMethodCompletedCallback">异步工作方法已完成回调函数</param>
    /// <returns>异步工作器句柄</returns>
    public static AsyncWorkerHandle<T> DoWork<T>(
      DoWorkEventHandler workerMethod,
      int workerMethodTimeoutSeconds,
      ProgressChangedEventHandler workerProgressChangedCallback,
      EventHandler<AsyncWorkerCallbackEventArgs<T>> workerMethodCompletedCallback)
    {
      AsyncWorker<T> worker = new AsyncWorker<T>(workerMethod, workerProgressChangedCallback, workerMethodCompletedCallback);
      return worker.StartWork(workerMethodTimeoutSeconds);
    }
  }
}
