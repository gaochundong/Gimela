using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Gimela.Crust;
using Gimela.Crust.Tectosphere;

namespace Gimela.Rukbat.GUI.Modules.Widgets.ViewModels
{
  public sealed class AboutViewModel : ViewModelResponsive
  {
    #region Ctors

    public AboutViewModel()
    {
      GetApplicationVersion();
    }

    #endregion

    #region Properties

    private string _version;
    public string Version
    {
      get
      {
        return _version;
      }
      set
      {
        if (_version == value) return;

        _version = value;
        RaisePropertyChanged("Version");
      }
    }

    #endregion

    #region Bindings

    protected override void BindCommands()
    {
      WebsiteHyperlinkCommand = new RelayCommand<Uri>((uri) =>
      {
        Process.Start(new ProcessStartInfo(uri.AbsoluteUri));
      });
    }

    protected override void UnbindCommands()
    {
      WebsiteHyperlinkCommand = null;
    }

    protected override void SubscribeMessages()
    {

    }

    protected override void UnsubscribeMessages()
    {

    }

    public RelayCommand<Uri> WebsiteHyperlinkCommand { get; private set; }

    #endregion

    #region Private Methods

    private void GetApplicationVersion()
    {
      Assembly asse = Assembly.GetExecutingAssembly();

      // "AssemblyName, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
      Match match = new Regex(
          @",\s+Version=(\S*),\s+Culture",
          RegexOptions.None).Match(asse.FullName.ToString());
      if (match.Success)
      {
        Version = "v" + match.Groups[1].ToString();
      }
    }

    #endregion
  }
}
