using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Gimela.Common.Cultures;

namespace Gimela.Rukbat.GUI.Modules.Navigation
{
  public static class NavigationMenu
  {
    private static readonly ObservableCollection<MenuItem> items = new ObservableCollection<MenuItem>();

    static NavigationMenu()
    {
      LoadNavigationMenu();
    }

    public static event EventHandler<NavigationMenuChangedEventArgs> NavigationMenuChangedEvent;

    private static void RaiseNavigationMenuChangedEvent()
    {
      if (NavigationMenuChangedEvent != null)
      {
        NavigationMenuChangedEvent(null, new NavigationMenuChangedEventArgs());
      }
    }

    public static ObservableCollection<MenuItem> GetItems()
    {
      return items;
    }

    public static void LoadNavigationMenu()
    {
      items.Clear();

      MenuItem miLive = new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Live"), Width = 60, HorizontalContentAlignment = HorizontalAlignment.Left };
      miLive.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_LiveVideo"), Command = NavigationCommands.ShowLiveVideoCameraListCommand });
      miLive.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_VideoSplice"), IsEnabled = false });
      miLive.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_VideoCarousel"), IsEnabled = false });
      miLive.Items.Add(new Separator());
      miLive.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Bookmark"), IsEnabled = false });

      MenuItem miRecord = new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Record"), Width = 60, HorizontalContentAlignment = HorizontalAlignment.Left };
      miRecord.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_RetrieveVideoPlayback"), IsEnabled = false });
      miRecord.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_PlayLocalFile"), IsEnabled = false });
      miRecord.Items.Add(new Separator());
      miRecord.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_RecordScheduler"), IsEnabled = false });

      MenuItem miIntelligence = new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Intelligence"), Width = 60, HorizontalContentAlignment = HorizontalAlignment.Left };
      miIntelligence.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_IntelligentAlarm"), IsEnabled = false });
      miIntelligence.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_IntelligentSearch"), IsEnabled = false });
      miIntelligence.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_IntelligentAnalysis"), IsEnabled = false });

      MenuItem miMap = new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Map"), Width = 60, HorizontalContentAlignment = HorizontalAlignment.Left };
      miMap.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_FloorImage"), IsEnabled = false });
      miMap.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_WebMap"), IsEnabled = false });

      MenuItem miDevice = new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Device"), Width = 60, HorizontalContentAlignment = HorizontalAlignment.Left };
      miDevice.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_DeviceManagement"), Command = NavigationCommands.ManageCameraCommand });
      miDevice.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_ServiceTopology"), IsEnabled = false });

      MenuItem miPublish = new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Publish"), Width = 60, HorizontalContentAlignment = HorizontalAlignment.Left };
      miPublish.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Tweet"), IsEnabled = false });
      miPublish.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_VideoUpload"), IsEnabled = false });
      miPublish.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_PublishMedia"), Command = NavigationCommands.PublishMediaCommand });

      MenuItem miUser = new MenuItem() { Header = LanguageString.Find("Navigation_Menu_User"), Width = 60, HorizontalContentAlignment = HorizontalAlignment.Left };
      miUser.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_UserManagement"), IsEnabled = false });
      miUser.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_RoleConfiguration"), IsEnabled = false });

      MenuItem miLayout = new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Window"), Width = 60, HorizontalContentAlignment = HorizontalAlignment.Left };
      miLayout.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Cascade"), Command = NavigationCommands.ChangeLayoutCascadeCommand });
      miLayout.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Horizontal"), Command = NavigationCommands.ChangeLayoutTileHorizontalCommand });
      miLayout.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Vertical"), Command = NavigationCommands.ChangeLayoutTileVerticalCommand });
      miLayout.Items.Add(new Separator());
      MenuItem miSplit = new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Split") };
      miSplit.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Split1"), IsEnabled = false });
      miSplit.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Split2"), IsEnabled = false });
      miSplit.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Split4"), IsEnabled = false });
      miSplit.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Split6"), IsEnabled = false });
      miSplit.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Split9"), IsEnabled = false });
      miSplit.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Split12"), IsEnabled = false });
      miSplit.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Split16"), IsEnabled = false });
      miLayout.Items.Add(miSplit);

      MenuItem miTools = new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Tools"), Width = 60, HorizontalContentAlignment = HorizontalAlignment.Left };
      miTools.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Skins"), Command = NavigationCommands.UpdateSkinCommand });
      MenuItem miLanguage = new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Languages") };
      miLanguage.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_ChineseSimplified"), Command = NavigationCommands.ChangeLanguage_zh_CN_Command });
      miLanguage.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_English"), Command = NavigationCommands.ChangeLanguage_en_US_Command });
      miTools.Items.Add(miLanguage);
      miTools.Items.Add(new Separator());
      miTools.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Options"), IsEnabled = false });

      MenuItem miHelp = new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Help"), Width = 60, HorizontalContentAlignment = HorizontalAlignment.Left };
      miHelp.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_OnlineHelp"), Command = NavigationCommands.HelpOnlineCommand });
      miHelp.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_CheckForUpdates"), Command = NavigationCommands.CheckUpdateCommand });
      miHelp.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_ReportABug"), Command = NavigationCommands.ReportBugCommand });
      miHelp.Items.Add(new Separator());
      miHelp.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_TermsOfUse"), IsEnabled = false });
      miHelp.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_PrivacyStatement"), IsEnabled = false });
      miHelp.Items.Add(new MenuItem() { Header = LanguageString.Find("Navigation_Menu_Abort"), Command = NavigationCommands.SeeAboutCommand });

      // calculate the menu item width, so that we can make the display more beautiful
      items.Add(CalculateMenuWidth(miLive));
      items.Add(CalculateMenuWidth(miRecord));
      items.Add(CalculateMenuWidth(miIntelligence));
      items.Add(CalculateMenuWidth(miMap));
      items.Add(CalculateMenuWidth(miDevice));
      items.Add(CalculateMenuWidth(miPublish));
      items.Add(CalculateMenuWidth(miUser));
      items.Add(CalculateMenuWidth(miLayout));
      items.Add(CalculateMenuWidth(miTools));
      items.Add(CalculateMenuWidth(miHelp));

      // notify that the navigation menu ites were changed
      RaiseNavigationMenuChangedEvent();
    }

    private static MenuItem CalculateMenuWidth(MenuItem menuItem)
    {
      string header = (string)menuItem.Header;
      double fontSize = menuItem.FontSize;

      var regex = new Regex("^[a-zA-Z0-9 ]+$");
      if (regex.Match(header).Success)
      {
        // latin symbols
        menuItem.Width = header.Length * fontSize / 2 + 2 * fontSize;
      }
      else
      {
        // chinese
        menuItem.Width = header.Length * fontSize + 2 * fontSize;
      }

      return menuItem;
    }
  }
}
