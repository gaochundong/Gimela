using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Gimela.Crust;
using Gimela.Crust.Tectosphere;
using Gimela.Common.Configuration;
using Gimela.Common.Cultures;
using Gimela.Common.ExceptionHandling;
using Gimela.Infrastructure.Messaging;
using Gimela.Presentation.Controls;
using Gimela.Presentation.Skins;
using Gimela.Presentation.Windows;
using Gimela.Rukbat.GUI.Controls;
using Gimela.Rukbat.GUI.Cultures;
using Gimela.Rukbat.GUI.Modules.Navigation;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Rukbat.GUI.Modules.UserManagement.Models;
using Gimela.Rukbat.GUI.Modules.UserManagement.ViewModels;

namespace Gimela.Rukbat.GUI.Workstation
{
  public partial class MainWindow : HeaderWindow
  {
    #region Ctors

    public MainWindow()
    {
      InitializeComponent();

      Messenger.Default.Register<NotificationMessage>(this, HandleNotificationMessage);
      Messenger.Default.Register<ViewModelMessageBoxMessage>(this, MessageBoxWindowHelper.HandleViewModelMessageBoxMessage);
      Messenger.Default.Register<NotificationMessage<MDILayoutType>>(this, message =>
      {
        this.container.MDILayout = message.Content;
      });
    }

    #endregion

    #region Loaded

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      try
      {
        if (ConfigurationMaster.ContainsKey(SkinHelper.SkinColorConfigurationKey))
        {
          SkinColorType skin = SkinHelper.StringToSkinColorType(ConfigurationMaster.Get(SkinHelper.SkinColorConfigurationKey));
          SkinHelper.LoadSkin(skin);
        }

        LanguageLoader.LoadLanguageResource(CultureHelper.Component, CultureHelper.Culture);
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }

      LoginModel model = new LoginModel();
      LoginViewModel viewModel = new LoginViewModel(model, LoginResultCallback);
      loginView.DataContext = viewModel;
    }

    private void LoginResultCallback(bool loginResult)
    {
      if (loginResult)
      {
        SetNavigationMenu();
        SetNavigationContainer();
      }
    }

    #endregion

    #region Navigation

    private void SetNavigationMenu()
    {
      ResetNavigationMenus();
      NavigationMenu.NavigationMenuChangedEvent += OnNavigationMenuChangedEvent;

      NavigationRouter.SubscribeMessage();      
    }

    private void SetNavigationContainer()
    {
      NavigationDeploy.SetContainer(this.container);
    }

    private void OnNavigationMenuChangedEvent(object sender, NavigationMenuChangedEventArgs e)
    {
      ResetNavigationMenus();
    }

    private void ResetNavigationMenus()
    {
      ObservableCollection<MenuItem> ic = NavigationMenu.GetItems();
      this.mainWindow.Menu.Items.Clear();
      foreach (var item in ic)
      {
        this.mainWindow.Menu.Items.Add(item);
      }
    }

    #endregion

    #region NotificationMessage

    private void HandleNotificationMessage(NotificationMessage message)
    {
      if (message == null) return;

      if (message.Notification == UIMessageType.Common_CloseWindowEvent)
      {
        this.Close();
      }
    }

    #endregion
  }
}
