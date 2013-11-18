using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Gimela.Presentation.Controls.Converters;

namespace Gimela.Presentation.Controls
{
  public class ColorPicker : Control
  {
    static ColorPicker()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
    }

    public ColorPicker()
    {
      SetupColorBindings();
    }

    public static DependencyProperty ColorProperty = DependencyProperty.Register(
        "Color", typeof(Color), typeof(ColorPicker),
        new PropertyMetadata(Colors.Black, new PropertyChangedCallback(OnColorChanged)));

    public static DependencyProperty RedProperty = DependencyProperty.Register(
        "Red", typeof(byte), typeof(ColorPicker));

    public static DependencyProperty GreenProperty = DependencyProperty.Register(
        "Green", typeof(byte), typeof(ColorPicker));

    public static DependencyProperty BlueProperty = DependencyProperty.Register(
        "Blue", typeof(byte), typeof(ColorPicker));

    public Color Color
    {
      get
      {
        return (Color)GetValue(ColorProperty);
      }
      set
      {
        SetValue(ColorProperty, value);
      }
    }

    public byte Red
    {
      get { return (byte)GetValue(RedProperty); }
      set { SetValue(RedProperty, value); }
    }

    public byte Green
    {
      get { return (byte)GetValue(GreenProperty); }
      set { SetValue(GreenProperty, value); }
    }

    public byte Blue
    {
      get { return (byte)GetValue(BlueProperty); }
      set { SetValue(BlueProperty, value); }
    }

    public static readonly RoutedEvent ColorChangedEvent =
        EventManager.RegisterRoutedEvent("ColorChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPicker));

    public event RoutedPropertyChangedEventHandler<Color> ColorChanged
    {
      add { AddHandler(ColorChangedEvent, value); }
      remove { RemoveHandler(ColorChangedEvent, value); }
    }

    protected virtual void OnColorChanged(Color oldValue, Color newValue)
    {
      RoutedPropertyChangedEventArgs<Color> args = new RoutedPropertyChangedEventArgs<Color>(oldValue, newValue);
      args.RoutedEvent = ColorPicker.ColorChangedEvent;
      RaiseEvent(args);
    }

    private static DependencyProperty InternalColorProperty = DependencyProperty.Register("InternalColor",
        typeof(Color), typeof(ColorPicker),
        new PropertyMetadata(Colors.Black, new PropertyChangedCallback(OnInternalColorChanged)));

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new ColorPickerAutomationPeer(this);
    }

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ColorPicker picker = (ColorPicker)d;

      Color oldValue = (Color)e.OldValue;
      Color newValue = (Color)e.NewValue;

      picker.SetValue(InternalColorProperty, newValue);

      picker.OnColorChanged(oldValue, newValue);

      ColorPickerAutomationPeer peer = UIElementAutomationPeer.FromElement(picker) as ColorPickerAutomationPeer;
      if (peer != null)
      {
        peer.RaiseValueChangedAutomationEvent(oldValue, newValue);
      }
    }

    private static void OnInternalColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ColorPicker picker = (ColorPicker)d;

      Color oldValue = (Color)e.OldValue;
      Color newValue = (Color)e.NewValue;

      picker.SetValue(ColorProperty, newValue);
    }

    private void SetupColorBindings()
    {
      MultiBinding binding = new MultiBinding();

      binding.Converter = new ByteToColorMultiConverter();
      binding.Mode = BindingMode.TwoWay;

      Binding redBinding = new Binding("Red");
      redBinding.Source = this;
      redBinding.Mode = BindingMode.TwoWay;
      binding.Bindings.Add(redBinding);

      Binding greenBinding = new Binding("Green");
      greenBinding.Source = this;
      greenBinding.Mode = BindingMode.TwoWay;
      binding.Bindings.Add(greenBinding);

      Binding blueBinding = new Binding("Blue");
      blueBinding.Source = this;
      blueBinding.Mode = BindingMode.TwoWay;
      binding.Bindings.Add(blueBinding);

      this.SetBinding(InternalColorProperty, binding);
    }
  }
}
