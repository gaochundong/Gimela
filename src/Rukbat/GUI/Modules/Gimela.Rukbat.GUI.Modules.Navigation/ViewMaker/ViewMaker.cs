using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Gimela.Common.Cultures;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration;
using Gimela.Rukbat.GUI.Modules.LiveVideo;
using Gimela.Rukbat.GUI.Modules.PublishMedia;
using Gimela.Rukbat.GUI.Modules.SkinConfiguration;
using Gimela.Rukbat.GUI.Modules.Widgets;

namespace Gimela.Rukbat.GUI.Modules.Navigation
{
  internal static class ViewMaker
  {
    internal static ResponsiveMDIChild MakeDeviceConfigurationView()
    {
      ResponsiveMDIChild mdi = new ResponsiveMDIChild()
      {
        TitleSource = "Widgets_MDI_CameraManagement",
        Content = new Grid()
      };
      mdi.RefreshTitle();

      DeviceConfigurationViewRouter.SetEntry(mdi.Content as Grid);

      return mdi;
    }

    internal static ResponsiveMDIChild MakeLiveVideoCameraListView()
    {
      ResponsiveMDIChild mdi = new ResponsiveMDIChild()
      {
        TitleSource = "Widgets_MDI_LiveCameraList",
        Content = new Grid()
      };
      mdi.RefreshTitle();

      CameraListViewRouter.SetEntry(mdi.Content as Grid);

      return mdi;
    }

    internal static ResponsiveMDIChild MakeLiveVideoView(Camera camera)
    {
      if (camera == null)
        throw new ArgumentNullException("camera");

      ResponsiveMDIChild mdi = new ResponsiveMDIChild()
      {
        Title = camera.Name,
        Content = new Grid()
      };

      LiveVideoViewRouter.SetEntry(mdi.Content as Grid, camera);

      return mdi;
    }

    internal static ResponsiveMDIChild MakePublishMediaView()
    {
      ResponsiveMDIChild mdi = new ResponsiveMDIChild()
      {
        TitleSource = "Widgets_MDI_PublishMedia",
        Content = new Grid()
      };
      mdi.RefreshTitle();

      PublishMediaViewRouter.SetEntry(mdi.Content as Grid);

      return mdi;
    }

    internal static ResponsiveMDIChild MakeSkinConfigurationView()
    {
      ResponsiveMDIChild mdi = new ResponsiveMDIChild()
      {
        TitleSource = "Widgets_MDI_ChangeSkin",
        Content = new Grid()
      };
      mdi.RefreshTitle();

      SkinConfigurationViewRouter.SetEntry(mdi.Content as Grid);

      return mdi;
    }

    internal static ResponsiveMDIChild MakeAboutView()
    {
      ResponsiveMDIChild mdi = new ResponsiveMDIChild()
      {
        TitleSource = "Widgets_MDI_Abort",
        Content = new Grid()
      };
      mdi.RefreshTitle();

      WidgetsViewRouter.SetEntry(mdi.Content as Grid);

      return mdi;
    }
  }
}
