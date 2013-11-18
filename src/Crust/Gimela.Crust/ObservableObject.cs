using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Gimela.Crust
{
  /// <summary>
  /// 观察者对象类。该类将作为ViewModel类型的基类，或者那些需要提供属性更改通知的类型基类。
  /// 该类实现INotifyPropertyChanged接口，通常被ViewModelBase基类继承使用。
  /// </summary>
  public class ObservableObject : INotifyPropertyChanged
  {
    /// <summary>
    /// 当属性值变化时发生
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// 为子类提供PropertyChanged事件处理函数的访问能力，事件的访问句柄被限制在归属类中。
    /// </summary>
    protected PropertyChangedEventHandler PropertyChangedHandler
    {
      get
      {
        return PropertyChanged;
      }
    }

    /// <summary>
    /// 验证某属性名称是否存在于某ViewModel中。该方法可在属性值被触发RaisePropertyChanged前调用。
    /// 这避免了当某属性名称更改而在其他位置未改动的错误发生。
    /// 该方法只在DEBUG模式下可用。
    /// </summary>
    /// <param name="propertyName">属性名称</param>
    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    public void VerifyPropertyName(string propertyName)
    {
      var myType = this.GetType();
      if (myType.GetProperty(propertyName) == null)
      {
        throw new ArgumentException("Property not found.", propertyName);
      }
    }

    /// <summary>
    /// 触发PropertyChanged事件通知。当调用属性的setter方法时，触发当前属性的PropertyChanged事件通知。
    /// </summary>
    /// <exception cref="InvalidOperationException">如果该方法未在属性setter内使用，将抛出异常。</exception>
    protected virtual void RaisePropertyChanged()
    {
      var frames = new StackTrace();

      for (var i = 0; i < frames.FrameCount; i++)
      {
        var frame = frames.GetFrame(i).GetMethod() as MethodInfo;
        if (frame != null)
          if (frame.IsSpecialName && frame.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
          {
            RaisePropertyChanged(frame.Name.Substring(4));
            return;
          }
      }

      throw new InvalidOperationException("This method can only by invoked within a property setter.");
    }

    /// <summary>
    /// 触发PropertyChanged事件通知。
    /// </summary>
    /// <param name="propertyName">属性值被更改的属性名称</param>
    protected virtual void RaisePropertyChanged(string propertyName)
    {
      VerifyPropertyName(propertyName);

      var handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// 触发PropertyChanged事件通知。
    /// </summary>
    /// <typeparam name="T">属性值变化的类型</typeparam>
    /// <param name="propertyExpression">属性值变化的表达式</param>
    protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
    {
      if (propertyExpression == null)
      {
        return;
      }

      var handler = PropertyChanged;
      if (handler != null)
      {
        var body = propertyExpression.Body as MemberExpression;
        var expression = body.Expression as ConstantExpression;
        handler(expression.Value, new PropertyChangedEventArgs(body.Member.Name));
      }
    }
  }
}
