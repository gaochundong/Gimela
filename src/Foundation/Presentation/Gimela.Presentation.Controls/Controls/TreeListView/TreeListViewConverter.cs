using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Gimela.Presentation.Controls
{
  /// <summary>
  /// Represents a convert that can calculate the indentation of any element in a class derived from TreeView.
  /// </summary>
  public class TreeListViewConverter : IValueConverter
  {
    public const double Indentation = 10;

    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      //If the value is null, don't return anything
      if (value == null) return null;

      //Convert the item to a double
      if (targetType == typeof(double) && typeof(DependencyObject).IsAssignableFrom(value.GetType()))
      {
        //Cast the item as a DependencyObject
        DependencyObject Element = value as DependencyObject;

        //Create a level counter with value set to -1
        int Level = -1;

        //Move up the visual tree and count the number of TreeViewItem's.
        for (; Element != null; Element = VisualTreeHelper.GetParent(Element))
          //Check whether the current elemeent is a TreeViewItem
          if (typeof(TreeViewItem).IsAssignableFrom(Element.GetType()))
            //Increase the level counter
            Level++;

        //Return the indentation as a double
        return Indentation * Level;
      }

      //Type conversion is not supported
      throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Cannot convert from <{0}> to <{1}> using <TreeListViewConverter>.", value.GetType(), targetType));
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException("This method is not supported.");
    }

    #endregion
  }
}
