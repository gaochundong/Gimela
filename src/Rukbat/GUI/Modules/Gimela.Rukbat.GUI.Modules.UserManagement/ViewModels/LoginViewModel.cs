using System;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;
using Gimela.Crust.Tectosphere;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Common.Cultures;
using Gimela.Rukbat.GUI.Modules.UIMessage;
using Gimela.Rukbat.GUI.Modules.UserManagement.Models;

namespace Gimela.Rukbat.GUI.Modules.UserManagement.ViewModels
{
  public class LoginViewModel : ViewModelResponsive
  {
    #region Ctors

    public LoginViewModel(LoginModel model, Action<bool> loginResultCallback)
    {
      Model = model;
      LoginResultCallback = loginResultCallback;

      ServerIPAddress = @"127.0.0.1";
      UserName = "admin";
    }

    #endregion

    #region Properties

    public LoginModel Model { get; private set; }

    public Action<bool> LoginResultCallback { get; private set; }

    private string _serverIPAddress;
    public string ServerIPAddress
    {
      get
      {
        return _serverIPAddress;
      }
      set
      {
        if (_serverIPAddress == value) return;

        _serverIPAddress = value;
        RaisePropertyChanged("ServerIPAddress");
      }
    }

    private string _userName;
    public string UserName
    {
      get
      {
        return _userName;
      }
      set
      {
        if (_userName == value) return;

        _userName = value;
        RaisePropertyChanged("UserName");
      }
    }

    private string _password;
    public string Password
    {
      get
      {
        return _password;
      }
      set
      {
        if (_password == value) return;

        _password = value;
        RaisePropertyChanged("Password");
      }
    }

    private bool _loginResult;
    public bool LoginResult
    {
      get
      {
        return _loginResult;
      }
      set
      {
        if (_loginResult == value) return;

        _loginResult = value;
        RaisePropertyChanged("LoginResult");
      }
    }

    #endregion

    #region Binding

    protected override void BindCommands()
    {
      LoginCommand = new RelayCommand(() =>
      {
        Login();
      });

      ExitCommand = new RelayCommand(() =>
      {
        Messenger.Default.Send(new NotificationMessage(UIMessageType.Common_CloseWindowEvent));
      });
    }

    protected override void UnbindCommands()
    {
      LoginCommand = null;
      ExitCommand = null;
    }

    protected override void SubscribeMessages()
    {

    }

    protected override void UnsubscribeMessages()
    {

    }

    public RelayCommand LoginCommand { get; private set; }

    public RelayCommand ExitCommand { get; private set; }

    #endregion

    #region Methods

    private void Login()
    {
      if (string.IsNullOrEmpty(ServerIPAddress))
        throw new ArgumentNullException("ServerIPAddress");
      if (string.IsNullOrEmpty(UserName))
        throw new ArgumentNullException("UserName");

      Status = ViewModelStatus.Loading;
      LoginResult = false;

      Model.Login(ServerIPAddress, UserName, Password, LoginCallback);
    }

    public void LoginCallback(object sender, AsyncWorkerCallbackEventArgs<bool> args)
    {
      bool result = CheckAsyncWorkerCallback<bool>(sender, args, true, LanguageString.Find("UserManagement_LoginView_ServerFailed"));

      Status = ViewModelStatus.Loaded;

      if (result)
      {
        LoginResult = (bool)args.Data;

        if (!LoginResult)
        {
          Messenger.Default.Send(new ViewModelMessageBoxMessage(this, LanguageString.Find("UserManagement_LoginView_InvalidUserNameOrPassword"), ViewModelMessageBoxType.Error));
        }

        if (LoginResultCallback != null)
        {
          LoginResultCallback(LoginResult);
        }
      }
    }

    #endregion
  }
}
