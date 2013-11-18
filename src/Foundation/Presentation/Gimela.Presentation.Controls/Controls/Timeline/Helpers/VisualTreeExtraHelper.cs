using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace Gimela.Presentation.Controls.Timeline
{
    public static class VisualTreeExtraHelper
    {
        /// <summary>
        /// Don't want to change signature of below, but this actually finds a child of a type with a name
        /// </summary>
        /// <typeparam name="childItem"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static childItem FindVisualChildByName<childItem>(DependencyObject obj, string name)
          where childItem : FrameworkElement
        {
            childItem childOfChild = null;

            DependencyObject child = null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                {
                    if (String.Compare(
                      (child as childItem).Name,
                      name,
                      StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        return (childItem)child;
                    }
                }

                else
                {
                    childOfChild = FindVisualChildByName<childItem>(child, name);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }

        /// <summary>
        /// Don't want to break existing code right now, but the name param does nothing in this method
        /// </summary>
        /// <typeparam name="childItem"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Obsolete]
        public static childItem FindVisualChild<childItem>(DependencyObject obj, string name)
          where childItem : DependencyObject
        {
            return FindVisualChild<childItem>(obj);
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj)
          where childItem : DependencyObject
        {
            childItem childOfChild = null;

            DependencyObject child = null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }

                else
                {
                    childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)

                        return childOfChild;
                }
            }

            return null;
        }

        /// <summary>
        /// Find a parent object of type parentItem in the visual tree
        /// </summary>
        /// <typeparam name="TParentType"></typeparam>
        /// <param name="outerDepObj"></param>
        /// <returns></returns>
        public static TParentType FindVisualParent<TParentType>(DependencyObject outerDepObj) where TParentType : DependencyObject
        {
            DependencyObject dObj = VisualTreeHelper.GetParent(outerDepObj);
            if (dObj == null)
                return null;

            if (dObj is TParentType)
                return dObj as TParentType;

            while ((dObj = VisualTreeHelper.GetParent(dObj)) != null)
            {
                if (dObj is TParentType)
                    return dObj as TParentType;
            }

            return null;
        }

        /// <summary>
        /// Find a parent object of type parentItem in the visual tree, also searches logical tree at any level missing a visual parent.
        /// </summary>
        /// <remarks>
        /// This could be made even more extensive to search both visual and logical and not look only for dependency objects in logical
        /// tree, but that is for another day, since this fixed my current bug.
        /// </remarks>
        /// <typeparam name="TParentType"></typeparam>
        /// <param name="outerDepObj"></param>
        /// <returns></returns>
        public static TParentType FindVisualParentExtended<TParentType>(DependencyObject outerDepObj) where TParentType : DependencyObject
        {
            var visualParent = VisualTreeHelper.GetParent(outerDepObj);
            if (visualParent is TParentType) return visualParent as TParentType;

            if (visualParent == null)
            {
                var logicalParent = LogicalTreeHelper.GetParent(outerDepObj);
                if (logicalParent == null) return null;
                if (logicalParent is TParentType) return logicalParent as TParentType;
                visualParent = FindVisualParentExtended<TParentType>(logicalParent);
            }
            else
            {
                visualParent = FindVisualParentExtended<TParentType>(visualParent);
            }

            return visualParent as TParentType;
        }

        /// <summary>
        /// Clone an element
        /// </summary>
        /// <param name="elementToClone"></param>
        /// <returns></returns>
        public static object CloneElement(object elementToClone)
        {
            string xaml = XamlWriter.Save(elementToClone);
            return XamlReader.Load(new XmlTextReader(new StringReader(xaml)));
        }
    }
}
