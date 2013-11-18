using System;
using System.Windows.Controls;
using Gimela.Rukbat.GUI.Modules.LiveVideo.Models;
using Gimela.Rukbat.GUI.Modules.LiveVideo.ViewModels;
using Gimela.Rukbat.GUI.Modules.LiveVideo.Views;

namespace Gimela.Rukbat.GUI.Modules.LiveVideo
{
  public static class CameraListViewRouter
  {
    public static void SetEntry(Panel container)
    {
      if (container == null)
        throw new ArgumentNullException("container");

      CameraModel model = new CameraModel();
      CameraListViewModel viewModel = new CameraListViewModel(model);
      CameraListView view = new CameraListView();
      view.DataContext = viewModel;
      container.Children.Add(view);
    }
  }
}
