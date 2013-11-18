using System;
using System.Windows.Controls;
using Gimela.Rukbat.GUI.Modules.SkinConfiguration.ViewModels;
using Gimela.Rukbat.GUI.Modules.SkinConfiguration.Views;

namespace Gimela.Rukbat.GUI.Modules.SkinConfiguration
{
  public static class SkinConfigurationViewRouter
  {
    public static void SetEntry(Panel container)
    {
      if (container == null)
        throw new ArgumentNullException("container");

      UpdateSkinViewModel model = new UpdateSkinViewModel();
      UpdateSkinView view = new UpdateSkinView();
      view.DataContext = model;
      container.Children.Add(view);
    }
  }
}
