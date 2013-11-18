using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Gimela.Presentation.Controls
{
  [ContentProperty("Children")]
  public class MDIContainer : UserControl
  {
    #region Constants

    /// <summary>
    /// 窗体出现时排列的偏移量
    /// </summary>
    private const int WindowOffset = 25;

    #endregion

    #region Member Declarations

    /// <summary>
    /// Gets or sets the child elements.
    /// </summary>
    /// <value>The child elements.</value>
    public ObservableCollection<MDIChild> Children { get; set; }

    private Canvas _windowCanvas;

    /// <summary>
    /// Contains user-specified element.
    /// </summary>
    private Border _menuBorder;

    /// <summary>
    /// Contains window buttons in maximized mode.
    /// </summary>
    private Border _buttonsBorder;

    /// <summary>
    /// Container for _buttons and _menu.
    /// </summary>
    private Panel _topPanel;

    /// <summary>
    /// Offset for new window.
    /// </summary>
    private double _windowOffset;

    /// <summary>
    /// Allows setting WindowState of all windows to Maximized.
    /// </summary>
    internal bool AllowWindowStateMax;

    #endregion

    #region Dependency Properties

    public static readonly DependencyProperty MDIMenuProperty =
        DependencyProperty.Register("MDIMenu", typeof(UIElement), typeof(MDIContainer),
        new UIPropertyMetadata(null, new PropertyChangedCallback(MDIMenuValueChanged)));

    public static readonly DependencyProperty MDILayoutProperty =
        DependencyProperty.Register("MDILayout", typeof(MDILayoutType), typeof(MDIContainer),
        new UIPropertyMetadata(MDILayoutType.ArrangeIcons, new PropertyChangedCallback(MDILayoutValueChanged)));

    private static void MDIMenuValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      MDIContainer mdiContainer = (MDIContainer)sender;
      UIElement menu = (UIElement)e.NewValue;

      mdiContainer._menuBorder.Child = menu;
    }

    private static void MDILayoutValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      MDIContainer mdiContainer = (MDIContainer)sender;
      MDILayoutType value = (MDILayoutType)e.NewValue;

      if (value == MDILayoutType.ArrangeIcons ||
          mdiContainer.Children.Count < 1)
        return;

      // 1. WindowState.Maximized -> WindowState.Normal
      List<MDIChild> minimizedWindows = new List<MDIChild>(),
          normalWindows = new List<MDIChild>();
      foreach (MDIChild mdiChild in mdiContainer.Children)
        switch (mdiChild.WindowState)
        {
          case WindowState.Minimized:
            minimizedWindows.Add(mdiChild);
            break;
          case WindowState.Maximized:
            mdiChild.WindowState = WindowState.Normal;
            normalWindows.Add(mdiChild);
            break;
          default:
            normalWindows.Add(mdiChild);
            break;
        }

      minimizedWindows.Sort(new MDIChildComparer());
      normalWindows.Sort(new MDIChildComparer());

      // 2. Arrange minimized windows
      double containerHeight = mdiContainer.InnerHeight;
      for (int i = 0; i < minimizedWindows.Count; i++)
      {
        MDIChild mdiChild = minimizedWindows[i];
        int capacity = Convert.ToInt32(mdiContainer.ActualWidth) / MDIChild.MinimizedWidth,
            row = i / capacity + 1,
            col = i % capacity;
        containerHeight = mdiContainer.InnerHeight - MDIChild.MinimizedHeight * row;
        double newLeft = MDIChild.MinimizedWidth * col;
        mdiChild.Position = new Point(newLeft, containerHeight);
      }

      // 3. Resize & arrange normal windows
      switch (value)
      {
        case MDILayoutType.Cascade:
          {
            double newWidth = mdiContainer.ActualWidth * 0.58, // should be non-linear formula here
                newHeight = containerHeight * 0.67,
                windowOffset = 0;
            foreach (MDIChild mdiChild in normalWindows)
            {
              if (mdiChild.Resizable)
              {
                mdiChild.Width = newWidth;
                mdiChild.Height = newHeight;
              }
              mdiChild.Position = new Point(windowOffset, windowOffset);

              windowOffset += WindowOffset;
              if (windowOffset + mdiChild.Width > mdiContainer.ActualWidth)
                windowOffset = 0;
              if (windowOffset + mdiChild.Height > containerHeight)
                windowOffset = 0;
            }
          }
          break;
        case MDILayoutType.TileHorizontal:
          {
            int cols = (int)Math.Sqrt(normalWindows.Count),
                rows = normalWindows.Count / cols;

            List<int> col_count = new List<int>(); // windows per column
            for (int i = 0; i < cols; i++)
            {
              if (normalWindows.Count % cols > cols - i - 1)
                col_count.Add(rows + 1);
              else
                col_count.Add(rows);
            }

            double newWidth = mdiContainer.ActualWidth / cols,
                newHeight = containerHeight / col_count[0],
                offsetTop = 0,
                offsetLeft = 0;

            for (int i = 0, col_index = 0, prev_count = 0; i < normalWindows.Count; i++)
            {
              if (i >= prev_count + col_count[col_index])
              {
                prev_count += col_count[col_index++];
                offsetLeft += newWidth;
                offsetTop = 0;
                newHeight = containerHeight / col_count[col_index];
              }

              MDIChild mdiChild = normalWindows[i];
              if (mdiChild.Resizable)
              {
                mdiChild.Width = newWidth;
                mdiChild.Height = newHeight;
              }
              mdiChild.Position = new Point(offsetLeft, offsetTop);
              offsetTop += newHeight;
            }
          }
          break;
        case MDILayoutType.TileVertical:
          {
            int rows = (int)Math.Sqrt(normalWindows.Count),
                cols = normalWindows.Count / rows;

            List<int> col_count = new List<int>(); // windows per column
            for (int i = 0; i < cols; i++)
            {
              if (normalWindows.Count % cols > cols - i - 1)
                col_count.Add(rows + 1);
              else
                col_count.Add(rows);
            }

            double newWidth = mdiContainer.ActualWidth / cols,
                newHeight = containerHeight / col_count[0],
                offsetTop = 0,
                offsetLeft = 0;

            for (int i = 0, col_index = 0, prev_count = 0; i < normalWindows.Count; i++)
            {
              if (i >= prev_count + col_count[col_index])
              {
                prev_count += col_count[col_index++];
                offsetLeft += newWidth;
                offsetTop = 0;
                newHeight = containerHeight / col_count[col_index];
              }

              MDIChild mdiChild = normalWindows[i];
              if (mdiChild.Resizable)
              {
                mdiChild.Width = newWidth;
                mdiChild.Height = newHeight;
              }
              mdiChild.Position = new Point(offsetLeft, offsetTop);
              offsetTop += newHeight;
            }
          }
          break;
      }
      mdiContainer.InvalidateSize();
      mdiContainer.MDILayout = MDILayoutType.ArrangeIcons;
    }

    /// <summary>
    /// MDI菜单
    /// </summary>
    public UIElement MDIMenu
    {
      get { return (UIElement)GetValue(MDIMenuProperty); }
      set { SetValue(MDIMenuProperty, value); }
    }

    /// <summary>
    /// MDI布局
    /// </summary>
    public MDILayoutType MDILayout
    {
      get { return (MDILayoutType)GetValue(MDILayoutProperty); }
      set { SetValue(MDILayoutProperty, value); }
    }

    /// <summary>
    /// Gets correct canvas height for internal usage.
    /// </summary>
    internal double InnerHeight
    {
      get { return ActualHeight - _topPanel.ActualHeight; }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="MDIContainer"/> class.
    /// </summary>
    public MDIContainer()
    {
      Children = new ObservableCollection<MDIChild>();
      Children.CollectionChanged += Children_CollectionChanged;

      Grid gr = new Grid();
      gr.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
      gr.RowDefinitions.Add(new RowDefinition());

      _topPanel = new DockPanel { Background = Brushes.Transparent };
      _topPanel.Children.Add(_menuBorder = new Border());
      DockPanel.SetDock(_menuBorder, Dock.Left);
      _topPanel.Children.Add(_buttonsBorder = new Border());
      DockPanel.SetDock(_buttonsBorder, Dock.Right);
      _topPanel.SizeChanged += MdiContainer_SizeChanged;
      _topPanel.Children.Add(new UIElement());
      gr.Children.Add(_topPanel);

      ScrollViewer sv = new ScrollViewer
      {
        Content = _windowCanvas = new Canvas(),
        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
        VerticalScrollBarVisibility = ScrollBarVisibility.Auto
      };
      gr.Children.Add(sv);
      Grid.SetRow(sv, 1);
      Content = gr;

      Loaded += MdiContainer_Loaded;
      SizeChanged += MdiContainer_SizeChanged;
      KeyDown += new System.Windows.Input.KeyEventHandler(MdiContainer_KeyDown);
      AllowWindowStateMax = true;
    }

    #endregion

    #region ObservableCollection Events

    private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          {
            MDIChild mdiChild = Children[e.NewStartingIndex],
                topChild = GetTopChild();

            if (topChild != null && topChild.WindowState == WindowState.Maximized)
              mdiChild.Loaded += (s, a) => mdiChild.WindowState = WindowState.Maximized;

            mdiChild.Position = new Point(_windowOffset, _windowOffset);

            _windowCanvas.Children.Add(mdiChild);
            mdiChild.Loaded += (s, a) => Focus(mdiChild);

            _windowOffset += WindowOffset;
            if (_windowOffset + mdiChild.Width > ActualWidth)
              _windowOffset = 0;
            if (_windowOffset + mdiChild.Height > ActualHeight)
              _windowOffset = 0;
          }
          break;
        case NotifyCollectionChangedAction.Remove:
          {
            _windowCanvas.Children.Remove((MDIChild)e.OldItems[0]);
            Focus(GetTopChild());
          }
          break;
        case NotifyCollectionChangedAction.Reset:
          _windowCanvas.Children.Clear();
          break;
      }
      InvalidateSize();
    }

    #endregion

    #region Container Events

    /// <summary>
    /// Handles the Loaded event of the MdiContainer control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void MdiContainer_Loaded(object sender, RoutedEventArgs e)
    {
      Window wnd = Window.GetWindow(this);
      if (wnd != null)
      {
        wnd.Activated += MdiContainer_Activated;
        wnd.Deactivated += MdiContainer_Deactivated;
      }

      _windowCanvas.Width = _windowCanvas.ActualWidth;
      _windowCanvas.Height = _windowCanvas.ActualHeight;

      _windowCanvas.VerticalAlignment = VerticalAlignment.Top;
      _windowCanvas.HorizontalAlignment = HorizontalAlignment.Left;

      InvalidateSize();
    }

    /// <summary>
    /// Handles the Activated event of the MdiContainer control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void MdiContainer_Activated(object sender, EventArgs e)
    {
      if (Children.Count == 0)
        return;

      int index = 0, maxZindex = Panel.GetZIndex(Children[0]);
      for (int i = 0; i < Children.Count; i++)
      {
        int zindex = Panel.GetZIndex(Children[i]);
        if (zindex > maxZindex)
        {
          maxZindex = zindex;
          index = i;
        }
      }
      Children[index].Focused = true;
    }

    /// <summary>
    /// Handles the Deactivated event of the MdiContainer control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void MdiContainer_Deactivated(object sender, EventArgs e)
    {
      if (Children.Count == 0)
        return;

      for (int i = 0; i < _windowCanvas.Children.Count; i++)
        Children[i].Focused = false;
    }

    /// <summary>
    /// Handles the SizeChanged event of the MdiContainer control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
    private void MdiContainer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (Children.Count == 0)
        return;

      for (int i = 0; i < Children.Count; i++)
      {
        MDIChild mdiChild = Children[i];
        if (mdiChild.WindowState == WindowState.Maximized)
        {
          mdiChild.Width = ActualWidth;
          mdiChild.Height = ActualHeight;
        }
        if (mdiChild.WindowState == WindowState.Minimized)
        {
          mdiChild.Position = new Point(mdiChild.Position.X, mdiChild.Position.Y + e.NewSize.Height - e.PreviousSize.Height);
        }
      }
    }

    private static void MdiContainer_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      MDIContainer mdiContainer = (MDIContainer)sender;
      if (mdiContainer.Children.Count < 2)
        return;
      switch (e.Key)
      {
        case Key.Tab:
          if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
          {
            int minZindex = Panel.GetZIndex(mdiContainer.Children[0]);
            foreach (MDIChild mdiChild in mdiContainer.Children)
              if (Panel.GetZIndex(mdiChild) < minZindex)
                minZindex = Panel.GetZIndex(mdiChild);
            Panel.SetZIndex(mdiContainer.GetTopChild(), minZindex - 1);
            mdiContainer.GetTopChild().Focus();
            e.Handled = true;
          }
          break;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Focuses a child and brings it into view.
    /// </summary>
    /// <param name="mdiChild">The MDI child.</param>
    internal static void Focus(MDIChild mdiChild)
    {
      if (mdiChild == null)
        return;

      mdiChild.Container._buttonsBorder.Child = mdiChild.Buttons;

      int maxZindex = 0;
      for (int i = 0; i < mdiChild.Container.Children.Count; i++)
      {
        int zindex = Panel.GetZIndex(mdiChild.Container.Children[i]);
        if (zindex > maxZindex)
          maxZindex = zindex;
        if (mdiChild.Container.Children[i] != mdiChild)
        {
          mdiChild.Container.Children[i].Focused = false;
        }
        else
          mdiChild.Focused = true;
      }
      Panel.SetZIndex(mdiChild, maxZindex + 1);
    }

    /// <summary>
    /// Invalidates the size checking to see if the furthest
    /// child point exceeds the current height and width.
    /// </summary>
    internal void InvalidateSize()
    {
      Point largestPoint = new Point(0, 0);

      for (int i = 0; i < Children.Count; i++)
      {
        MDIChild mdiChild = Children[i];

        Point farPosition = new Point(mdiChild.Position.X + mdiChild.Width, mdiChild.Position.Y + mdiChild.Height);

        if (farPosition.X > largestPoint.X)
          largestPoint.X = farPosition.X;

        if (farPosition.Y > largestPoint.Y)
          largestPoint.Y = farPosition.Y;
      }

      if (_windowCanvas.Width != largestPoint.X)
        _windowCanvas.Width = largestPoint.X;

      if (_windowCanvas.Height != largestPoint.Y)
        _windowCanvas.Height = largestPoint.Y;
    }

    /// <summary>
    /// Gets MdiChild with maximum ZIndex.
    /// </summary>
    internal MDIChild GetTopChild()
    {
      if (_windowCanvas.Children.Count < 1)
        return null;

      int index = 0, maxZindex = Panel.GetZIndex(_windowCanvas.Children[0]);
      for (int i = 1, zindex; i < _windowCanvas.Children.Count; i++)
      {
        zindex = Panel.GetZIndex(_windowCanvas.Children[i]);
        if (zindex > maxZindex)
        {
          maxZindex = zindex;
          index = i;
        }
      }
      return (MDIChild)_windowCanvas.Children[index];
    }

    #endregion
  }
}
