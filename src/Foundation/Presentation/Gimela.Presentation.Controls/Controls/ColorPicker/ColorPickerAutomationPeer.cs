using System;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Media;

namespace Gimela.Presentation.Controls
{
  public class ColorPickerAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
  {
    public ColorPickerAutomationPeer(ColorPicker picker)
      : base(picker)
    {
    }

    protected override string GetClassNameCore()
    {
      return "ColorPicker";
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Custom;
    }

    public override object GetPattern(PatternInterface patternInterface)
    {
      if (patternInterface == PatternInterface.Value)
      {
        return this;
      }
      return base.GetPattern(patternInterface);
    }

    internal void RaiseValueChangedAutomationEvent(Color oldColor, Color newColor)
    {
      base.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldColor.ToString(), newColor.ToString());
    }

    public bool IsReadOnly
    {
      get { return true; }
    }

    public void SetValue(string value)
    {
      throw new NotSupportedException();
    }

    public string Value
    {
      get
      {
        return MyOwner.Color.ToString();
      }
    }

    private ColorPicker MyOwner
    {
      get
      {
        return (ColorPicker)base.Owner;
      }
    }
  }

}
