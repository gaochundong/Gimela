using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Gimela.Presentation.Controls
{
  [ContentProperty("Content")]
  public class MDIChild : Control
  {
    #region Constants

    /// <summary>
    /// Width of minimized window.
    /// </summary>
    internal const int MinimizedWidth = 160;

    /// <summary>
    /// Height of minimized window.
    /// </summary>
    internal const int MinimizedHeight = 29;

    #endregion

    #region Dependency Properties

    /// <summary>
    /// Identifies the MDIChild.ContentProperty dependency property.
    /// </summary>
    /// <returns>The identifier for the MDIChild.ContentProperty property.</returns>
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register("Content", typeof(UIElement), typeof(MDIChild));

    /// <summary>
    /// Identifies the MDIChild.TitleProperty dependency property.
    /// </summary>
    /// <returns>The identifier for the MDIChild.TitleProperty property.</returns>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(MDIChild));

    /// <summary>
    /// Identifies the MDIChild.IconProperty dependency property.
    /// </summary>
    /// <returns>The identifier for the MDIChild.IconProperty property.</returns>
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(ImageSource), typeof(MDIChild));

    /// <summary>
    /// Identifies the MDIChild.ShowIconProperty dependency property.
    /// </summary>
    /// <returns>The identifier for the MDIChild.ShowIconProperty property.</returns>
    public static readonly DependencyProperty ShowIconProperty =
        DependencyProperty.Register("ShowIcon", typeof(bool), typeof(MDIChild),
        new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the MDIChild.ResizableProperty dependency property.
    /// </summary>
    /// <returns>The identifier for the MDIChild.ResizableProperty property.</returns>
    public static readonly DependencyProperty ResizableProperty =
        DependencyProperty.Register("Resizable", typeof(bool), typeof(MDIChild),
        new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the MDIChild.FocusedProperty dependency property.
    /// </summary>
    /// <returns>The identifier for the MDIChild.FocusedProperty property.</returns>
    public static readonly DependencyProperty FocusedProperty =
        DependencyProperty.Register("Focused", typeof(bool), typeof(MDIChild),
        new UIPropertyMetadata(false, new PropertyChangedCallback(FocusedValueChanged)));

    /// <summary>
    /// Identifies the MDIChild.MinimizeBoxProperty dependency property.
    /// </summary>
    /// <returns>The identifier for the MDIChild.MinimizeBoxProperty property.</returns>
    public static readonly DependencyProperty MinimizeBoxProperty =
        DependencyProperty.Register("MinimizeBox", typeof(bool), typeof(MDIChild),
        new UIPropertyMetadata(true, new PropertyChangedCallback(MinimizeBoxValueChanged)));

    /// <summary>
    /// Identifies the MDIChild.MaximizeBoxProperty dependency property.
    /// </summary>
    /// <returns>The identifier for the MDIChild.MaximizeBoxProperty property.</returns>
    public static readonly DependencyProperty MaximizeBoxProperty =
        DependencyProperty.Register("MaximizeBox", typeof(bool), typeof(MDIChild),
        new UIPropertyMetadata(true, new PropertyChangedCallback(MaximizeBoxValueChanged)));

    /// <summary>
    /// Identifies the MDIChild.WindowStateProperty dependency property.
    /// </summary>
    /// <returns>The identifier for the MDIChild.WindowStateProperty property.</returns>
    public static readonly DependencyProperty WindowStateProperty =
        DependencyProperty.Register("WindowState", typeof(WindowState), typeof(MDIChild),
        new UIPropertyMetadata(WindowState.Normal, new PropertyChangedCallback(WindowStateValueChanged)));

    /// <summary>
    /// Identifies the MDIChild.PositionProperty dependency property.
    /// </summary>
    /// <returns>The identifier for the MDIChild.PositionProperty property.</returns>
    public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(Point), typeof(MDIChild),
        new UIPropertyMetadata(new Point(0, 0), new PropertyChangedCallback(PositionValueChanged)));

    /// <summary>
    /// Identifies the MDIChild.ButtonsProperty dependency property.
    /// </summary>
    /// <returns>The identifier for the MDIChild.ButtonsProperty property.</returns>
    public static readonly DependencyProperty ButtonsProperty =
        DependencyProperty.Register("Buttons", typeof(Panel), typeof(MDIChild),
        new UIPropertyMetadata(null));


    #endregion

    #region Depedency Events

    /// <summary>
    /// Identifies the MDIChild.ClosingEvent routed event.
    /// </summary>
    /// <returns>The identifier for the MDIChild.ClosingEvent routed event.</returns>
    public static readonly RoutedEvent ClosingEvent =
        EventManager.RegisterRoutedEvent("Closing", RoutingStrategy.Bubble, typeof(MDIClosingEventArgs), typeof(MDIChild));

    /// <summary>
    /// Identifies the MDIChild.ClosedEvent routed event.
    /// </summary>
    /// <returns>The identifier for the MDIChild.ClosedEvent routed event.</returns>
    public static readonly RoutedEvent ClosedEvent =
        EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof(RoutedEventArgs), typeof(MDIChild));

    #endregion

    #region Property Accessors

    /// <summary>
    /// Gets or sets the content.
    /// This is a dependency property.
    /// </summary>
    /// <value>The content.</value>
    public UIElement Content
    {
      get { return (UIElement)GetValue(ContentProperty); }
      set { SetValue(ContentProperty, value); }
    }

    /// <summary>
    /// Gets or sets the window title.
    /// This is a dependency property.
    /// </summary>
    /// <value>The window title.</value>
    public string Title
    {
      get { return (string)GetValue(TitleProperty); }
      set { SetValue(TitleProperty, value); }
    }

    /// <summary>
    /// Gets or sets the window icon.
    /// This is a dependency property.
    /// </summary>
    /// <value>The window icon.</value>
    public ImageSource Icon
    {
      get { return (ImageSource)GetValue(IconProperty); }
      set { SetValue(IconProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to [show the window icon].
    /// This is a dependency property.
    /// </summary>
    /// <value><c>true</c> if [show the window icon]; otherwise, <c>false</c>.</value>
    public bool ShowIcon
    {
      get { return (bool)GetValue(ShowIconProperty); }
      set { SetValue(ShowIconProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the [window is resizable].
    /// This is a dependency property.
    /// </summary>
    /// <value><c>true</c> if [window is resizable]; otherwise, <c>false</c>.</value>
    public bool Resizable
    {
      get { return (bool)GetValue(ResizableProperty); }
      set { SetValue(ResizableProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the [window is focused].
    /// This is a dependency property.
    /// </summary>
    /// <value><c>true</c> if [window is focused]; otherwise, <c>false</c>.</value>
    public bool Focused
    {
      get { return (bool)GetValue(FocusedProperty); }
      set { SetValue(FocusedProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to [show the minimize box button].
    /// This is a dependency property.
    /// </summary>
    /// <value><c>true</c> if [show the minimize box button]; otherwise, <c>false</c>.</value>
    public bool MinimizeBox
    {
      get { return (bool)GetValue(MinimizeBoxProperty); }
      set { SetValue(MinimizeBoxProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to [show the maximize box button].
    /// This is a dependency property.
    /// </summary>
    /// <value><c>true</c> if [show the maximize box button]; otherwise, <c>false</c>.</value>
    public bool MaximizeBox
    {
      get { return (bool)GetValue(MaximizeBoxProperty); }
      set { SetValue(MaximizeBoxProperty, value); }
    }

    /// <summary>
    /// Gets or sets the state of the window.
    /// This is a dependency property.
    /// </summary>
    /// <value>The state of the window.</value>
    public WindowState WindowState
    {
      get { return (WindowState)GetValue(WindowStateProperty); }
      set { SetValue(WindowStateProperty, value); }
    }

    /// <summary>
    /// Gets or sets position of top left corner of window.
    /// This is a dependency property.
    /// </summary>
    public Point Position
    {
      get { return (Point)GetValue(PositionProperty); }
      set { SetValue(PositionProperty, value); }
    }

    /// <summary>
    /// Gets buttons for window in maximized state.
    /// </summary>
    public Panel Buttons
    {
      get { return (Panel)GetValue(ButtonsProperty); }
      private set { SetValue(ButtonsProperty, value); }
    }

    #endregion

    #region Event Accessors

    public event RoutedEventHandler Closing
    {
      add { AddHandler(ClosingEvent, value); }
      remove { RemoveHandler(ClosingEvent, value); }
    }

    public event RoutedEventHandler Closed
    {
      add { AddHandler(ClosedEvent, value); }
      remove { RemoveHandler(ClosedEvent, value); }
    }

    #endregion

    #region Member Declarations

    #region Top Buttons

    private ToggleButton minimizeButton;

    private ToggleButton maximizeButton;

    private Button closeButton;

    private StackPanel buttonsPanel;

    #endregion

    /// <summary>
    /// Gets or sets the container.
    /// </summary>
    /// <value>The container.</value>
    public MDIContainer Container { get; private set; }

    /// <summary>
    /// Dimensions of window in Normal state.
    /// </summary>
    private Rect originalDimension;

    /// <summary>
    /// Position of window in Minimized state.
    /// </summary>
    private Point minimizedPosition = new Point(-1, -1);

    #endregion

    #region Ctors

    /// <summary>
    /// Initializes the <see cref="MDIChild"/> class.
    /// </summary>
    static MDIChild()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIChild), new FrameworkPropertyMetadata(typeof(MDIChild)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MDIChild"/> class.
    /// </summary>
    public MDIChild()
    {
      Focusable = IsTabStop = false;

      Loaded += MdiChild_Loaded;
      GotFocus += MdiChild_GotFocus;
    }

    #endregion

    #region Control Events

    /// <summary>
    /// Handles the Loaded event of the MdiChild control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void MdiChild_Loaded(object sender, RoutedEventArgs e)
    {
      FrameworkElement currentControl = this;

      while (currentControl != null && currentControl.GetType() != typeof(MDIContainer))
        currentControl = (FrameworkElement)currentControl.Parent;

      if (currentControl != null)
        Container = (MDIContainer)currentControl;
      else throw new Exception("Unable to find MdiContainer parent.");
    }

    /// <summary>
    /// Handles the GotFocus event of the MdiChild control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void MdiChild_GotFocus(object sender, RoutedEventArgs e)
    {
      Focus();
    }

    #endregion

    #region Control Overrides

    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      minimizeButton = (ToggleButton)Template.FindName("MinimizeButton", this);
      maximizeButton = (ToggleButton)Template.FindName("MaximizeButton", this);
      closeButton = (Button)Template.FindName("CloseButton", this);
      buttonsPanel = (StackPanel)Template.FindName("ButtonsPanel", this);

      if (minimizeButton != null)
        minimizeButton.Click += new RoutedEventHandler(minimizeButton_Click);

      if (maximizeButton != null)
        maximizeButton.Click += new RoutedEventHandler(maximizeButton_Click);

      if (closeButton != null)
        closeButton.Click += new RoutedEventHandler(closeButton_Click);

      Thumb dragThumb = (Thumb)Template.FindName("DragThumb", this);

      if (dragThumb != null)
      {
        dragThumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
        dragThumb.DragDelta += new DragDeltaEventHandler(dragThumb_DragDelta);

        dragThumb.MouseDoubleClick += (sender, e) =>
        {
          if (WindowState == WindowState.Minimized)
            minimizeButton_Click(null, null);
          else if (WindowState == WindowState.Normal)
            maximizeButton_Click(null, null);
        };
      }

      Thumb resizeLeft = (Thumb)Template.FindName("ResizeLeft", this);
      Thumb resizeTopLeft = (Thumb)Template.FindName("ResizeTopLeft", this);
      Thumb resizeTop = (Thumb)Template.FindName("ResizeTop", this);
      Thumb resizeTopRight = (Thumb)Template.FindName("ResizeTopRight", this);
      Thumb resizeRight = (Thumb)Template.FindName("ResizeRight", this);
      Thumb resizeBottomRight = (Thumb)Template.FindName("ResizeBottomRight", this);
      Thumb resizeBottom = (Thumb)Template.FindName("ResizeBottom", this);
      Thumb resizeBottomLeft = (Thumb)Template.FindName("ResizeBottomLeft", this);

      if (resizeLeft != null)
      {
        resizeLeft.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
        resizeLeft.DragDelta += new DragDeltaEventHandler(ResizeLeft_DragDelta);
      }

      if (resizeTop != null)
      {
        resizeTop.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
        resizeTop.DragDelta += new DragDeltaEventHandler(ResizeTop_DragDelta);
      }

      if (resizeRight != null)
      {
        resizeRight.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
        resizeRight.DragDelta += new DragDeltaEventHandler(ResizeRight_DragDelta);
      }

      if (resizeBottom != null)
      {
        resizeBottom.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
        resizeBottom.DragDelta += new DragDeltaEventHandler(ResizeBottom_DragDelta);
      }

      if (resizeTopLeft != null)
      {
        resizeTopLeft.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);

        resizeTopLeft.DragDelta += (sender, e) =>
        {
          ResizeTop_DragDelta(null, e);
          ResizeLeft_DragDelta(null, e);

          Container.InvalidateSize();
        };
      }

      if (resizeTopRight != null)
      {
        resizeTopRight.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);

        resizeTopRight.DragDelta += (sender, e) =>
        {
          ResizeTop_DragDelta(null, e);
          ResizeRight_DragDelta(null, e);

          Container.InvalidateSize();
        };
      }

      if (resizeBottomRight != null)
      {
        resizeBottomRight.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);

        resizeBottomRight.DragDelta += (sender, e) =>
        {
          ResizeBottom_DragDelta(null, e);
          ResizeRight_DragDelta(null, e);

          Container.InvalidateSize();
        };
      }

      if (resizeBottomLeft != null)
      {
        resizeBottomLeft.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);

        resizeBottomLeft.DragDelta += (sender, e) =>
        {
          ResizeBottom_DragDelta(null, e);
          ResizeLeft_DragDelta(null, e);

          Container.InvalidateSize();
        };
      }

      MinimizeBoxValueChanged(this, new DependencyPropertyChangedEventArgs(MinimizeBoxProperty, true, MinimizeBox));
      MaximizeBoxValueChanged(this, new DependencyPropertyChangedEventArgs(MaximizeBoxProperty, true, MaximizeBox));
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);

      Focused = true;
    }

    #endregion

    #region Top Button Events

    /// <summary>
    /// Handles the Click event of the minimizeButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void minimizeButton_Click(object sender, RoutedEventArgs e)
    {
      if (WindowState == WindowState.Minimized)
        WindowState = WindowState.Normal;
      else
        WindowState = WindowState.Minimized;

      maximizeButton.IsChecked = false;
    }

    /// <summary>
    /// Handles the Click event of the maximizeButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void maximizeButton_Click(object sender, RoutedEventArgs e)
    {
      if (WindowState == WindowState.Maximized)
        WindowState = WindowState.Normal;
      else
        WindowState = WindowState.Maximized;

      minimizeButton.IsChecked = false;
    }

    /// <summary>
    /// Handles the Click event of the closeButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void closeButton_Click(object sender, RoutedEventArgs e)
    {
      MDIClosingEventArgs eventArgs = new MDIClosingEventArgs(ClosingEvent);
      RaiseEvent(eventArgs);

      if (eventArgs.Cancel)
        return;

      Close();

      RaiseEvent(new RoutedEventArgs(ClosedEvent));
    }

    #endregion

    #region Thumb Events

    /// <summary>
    /// Handles the DragStarted event of the Thumb control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragStartedEventArgs"/> instance containing the event data.</param>
    private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
    {
      if (!Focused)
        Focused = true;
    }

    /// <summary>
    /// Handles the DragDelta event of the ResizeLeft control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
    private void ResizeLeft_DragDelta(object sender, DragDeltaEventArgs e)
    {
      if (Width - e.HorizontalChange < MinWidth)
        return;

      double newLeft = e.HorizontalChange;

      if (Position.X + newLeft < 0)
        newLeft = 0 - Position.X;

      Width -= newLeft;
      Position = new Point(Position.X + newLeft, Position.Y);

      if (sender != null)
        Container.InvalidateSize();
    }

    /// <summary>
    /// Handles the DragDelta event of the ResizeTop control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
    private void ResizeTop_DragDelta(object sender, DragDeltaEventArgs e)
    {
      if (Height - e.VerticalChange < MinHeight)
        return;

      double newTop = e.VerticalChange;

      if (Position.Y + newTop < 0)
        newTop = 0 - Position.Y;

      Height -= newTop;
      Position = new Point(Position.X, Position.Y + newTop);

      if (sender != null)
        Container.InvalidateSize();
    }

    /// <summary>
    /// Handles the DragDelta event of the ResizeRight control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
    private void ResizeRight_DragDelta(object sender, DragDeltaEventArgs e)
    {
      if (Width + e.HorizontalChange < MinWidth)
        return;

      Width += e.HorizontalChange;

      if (sender != null)
        Container.InvalidateSize();
    }

    /// <summary>
    /// Handles the DragDelta event of the ResizeBottom control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
    private void ResizeBottom_DragDelta(object sender, DragDeltaEventArgs e)
    {
      if (Height + e.VerticalChange < MinHeight)
        return;

      Height += e.VerticalChange;

      if (sender != null)
        Container.InvalidateSize();
    }

    #endregion

    #region Control Drag Event

    /// <summary>
    /// Handles the DragDelta event of the dragThumb control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
    private void dragThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
      if (WindowState == WindowState.Maximized)
        return;

      double newLeft = Position.X + e.HorizontalChange,
          newTop = Position.Y + e.VerticalChange;

      if (newLeft < 0)
        newLeft = 0;
      if (newTop < 0)
        newTop = 0;

      Position = new Point(newLeft, newTop);

      Container.InvalidateSize();
    }

    #endregion

    #region Dependency Property Events

    /// <summary>
    /// Dependency property event once the position value has changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void PositionValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ((Point)e.NewValue == (Point)e.OldValue)
        return;

      MDIChild mdiChild = (MDIChild)sender;
      Point newPosition = (Point)e.NewValue;

      Canvas.SetTop(mdiChild, newPosition.Y < 0 ? 0 : newPosition.Y);
      Canvas.SetLeft(mdiChild, newPosition.X < 0 ? 0 : newPosition.X);
    }

    /// <summary>
    /// Dependency property event once the focused value has changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void FocusedValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ((bool)e.NewValue == (bool)e.OldValue)
        return;

      MDIChild mdiChild = (MDIChild)sender;
      bool focused = (bool)e.NewValue;
      if (focused)
      {
        mdiChild.Dispatcher.BeginInvoke(new Func<IInputElement, IInputElement>(Keyboard.Focus), System.Windows.Threading.DispatcherPriority.ApplicationIdle, mdiChild.Content);
        mdiChild.RaiseEvent(new RoutedEventArgs(GotFocusEvent, mdiChild));
      }
      else
        mdiChild.RaiseEvent(new RoutedEventArgs(LostFocusEvent, mdiChild));
    }

    /// <summary>
    /// Dependency property event once the minimize box value has changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void MinimizeBoxValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      MDIChild mdiChild = (MDIChild)sender;
      bool visible = (bool)e.NewValue;

      if (visible)
      {
        bool maximizeVisible = true;

        if (mdiChild.maximizeButton != null)
          maximizeVisible = mdiChild.maximizeButton.Visibility == Visibility.Visible;

        if (mdiChild.minimizeButton != null)
          mdiChild.minimizeButton.IsEnabled = true;

        if (!maximizeVisible)
        {
          if (mdiChild.maximizeButton != null)
            mdiChild.minimizeButton.Visibility = Visibility.Visible;

          if (mdiChild.maximizeButton != null)
            mdiChild.maximizeButton.Visibility = Visibility.Visible;
        }
      }
      else
      {
        bool maximizeEnabled = true;

        if (mdiChild.maximizeButton != null)
          maximizeEnabled = mdiChild.maximizeButton.IsEnabled;

        if (mdiChild.minimizeButton != null)
          mdiChild.minimizeButton.IsEnabled = false;

        if (!maximizeEnabled)
        {
          if (mdiChild.minimizeButton != null)
            mdiChild.minimizeButton.Visibility = Visibility.Hidden;

          if (mdiChild.maximizeButton != null)
            mdiChild.maximizeButton.Visibility = Visibility.Hidden;
        }
      }
    }

    /// <summary>
    /// Dependency property event once the maximize box value has changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void MaximizeBoxValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      MDIChild mdiChild = (MDIChild)sender;
      bool visible = (bool)e.NewValue;

      if (visible)
      {
        bool minimizeVisible = true;

        if (mdiChild.minimizeButton != null)
          minimizeVisible = mdiChild.minimizeButton.Visibility == Visibility.Visible;

        if (mdiChild.maximizeButton != null)
          mdiChild.maximizeButton.IsEnabled = true;

        if (!minimizeVisible)
        {
          if (mdiChild.maximizeButton != null)
            mdiChild.minimizeButton.Visibility = Visibility.Visible;

          if (mdiChild.maximizeButton != null)
            mdiChild.maximizeButton.Visibility = Visibility.Visible;
        }
      }
      else
      {
        bool minimizeEnabled = true;

        if (mdiChild.minimizeButton != null)
          minimizeEnabled = mdiChild.minimizeButton.IsEnabled;

        if (mdiChild.maximizeButton != null)
          mdiChild.maximizeButton.IsEnabled = false;

        if (!minimizeEnabled)
        {
          if (mdiChild.maximizeButton != null)
            mdiChild.minimizeButton.Visibility = Visibility.Hidden;

          if (mdiChild.maximizeButton != null)
            mdiChild.maximizeButton.Visibility = Visibility.Hidden;
        }
      }
    }

    /// <summary>
    /// Dependency property event once the windows state value has changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void WindowStateValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      MDIChild mdiChild = (MDIChild)sender;

      WindowState previousWindowState = (WindowState)e.OldValue;
      WindowState windowState = (WindowState)e.NewValue;

      if (mdiChild.Container == null ||
          previousWindowState == windowState)
        return;

      if (previousWindowState == WindowState.Maximized)
      {
        for (int i = 0; i < mdiChild.Container.Children.Count; i++)
        {
          if (mdiChild.Container.Children[i] != mdiChild &&
                  mdiChild.Container.Children[i].WindowState == WindowState.Maximized &&
                  mdiChild.Container.Children[i].MaximizeBox)
            mdiChild.Container.Children[i].WindowState = WindowState.Normal;
        }

        ScrollViewer sv = (ScrollViewer)((Grid)mdiChild.Container.Content).Children[1];
        sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

        mdiChild.Buttons.Children.Clear();
        mdiChild.buttonsPanel.Children.Add(mdiChild.minimizeButton);
        mdiChild.buttonsPanel.Children.Add(mdiChild.maximizeButton);
        mdiChild.buttonsPanel.Children.Add(mdiChild.closeButton);
      }

      if (previousWindowState == WindowState.Minimized)
      {
        mdiChild.minimizedPosition = mdiChild.Position;
      }

      switch (windowState)
      {
        case WindowState.Normal:
          {
            mdiChild.Position = new Point(mdiChild.originalDimension.X, mdiChild.originalDimension.Y);

            mdiChild.Width = mdiChild.originalDimension.Width;
            mdiChild.Height = mdiChild.originalDimension.Height;
          }
          break;
        case WindowState.Minimized:
          {
            if (previousWindowState == WindowState.Normal)
              mdiChild.originalDimension = new Rect(mdiChild.Position.X, mdiChild.Position.Y, mdiChild.ActualWidth, mdiChild.ActualHeight);

            double newLeft, newTop;
            if (mdiChild.minimizedPosition.X >= 0 || mdiChild.minimizedPosition.Y >= 0)
            {
              newLeft = mdiChild.minimizedPosition.X;
              newTop = mdiChild.minimizedPosition.Y;
            }
            else
            {
              int minimizedWindows = 0;
              for (int i = 0; i < mdiChild.Container.Children.Count; i++)
                if (mdiChild.Container.Children[i] != mdiChild && mdiChild.Container.Children[i].WindowState == WindowState.Minimized)
                  minimizedWindows++;
              int capacity = Convert.ToInt32(mdiChild.Container.ActualWidth) / MDIChild.MinimizedWidth,
                  row = minimizedWindows / capacity + 1,
                  col = minimizedWindows % capacity;
              newTop = mdiChild.Container.InnerHeight - MDIChild.MinimizedHeight * row;
              newLeft = MDIChild.MinimizedWidth * col;
            }

            mdiChild.Position = new Point(newLeft, newTop);

            mdiChild.Width = MDIChild.MinimizedWidth;
            mdiChild.Height = MDIChild.MinimizedHeight;
          }
          break;
        case WindowState.Maximized:
          {
            if (previousWindowState == WindowState.Normal)
              mdiChild.originalDimension = new Rect(mdiChild.Position.X, mdiChild.Position.Y, mdiChild.ActualWidth, mdiChild.ActualHeight);

            mdiChild.buttonsPanel.Children.Clear();
            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };
            sp.Children.Add(mdiChild.minimizeButton);
            sp.Children.Add(mdiChild.maximizeButton);
            sp.Children.Add(mdiChild.closeButton);
            mdiChild.Buttons = sp;

            mdiChild.Position = new Point(0, 0);
            mdiChild.Width = mdiChild.Container.ActualWidth;
            mdiChild.Height = mdiChild.Container.InnerHeight - 2; // ContentBorder.BorderThickness="1" in template

            if (mdiChild.Container.AllowWindowStateMax)
            {
              MDIContainer mdiContainer = mdiChild.Container;
              mdiContainer.AllowWindowStateMax = false;

              for (int i = 0; i < mdiContainer.Children.Count; i++)
                if (mdiContainer.Children[i] != mdiChild)
                {
                  if (mdiContainer.Children[i].WindowState == WindowState.Normal &&
                      mdiContainer.Children[i].MaximizeBox)
                    mdiContainer.Children[i].WindowState = WindowState.Maximized;
                  else if (mdiContainer.Children[i].WindowState == WindowState.Maximized)
                    mdiContainer.Children[i].Height = mdiContainer.InnerHeight;
                }

              ScrollViewer sv = (ScrollViewer)((Grid)mdiContainer.Content).Children[1];
              sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
              sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

              mdiChild.Focus();
              mdiContainer.InvalidateSize();
              mdiContainer.AllowWindowStateMax = true;
            }
          }
          break;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Set focus to the child window and brings into view.
    /// </summary>
    public new void Focus()
    {
      MDIContainer.Focus(this);
    }

    /// <summary>
    /// Manually closes the child window.
    /// </summary>
    public virtual void Close()
    {
      if (Buttons != null)
      {
        Buttons.Children.Clear();
      }
      if (Container != null)
      {
        Container.Children.Remove(this);
      }
    }

    #endregion
  }
}
