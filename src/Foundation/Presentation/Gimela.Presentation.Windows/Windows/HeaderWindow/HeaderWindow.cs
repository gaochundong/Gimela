using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Gimela.Presentation.Controls;

namespace Gimela.Presentation.Windows
{
  /// <summary>
  /// 带有标题栏的窗体
  /// </summary>
  [TemplatePart(Name = HeaderContainerName, Type = typeof(FrameworkElement))]
  [TemplatePart(Name = MinimizeButtonName, Type = typeof(Button))]
  [TemplatePart(Name = RestoreButtonName, Type = typeof(ToggleButton))]
  [TemplatePart(Name = CloseButtonName, Type = typeof(Button))]
  [TemplatePart(Name = ChildContainerName, Type = typeof(Grid))]
  [TemplatePart(Name = MenuName, Type = typeof(Menu))]
  public class HeaderWindow : Window, IControlContainable
  {
    #region Template Part Name

    private const string HeaderContainerName = "PART_HeaderContainer";
    private const string MinimizeButtonName = "PART_MinimizeButton";
    private const string RestoreButtonName = "PART_RestoreButton";
    private const string CloseButtonName = "PART_CloseButton";
    private const string ChildContainerName = "PART_ChildContainer";
    private const string MenuName = "PART_Menu";

    #endregion

    #region Private Fields

    private FrameworkElement headerContainer;
    private Button minimizeButton;
    private ToggleButton restoreButton;
    private Button closeButton;
    private Grid childContainer;
    private Menu menu;

    #endregion

    #region Ctors

    static HeaderWindow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderWindow), new FrameworkPropertyMetadata(typeof(HeaderWindow)));
    }

    #endregion

    #region Dependency Properties

    public static readonly DependencyProperty IsShowDefaultHeaderProperty =
        DependencyProperty.Register("IsShowDefaultHeader", typeof(bool), typeof(HeaderWindow), new FrameworkPropertyMetadata(true));

    public static readonly DependencyProperty IsShowResizeGripProperty =
        Resizer.IsShowResizeGripProperty.AddOwner(typeof(HeaderWindow));

    public static readonly DependencyProperty CanResizeProperty =
        DependencyProperty.Register("CanResize", typeof(bool), typeof(HeaderWindow), new FrameworkPropertyMetadata(true));

    public static readonly DependencyProperty IsFullScreenMaximizeProperty =
        DependencyProperty.Register("IsFullScreenMaximize", typeof(bool), typeof(HeaderWindow), new FrameworkPropertyMetadata(false));

    public static readonly DependencyProperty IsShowMenuProperty =
        DependencyProperty.Register("IsShowMenu", typeof(bool), typeof(HeaderWindow), new FrameworkPropertyMetadata(false));

    public bool IsShowDefaultHeader
    {
      get { return (bool)GetValue(IsShowDefaultHeaderProperty); }
      set { SetValue(IsShowDefaultHeaderProperty, value); }
    }

    public bool IsShowResizeGrip
    {
      get { return (bool)GetValue(IsShowResizeGripProperty); }
      set { SetValue(IsShowResizeGripProperty, value); }
    }

    public bool CanResize
    {
      get { return (bool)GetValue(CanResizeProperty); }
      set { SetValue(CanResizeProperty, value); }
    }

    public bool IsFullScreenMaximize
    {
      get { return (bool)GetValue(IsFullScreenMaximizeProperty); }
      set { SetValue(IsFullScreenMaximizeProperty, value); }
    }

    public bool IsShowMenu
    {
      get { return (bool)GetValue(IsShowMenuProperty); }
      set { SetValue(IsShowMenuProperty, value); }
    }

    #endregion

    #region Apply Template

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      headerContainer = GetTemplateChild<FrameworkElement>(HeaderContainerName);
      headerContainer.MouseLeftButtonDown += OnHeaderContainerMouseLeftButtonDown;

      closeButton = GetTemplateChild<Button>(CloseButtonName);
      closeButton.Click += delegate { Close(); };

      restoreButton = GetTemplateChild<ToggleButton>(RestoreButtonName);
      restoreButton.Checked += delegate { ChangeWindowState(WindowState.Maximized); };
      restoreButton.Unchecked += delegate { ChangeWindowState(WindowState.Normal); };

      OnStateChanged(EventArgs.Empty);

      minimizeButton = GetTemplateChild<Button>(MinimizeButtonName);
      minimizeButton.Click += delegate { ChangeWindowState(WindowState.Minimized); };

      childContainer = GetTemplateChild<Grid>(ChildContainerName);

      menu = GetTemplateChild<Menu>(MenuName);
    }

    private T GetTemplateChild<T>(string childName) where T : FrameworkElement, new()
    {
      return (GetTemplateChild(childName) as T) ?? new T();
    }

    private void OnHeaderContainerMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 1)
      {
        DragMove();
      }
      else
      {
        restoreButton.IsChecked = !restoreButton.IsChecked;
      }
    }

    private void ChangeWindowState(WindowState state)
    {
      if (state == WindowState.Maximized)
      {
        if (!IsFullScreenMaximize && IsLocationOnPrimaryScreen())
        {
          MaxHeight = SystemParameters.WorkArea.Height;
          MaxWidth = SystemParameters.WorkArea.Width;
        }
        else
        {
          MaxHeight = double.PositiveInfinity;
          MaxWidth = double.PositiveInfinity;
        }
      }

      WindowState = state;
    }

    protected override void OnStateChanged(EventArgs e)
    {
      try
      {
        if (WindowState == WindowState.Minimized)
        {
          restoreButton.IsChecked = null;
        }
        else
        {
          restoreButton.IsChecked = WindowState == WindowState.Maximized;
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Trace.WriteLine(ex.ToString());
      }

      base.OnStateChanged(e);
    }

    private bool IsLocationOnPrimaryScreen()
    {
      return Left < SystemParameters.PrimaryScreenWidth && Top < SystemParameters.PrimaryScreenHeight;
    }

    #endregion

    #region IControlContainerable Members

    public Panel Container
    {
      get
      {
        return childContainer;
      }
    }

    public Menu Menu
    {
      get
      {
        return this.menu;
      }
    }

    #endregion
  }
}
