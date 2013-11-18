using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Gimela.Infrastructure.Messaging;
using Gimela.Common.Cultures;
using Gimela.Rukbat.GUI.Cultures;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.GUI.Modules.SkinConfiguration;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Rukbat.GUI.Modules.Widgets;

namespace Gimela.Rukbat.GUI.Modules.Navigation
{
  public static class NavigationRouter
  {
    private static readonly object _sender = new object();
    private static Dictionary<ViewType, WeakReference> _cacheSingle = new Dictionary<ViewType, WeakReference>();
    private static Dictionary<string, WeakReference> _cacheMultiple = new Dictionary<string, WeakReference>();

    public static void SubscribeMessage()
    {
      RegisterNavigation();

      Messenger.Default.Register<NotificationMessage<Camera>>(_sender, message =>
      {
        if (message.Notification == UIMessageType.LiveVideo_LiveVideoEvent)
        {
          ResponsiveMDIChild view = ViewMaker.MakeLiveVideoView(message.Content);
          view.Closing += new RoutedEventHandler((sender, e) =>
          {
            // 实时视频窗体正在关闭
            Messenger.Default.Send(new NotificationMessage<Camera>(
              UIMessageType.LiveVideo_LiveVideoClosingEvent, message.Content));
          });
          NavigationDeploy.DeployView(view);
        }
      });

      Messenger.Default.Register<NotificationMessage>(_sender, message =>
      {
        // 关闭皮肤窗体
        if (message.Notification == UIMessageType.SkinConfiguration_CloseWindowEvent)
        {
          CloseView(ViewType.SkinConfiguration);
        }
      });
    }

    private static void RegisterNavigation()
    {
      Messenger.Default.Register<NotificationMessage>(_sender, message =>
      {
        if (message.Notification == UIMessageType.Navigation_ManageCameraEvent)
        {
          // 设备管理
          MakeSingleView(ViewType.DeviceConfiguration, ViewMaker.MakeDeviceConfigurationView);
        }
        else if (message.Notification == UIMessageType.Navigation_ShowLiveVideoCameraListEvent)
        {
          // 实时视频摄像机列表
          MakeSingleView(ViewType.LiveVideoCameraList, ViewMaker.MakeLiveVideoCameraListView);
        }
        else if (message.Notification == UIMessageType.Navigation_PublishMediaEvent)
        {
          // 媒体发布
          MakeSingleView(ViewType.PublishMedia, ViewMaker.MakePublishMediaView);
        }
        else if (message.Notification == UIMessageType.Navigation_UpdateSkinEvent)
        {
          // 皮肤
          MakeSingleView(ViewType.SkinConfiguration, ViewMaker.MakeSkinConfigurationView);
        }
        else if (message.Notification == UIMessageType.Navigation_ChangeLanguage_zh_CN_Event)
        {
          LanguageLoader.LoadLanguageResource(CultureHelper.Component, @"zh-CN");
          NavigationMenu.LoadNavigationMenu();
          UpdateViewLanguageResource();
        }
        else if (message.Notification == UIMessageType.Navigation_ChangeLanguage_en_US_Event)
        {
          LanguageLoader.LoadLanguageResource(CultureHelper.Component, @"en-US");
          NavigationMenu.LoadNavigationMenu();
          UpdateViewLanguageResource();
        }
        else if (message.Notification == UIMessageType.Navigation_HelpOnlineEvent)
        {
          Process.Start(new ProcessStartInfo(@"http://weibo.com/gaochundong"));
        }
        else if (message.Notification == UIMessageType.Navigation_CheckUpdateEvent)
        {
          Process.Start(new ProcessStartInfo(@"http://weibo.com/gaochundong"));
        }
        else if (message.Notification == UIMessageType.Navigation_ReportBugEvent)
        {
          Process.Start(new ProcessStartInfo(@"http://weibo.com/gaochundong"));
        }
        else if (message.Notification == UIMessageType.Navigation_SeeAboutEvent)
        {
          MakeSingleView(ViewType.Abort, ViewMaker.MakeAboutView);
        }
      });
    }

    private static void MakeSingleView(ViewType viewType, Func<ResponsiveMDIChild> func)
    {
      if (_cacheSingle.ContainsKey(viewType) && _cacheSingle[viewType].IsAlive)
      {
        ResponsiveMDIChild view = (ResponsiveMDIChild)_cacheSingle[viewType].Target;
        NavigationDeploy.DeployView(view);
      }
      else
      {
        ResponsiveMDIChild view = func();
        CloseView(viewType);
        _cacheSingle.Add(viewType, new WeakReference(view, false));
        NavigationDeploy.DeployView(view);
      }
    }

    private static void CloseView(ViewType viewType)
    {
      if (_cacheSingle.ContainsKey(viewType) && _cacheSingle[viewType].IsAlive)
      {
        ResponsiveMDIChild view = _cacheSingle[viewType].Target as ResponsiveMDIChild;
        if (view != null)
        {
          view.Close();
        }
        _cacheSingle[viewType].Target = null;
        _cacheSingle.Remove(viewType);
      }

      List<ViewType> deleteItems = new List<ViewType>();
      foreach (var item in _cacheSingle)
      {
        if (item.Value.IsAlive == false || item.Value.Target == null)
        {
          deleteItems.Add(item.Key);
        }
      }
      foreach (var item in deleteItems)
      {
        _cacheSingle.Remove(item);
      }
    }

    private static void CloseView(string correlationId)
    {
      if (_cacheMultiple.ContainsKey(correlationId) && _cacheMultiple[correlationId].IsAlive)
      {
        ResponsiveMDIChild view = _cacheMultiple[correlationId].Target as ResponsiveMDIChild;
        if (view != null)
        {
          view.Close();
        }
        _cacheMultiple[correlationId].Target = null;
        _cacheMultiple.Remove(correlationId);
      }

      List<string> deleteItems = new List<string>();
      foreach (var item in _cacheMultiple)
      {
        if (item.Value.IsAlive == false || item.Value.Target == null)
        {
          deleteItems.Add(item.Key);
        }
      }
      foreach (var item in deleteItems)
      {
        _cacheMultiple.Remove(item);
      }
    }

    private static void UpdateViewLanguageResource()
    {
      foreach (var item in _cacheSingle)
      {
        ResponsiveMDIChild view = item.Value.Target as ResponsiveMDIChild;
        if (view != null)
        {
          view.RefreshTitle();
        }
      }
    }
  }
}
