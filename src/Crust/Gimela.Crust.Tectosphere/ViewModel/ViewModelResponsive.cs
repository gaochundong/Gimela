using System;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;
using Gimela.Infrastructure.AsyncModel;

namespace Gimela.Crust.Tectosphere
{
  /// <summary>
  /// 响应式的ViewModel模型
  /// </summary>
  public abstract class ViewModelResponsive : ViewModelBase, IViewModelResponsive
  {
    #region Fields

    private ViewModelStatus _status = ViewModelStatus.None;

    #endregion

    #region Ctor

    /// <summary>
    /// 响应式的ViewModel模型
    /// </summary>
    public ViewModelResponsive()
      : this(true)
    {
    }

    /// <summary>
    /// 响应式的ViewModel模型
    /// </summary>
    /// <param name="automaticInitialization">是否自动初始化</param>
    public ViewModelResponsive(bool automaticInitialization)
    {
      if (automaticInitialization)
      {
        Initialize();
      }
    }

    #endregion

    #region ViewModel Initialize

    /// <summary>
    /// 初始化ViewModel
    /// </summary>
    public void Initialize()
    {
      if (Status == ViewModelStatus.None)
      {
        Status = ViewModelStatus.Initializing;
        SubscribeMessages();
        BindCommands();
        Status = ViewModelStatus.Initialized;
      }
    }

    /// <summary>
    /// 订阅消息通知，可从其他对象收取消息通知。
    /// </summary>
    protected abstract void SubscribeMessages();

    /// <summary>
    /// 取消订阅消息通知
    /// </summary>
    protected abstract void UnsubscribeMessages();

    /// <summary>
    /// 绑定Command供View使用
    /// </summary>
    protected abstract void BindCommands();

    /// <summary>
    /// 取消绑定Command
    /// </summary>
    protected abstract void UnbindCommands();

    #endregion

    #region ViewModel Status

    /// <summary>
    /// 刷新UI数据
    /// </summary>
    public virtual void Refresh()
    {

    }

    /// <summary>
    /// ViewModel状态
    /// </summary>
    public ViewModelStatus Status
    {
      get
      {
        return _status;
      }
      protected set
      {
        if (_status != value)
        {
          _status = value;
          RaisePropertyChanged(@"Status");
        }
      }
    }

    #endregion

    #region Async Worker Callback

    /// <summary>
    /// 检查异步方法回调参数
    /// </summary>
    /// <typeparam name="T">异步方法回调中的期待返回值类型</typeparam>
    /// <param name="sender">异步回调的发送者</param>
    /// <param name="e">异步方法回调参数</param>
    /// <param name="showingMessageBox">指定当检测回调参数异常时是否显示提示框</param>
    /// <param name="messageBoxDescription">指定当检测回调参数异常时显示的提示框描述</param>
    /// <returns>如果为真，则异步方法回调参数解析正常。</returns>
    protected virtual bool CheckAsyncWorkerCallback<TArgument>(object sender, AsyncWorkerCallbackEventArgs<TArgument> e, bool showingMessageBox, string messageBoxDescription)
    {
      bool result = false;

      if (e == null)
      {
        if (showingMessageBox)
        {
          DispatcherHelper.InvokeOnUI(() =>
          {
            Messenger.Default.Send(new ViewModelMessageBoxMessage(this, messageBoxDescription, ViewModelMessageBoxType.Error));
          });
        }
      }
      else if (e.Exception != null)
      {
        if (showingMessageBox)
        {
          DispatcherHelper.InvokeOnUI(() =>
          {
            Messenger.Default.Send(new ViewModelMessageBoxMessage(this, messageBoxDescription, ViewModelMessageBoxType.Error, e.Exception.ToString()));
          });
        }
      }
      else if (e.Data == null)
      {
        if (showingMessageBox)
        {
          DispatcherHelper.InvokeOnUI(() =>
          {
            Messenger.Default.Send(new ViewModelMessageBoxMessage(this, messageBoxDescription, ViewModelMessageBoxType.Error));
          });
        }
      }
      else
      {
        result = true;
      }

      return result;
    }

    /// <summary>
    /// 检查异步方法回调参数
    /// </summary>
    /// <typeparam name="T">异步方法回调中的期待返回值类型</typeparam>
    /// <param name="sender">异步回调的发送者</param>
    /// <param name="e">异步方法回调参数</param>
    /// <param name="showingMessageBox">指定当检测回调参数异常时是否显示提示框</param>
    /// <param name="messageBoxDescription">指定当检测回调参数异常时显示的提示框描述</param>
    /// <param name="messageBoxResultCallback">指定当检测回调参数异常时显示的提示框操作结果回调</param>
    /// <returns>如果为真，则异步方法回调参数解析正常。</returns>
    protected virtual bool CheckAsyncWorkerCallback<TArgument, TCallbackParameter>(object sender, AsyncWorkerCallbackEventArgs<TArgument> e, bool showingMessageBox, string messageBoxDescription, Action<TCallbackParameter> messageBoxResultCallback)
    {
      bool result = false;

      if (e == null)
      {
        if (showingMessageBox)
        {
          DispatcherHelper.InvokeOnUI(() =>
          {
            Messenger.Default.Send(new ViewModelMessageBoxMessage<TCallbackParameter>(this, messageBoxDescription, ViewModelMessageBoxType.Error, messageBoxResultCallback));
          });
        }
      }
      else if (e.Exception != null)
      {
        if (showingMessageBox)
        {
          DispatcherHelper.InvokeOnUI(() =>
          {
            Messenger.Default.Send(new ViewModelMessageBoxMessage<TCallbackParameter>(this, messageBoxDescription, ViewModelMessageBoxType.Error, messageBoxResultCallback, e.Exception.ToString()));
          });
        }
      }
      else if (e.Data == null)
      {
        if (showingMessageBox)
        {
          DispatcherHelper.InvokeOnUI(() =>
          {
            Messenger.Default.Send(new ViewModelMessageBoxMessage<TCallbackParameter>(this, messageBoxDescription, ViewModelMessageBoxType.Error, messageBoxResultCallback));
          });
        }
      }
      else
      {
        result = true;
      }

      return result;
    }

    #endregion

    #region ICleanup Members

    /// <summary>
    /// 清理实例。如取消注册该实例对消息发送器的订阅。
    /// </summary>
    public override void Cleanup()
    {
      base.Cleanup();

      this.UnbindCommands();
      this.UnsubscribeMessages();
    }

    #endregion
  }
}
