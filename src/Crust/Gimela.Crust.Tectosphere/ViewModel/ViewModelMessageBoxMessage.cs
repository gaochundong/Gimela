using System;
using Gimela.Infrastructure.Messaging;

namespace Gimela.Crust.Tectosphere
{
  /// <summary>
  /// ViewModel消息框通知消息
  /// </summary>
  public class ViewModelMessageBoxMessage : NotificationMessage
  {
    #region Fields
    
    /// <summary>
    /// 消息处理完毕的回调方法
    /// </summary>
    private readonly Delegate _callback;

    #endregion

    #region Ctors
    
    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    public ViewModelMessageBoxMessage(string messageBoxDescription, ViewModelMessageBoxType messageBoxType)
      : base(messageBoxDescription)
    {
      MessageBoxType = messageBoxType;
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    public ViewModelMessageBoxMessage(object sender, string messageBoxDescription, ViewModelMessageBoxType messageBoxType)
      : base(sender, messageBoxDescription)
    {
      MessageBoxType = messageBoxType;
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="target">指定的消息接收者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    public ViewModelMessageBoxMessage(object sender, object target, string messageBoxDescription, ViewModelMessageBoxType messageBoxType)
      : base(sender, target, messageBoxDescription)
    {
      MessageBoxType = messageBoxType;
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="callback">消息处理完毕的回调方法</param>
    public ViewModelMessageBoxMessage(string messageBoxDescription, ViewModelMessageBoxType messageBoxType, Delegate callback)
      : base(messageBoxDescription)
    {
      MessageBoxType = messageBoxType;
      _callback = callback;  
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="callback">消息处理完毕的回调方法</param>
    public ViewModelMessageBoxMessage(object sender, string messageBoxDescription, ViewModelMessageBoxType messageBoxType, Delegate callback)
      : base(sender, messageBoxDescription)
    {
      MessageBoxType = messageBoxType;
      _callback = callback;  
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="target">指定的消息接收者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="callback">消息处理完毕的回调方法</param>
    public ViewModelMessageBoxMessage(object sender, object target, string messageBoxDescription, ViewModelMessageBoxType messageBoxType, Delegate callback)
      : base(sender, target, messageBoxDescription)
    {
      MessageBoxType = messageBoxType;
      _callback = callback;      
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="messageBoxDetailDescription">消息提示框中显示的详细内容</param>
    public ViewModelMessageBoxMessage(string messageBoxDescription, ViewModelMessageBoxType messageBoxType, string messageBoxDetailDescription)
      : base(messageBoxDescription)
    {
      Detail = messageBoxDetailDescription;
      MessageBoxType = messageBoxType;
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="messageBoxDetailDescription">消息提示框中显示的详细内容</param>
    public ViewModelMessageBoxMessage(object sender, string messageBoxDescription, ViewModelMessageBoxType messageBoxType, string messageBoxDetailDescription)
      : base(sender, messageBoxDescription)
    {
      Detail = messageBoxDetailDescription;
      MessageBoxType = messageBoxType;
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="target">指定的消息接收者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="messageBoxDetailDescription">消息提示框中显示的详细内容</param>
    public ViewModelMessageBoxMessage(object sender, object target, string messageBoxDescription, ViewModelMessageBoxType messageBoxType, string messageBoxDetailDescription)
      : base(sender, target, messageBoxDescription)
    {
      Detail = messageBoxDetailDescription;
      MessageBoxType = messageBoxType;
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="callback">消息处理完毕的回调方法</param>
    /// <param name="messageBoxDetailDescription">消息提示框中显示的详细内容</param>
    public ViewModelMessageBoxMessage(string messageBoxDescription, ViewModelMessageBoxType messageBoxType, Delegate callback, string messageBoxDetailDescription)
      : base(messageBoxDescription)
    {
      Detail = messageBoxDetailDescription;
      MessageBoxType = messageBoxType;
      _callback = callback;
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="callback">消息处理完毕的回调方法</param>
    /// <param name="messageBoxDetailDescription">消息提示框中显示的详细内容</param>
    public ViewModelMessageBoxMessage(object sender, string messageBoxDescription, ViewModelMessageBoxType messageBoxType, Delegate callback, string messageBoxDetailDescription)
      : base(sender, messageBoxDescription)
    {
      Detail = messageBoxDetailDescription;
      MessageBoxType = messageBoxType;
      _callback = callback;
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="target">指定的消息接收者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="callback">消息处理完毕的回调方法</param>
    /// <param name="messageBoxDetailDescription">消息提示框中显示的详细内容</param>
    public ViewModelMessageBoxMessage(object sender, object target, string messageBoxDescription, ViewModelMessageBoxType messageBoxType, Delegate callback, string messageBoxDetailDescription)
      : base(sender, target, messageBoxDescription)
    {
      Detail = messageBoxDetailDescription;
      MessageBoxType = messageBoxType;
      _callback = callback;
    }

    #endregion

    #region Properties
    
    /// <summary>
    /// 获取消息提示框的消息详细内容
    /// </summary>
    public string Detail { get; protected set; }

    /// <summary>
    /// 获取消息提示框的类型
    /// </summary>
    public ViewModelMessageBoxType MessageBoxType { get; protected set; }

    #endregion

    #region Public Methods
    
    /// <summary>
    /// 执行回调函数
    /// </summary>
    /// <param name="arguments">回调函数的参数</param>
    /// <returns>回调函数的执行结果返回值</returns>
    public virtual object Execute(params object[] arguments)
    {
      object result = null;

      if (_callback != null)
      {
        result = _callback.DynamicInvoke(arguments);
      }

      return result;
    }

    #endregion
  }
}
