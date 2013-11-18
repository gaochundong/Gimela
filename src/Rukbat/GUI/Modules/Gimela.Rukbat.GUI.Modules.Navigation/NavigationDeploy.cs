using System;
using Gimela.Presentation.Controls;

namespace Gimela.Rukbat.GUI.Modules.Navigation
{
  public static class NavigationDeploy
  {
    public static MDIContainer Container { get; private set; }

    public static void SetContainer(MDIContainer container)
    {
      if (container == null)
        throw new ArgumentNullException("container");

      Container = container;
    }

    public static void DeployView(ResponsiveMDIChild child)
    {
      if (child == null)
        throw new ArgumentNullException("child");
      if (Container == null)
        throw new ArgumentNullException("Container", "The container must be set before deploy child view.");

      if (!Container.Children.Contains(child))
      {
        Container.Children.Add(child);
      }
    }
  }
}
