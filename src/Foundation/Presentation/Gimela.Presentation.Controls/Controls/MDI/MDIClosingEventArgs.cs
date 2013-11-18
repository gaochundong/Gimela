using System.Windows;

namespace Gimela.Presentation.Controls
{
  public class MDIClosingEventArgs : RoutedEventArgs
  {
    /// <summary>
    /// Gets or sets a value indicating whether the event should be canceled.
    /// </summary>
    /// <value><c>true</c> if the event should be canceled; otherwise, <value><c>false</c>.</value>
    public bool Cancel { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MDIClosingEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    public MDIClosingEventArgs(RoutedEvent routedEvent)
      : base(routedEvent)
    {
    }
  }
}
