using System;
using System.Windows.Controls;
using Gimela.Rukbat.GUI.Modules.LiveVideo.Models;
using Gimela.Rukbat.GUI.Modules.LiveVideo.ViewModels;
using Gimela.Rukbat.GUI.Modules.LiveVideo.Views;

namespace Gimela.Rukbat.GUI.Modules.LiveVideo
{
  public static class LiveVideoViewRouter
  {
    public static void SetEntry(Panel container, object target)
    {
      if (container == null)
        throw new ArgumentNullException("container");

      CameraModel model = new CameraModel();
      LiveVideoViewModel viewModel = new LiveVideoViewModel(model);
      viewModel.SetObject(target);
      LiveVideoView view = new LiveVideoView();
      view.DataContext = viewModel;
      container.Children.Add(view);
    }
  }
}
