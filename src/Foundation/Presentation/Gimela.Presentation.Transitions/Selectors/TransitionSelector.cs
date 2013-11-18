using System.Windows;

namespace Gimela.Presentation.Transitions
{
    /// <summary>
    /// Allows different transitions to run based on the old and new contents
    /// Override the SelectTransition method to return the transition to apply
    /// </summary>
    public class TransitionSelector : DependencyObject
    {
        public virtual Transition SelectTransition(object oldContent, object newContent)
        {
            return null;
        }
    }
}
