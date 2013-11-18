using System;
using System.Text;
using System.Windows;

namespace Gimela.Presentation.Windows
{
  /// <summary>
  /// 消息通知窗体
  /// </summary>
  public partial class MessageBoxWindow : Window
  {
    #region Ctor

    public MessageBoxWindow()
    {
      InitializeComponent();
    }

    #endregion

    #region Dependency Property

    /// <summary>
    /// 消息通知窗体类型
    /// </summary>
    public MessageBoxWindowType WindowType
    {
      get
      {
        return (MessageBoxWindowType)GetValue(WindowTypeProperty);
      }
      set
      {
        SetValue(WindowTypeProperty, value);
      }
    }

    public static DependencyProperty WindowTypeProperty = DependencyProperty.Register(
        "WindowType",
        typeof(MessageBoxWindowType),
        typeof(MessageBoxWindow),
        new FrameworkPropertyMetadata(MessageBoxWindowType.Information,
            FrameworkPropertyMetadataOptions.AffectsRender,
            new PropertyChangedCallback(OnWindowTypeChanged)));

    private static void OnWindowTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

    }

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Message
    {
      get
      {
        return (string)GetValue(MessageProperty);
      }
      set
      {
        SetValue(MessageProperty, value);
      }
    }

    public static DependencyProperty MessageProperty = DependencyProperty.Register(
        "Message",
        typeof(string),
        typeof(MessageBoxWindow),
        new FrameworkPropertyMetadata(String.Empty,
            FrameworkPropertyMetadataOptions.AffectsRender,
            new PropertyChangedCallback(OnMessageChanged)));

    private static void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

    }

    /// <summary>
    /// 消息详细
    /// </summary>
    public string Detail
    {
      get
      {
        return (string)GetValue(DetailProperty);
      }
      set
      {
        SetValue(DetailProperty, value);
      }
    }

    public static DependencyProperty DetailProperty = DependencyProperty.Register(
        "Detail",
        typeof(string),
        typeof(MessageBoxWindow),
        new FrameworkPropertyMetadata(String.Empty,
            FrameworkPropertyMetadataOptions.AffectsRender,
            new PropertyChangedCallback(OnDetailChanged)));

    private static void OnDetailChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    /// <summary>
    /// 消息窗体中的按钮类型
    /// </summary>
    public MessageBoxWindowButtonsType ButtonsType
    {
      get
      {
        return (MessageBoxWindowButtonsType)GetValue(ButtonsTypeModeroperty);
      }
      set
      {
        SetValue(ButtonsTypeModeroperty, value);
      }
    }

    public static DependencyProperty ButtonsTypeModeroperty = DependencyProperty.Register(
        "ButtonsType",
        typeof(MessageBoxWindowButtonsType),
        typeof(MessageBoxWindow),
        new FrameworkPropertyMetadata(MessageBoxWindowButtonsType.Ok,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            new PropertyChangedCallback(OnButtonsTypeChanged)));

    private static void OnButtonsTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as MessageBoxWindow).SetButtonsTypeVisibility((MessageBoxWindowButtonsType)e.NewValue);
    }

    private void SetButtonsTypeVisibility(MessageBoxWindowButtonsType mode)
    {
      switch (mode)
      {
        case MessageBoxWindowButtonsType.OkCancel:
          OkButton.Visibility = Visibility.Collapsed;
          OkCancelButtons.Visibility = Visibility.Visible;
          YesNoButtons.Visibility = Visibility.Collapsed;
          break;
        case MessageBoxWindowButtonsType.YesNo:
          OkButton.Visibility = Visibility.Collapsed;
          OkCancelButtons.Visibility = Visibility.Collapsed;
          YesNoButtons.Visibility = Visibility.Visible;
          break;
        case MessageBoxWindowButtonsType.Ok:
        default:
          OkButton.Visibility = Visibility.Visible;
          OkCancelButtons.Visibility = Visibility.Collapsed;
          YesNoButtons.Visibility = Visibility.Collapsed;
          break;
      }
    }

    #endregion

    #region Button Click

    private void OnLayoutHeaderMouseLeftButtonDown(object sender, EventArgs e)
    {
      DragMove();
    }

    private void OnButtonWindowCloseClick(object sender, RoutedEventArgs args)
    {
      DialogResult = null;
      Close();
    }

    private void OnOkButtonClick(object sender, RoutedEventArgs args)
    {
      DialogResult = true;
      Close();
    }

    private void OnNoButtonClick(object sender, RoutedEventArgs args)
    {
      DialogResult = false;
      Close();
    }

    private void OnCopyButtonClick(object sender, RoutedEventArgs e)
    {
      StringBuilder sb = null;

      sb = new StringBuilder();
      sb.Append("消息(");
      sb.Append(this.Title);
      sb.AppendLine("):");
      sb.AppendLine(this.Message);
      sb.AppendLine("消息详细描述：");
      sb.AppendLine(this.Detail);
      sb.AppendLine("");

      Clipboard.SetText(sb.ToString());
    }

    void OnDetailExpanderCollapsed(object sender, RoutedEventArgs e)
    {
      this.Width = 360;
      this.Height = 200;      
    }

    void OnDetailExpanderExpanded(object sender, RoutedEventArgs e)
    {
      this.Width = 500;
      this.Height = 400;      
    }

    #endregion
  }
}
