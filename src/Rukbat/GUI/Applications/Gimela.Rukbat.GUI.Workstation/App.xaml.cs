using Gimela.Crust;
using Gimela.Common.Logging;
using Gimela.Rukbat.Communication;

namespace Gimela.Rukbat.GUI.Workstation
{
  public partial class App : System.Windows.Application
  {
    static App()
    {
      LogFactory.Assign(new ConsoleLogFactory());
      DispatcherHelper.Initialize();
      ServiceProvider.Bootstrap();
    }
  }
}
