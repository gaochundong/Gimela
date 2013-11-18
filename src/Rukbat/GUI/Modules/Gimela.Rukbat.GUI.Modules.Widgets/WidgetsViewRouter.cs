using System;
using System.Windows.Controls;
using Gimela.Rukbat.GUI.Modules.Widgets.ViewModels;
using Gimela.Rukbat.GUI.Modules.Widgets.Views;

namespace Gimela.Rukbat.GUI.Modules.Widgets
{
  public static class WidgetsViewRouter
  {
    public static void SetEntry(Panel container)
    {
      if (container == null)
        throw new ArgumentNullException("container");

      AboutViewModel viewModel = new AboutViewModel();
      AboutView view = new AboutView();
      view.DataContext = viewModel;
      container.Children.Add(view);
    }
  }
}
