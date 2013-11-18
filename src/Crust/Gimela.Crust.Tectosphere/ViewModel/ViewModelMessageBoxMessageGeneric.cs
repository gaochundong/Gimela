using System;

namespace Gimela.Crust.Tectosphere
{
  /// <summary>
  /// ViewModel消息框通知消息
  /// </summary>
  /// <typeparam name="TCallbackParameter">消息框通知处理完毕后的回调方法参数类型</typeparam>
  public class ViewModelMessageBoxMessage<TCallbackParameter> : ViewModelMessageBoxMessage
  {
    #region Ctors
    
    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    public ViewModelMessageBoxMessage(string messageBoxDescription, ViewModelMessageBoxType messageBoxType)
      : base(messageBoxDescription, messageBoxType)
    {
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    public ViewModelMessageBoxMessage(object sender, string messageBoxDescription, ViewModelMessageBoxType messageBoxType)
      : base(sender, messageBoxDescription, messageBoxType)
    {
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="target">指定的消息接收者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    public ViewModelMessageBoxMessage(object sender, object target, string messageBoxDescription, ViewModelMessageBoxType messageBoxType)
      : base(sender, target, messageBoxDescription, messageBoxType)
    {
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="callback">消息处理完毕的回调方法</param>
    public ViewModelMessageBoxMessage(string messageBoxDescription, ViewModelMessageBoxType messageBoxType, Action<TCallbackParameter> callback)
      : base(messageBoxDescription, messageBoxType, callback)
    {
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="callback">消息处理完毕的回调方法</param>
    public ViewModelMessageBoxMessage(object sender, string messageBoxDescription, ViewModelMessageBoxType messageBoxType, Action<TCallbackParameter> callback)
      : base(sender, messageBoxDescription, messageBoxType, callback)
    {
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="target">指定的消息接收者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="callback">消息处理完毕的回调方法</param>
    public ViewModelMessageBoxMessage(object sender, object target, string messageBoxDescription, ViewModelMessageBoxType messageBoxType, Action<TCallbackParameter> callback)
      : base(sender, target, messageBoxDescription, messageBoxType, callback)
    {   
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="messageBoxDetailDescription">消息提示框中显示的详细内容</param>
    public ViewModelMessageBoxMessage(string messageBoxDescription, ViewModelMessageBoxType messageBoxType, string messageBoxDetailDescription)
      : base(messageBoxDescription, messageBoxType, messageBoxDetailDescription)
    {
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="messageBoxDetailDescription">消息提示框中显示的详细内容</param>
    public ViewModelMessageBoxMessage(object sender, string messageBoxDescription, ViewModelMessageBoxType messageBoxType, string messageBoxDetailDescription)
      : base(sender, messageBoxDescription, messageBoxType, messageBoxDetailDescription)
    {
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
      : base(sender, target, messageBoxDescription, messageBoxType, messageBoxDetailDescription)
    {
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="callback">消息处理完毕的回调方法</param>
    /// <param name="messageBoxDetailDescription">消息提示框中显示的详细内容</param>
    public ViewModelMessageBoxMessage(string messageBoxDescription, ViewModelMessageBoxType messageBoxType, Action<TCallbackParameter> callback, string messageBoxDetailDescription)
      : base(messageBoxDescription, messageBoxType, callback, messageBoxDetailDescription)
    {
    }

    /// <summary>
    /// ViewModel消息框通知消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    /// <param name="messageBoxDescription">消息提示框中显示的内容</param>
    /// <param name="messageBoxType">消息提示框的类型</param>
    /// <param name="callback">消息处理完毕的回调方法</param>
    /// <param name="messageBoxDetailDescription">消息提示框中显示的详细内容</param>
    public ViewModelMessageBoxMessage(object sender, string messageBoxDescription, ViewModelMessageBoxType messageBoxType, Action<TCallbackParameter> callback, string messageBoxDetailDescription)
      : base(sender, messageBoxDescription, messageBoxType, callback, messageBoxDetailDescription)
    {
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
    public ViewModelMessageBoxMessage(object sender, object target, string messageBoxDescription, ViewModelMessageBoxType messageBoxType, Action<TCallbackParameter> callback, string messageBoxDetailDescription)
      : base(sender, target, messageBoxDescription, messageBoxType, callback, messageBoxDetailDescription)
    {
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 执行回调函数
    /// </summary>
    /// <param name="arguments">回调函数的参数</param>
    /// <returns>回调函数的执行结果返回值</returns>
    public object Execute(TCallbackParameter parameter)
    {
      return base.Execute(parameter);
    }

    #endregion
  }
}
