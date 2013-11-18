using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Gimela.Common.Configuration;
using Gimela.Common.Logging;
using Gimela.Net.Http;
using Gimela.Net.Http.Modules;
using Gimela.Net.Http.Resources;
using Gimela.Net.Http.Routing;
using Gimela.Rukbat.Communication;
using Gimela.Rukbat.DPS.BusinessLogic.HttpModules;
using Gimela.Rukbat.DPS.Contracts.ServiceContracts;
using Gimela.Rukbat.DPS.ServiceImplementation;
using Gimela.ServiceModel.ManagedDiscovery;
using Gimela.ServiceModel.ManagedHosting;
using Gimela.ServiceModel.ManagedService;

namespace Gimela.Rukbat.DPS.ServiceConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      LogFactory.Assign(new ConsoleLogFactory());
      ServiceProvider.Bootstrap();

      ServiceHostInfo info = new ServiceHostInfo();
      info.Name = ConfigurationMaster.Get(ServiceConfiguration.DefaultServiceConfigurationName);
      info.Address = Dns.GetHostName();
      info.Port = int.Parse(ConfigurationMaster.Get(ServiceConfiguration.DefaultServicePortConfigurationName));
      info.Binding = new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName);
      info.Contract = typeof(IDeviceProfileService);
      info.Service = typeof(DeviceProfileService);

      Console.WriteLine(string.Format(@"Service is starting on [{0}]", info.ToString()));

      ManagedServiceHostActivator<IDeviceProfileService> activator = new ManagedServiceHostActivator<IDeviceProfileService>(info);
      activator.Start();

      Console.WriteLine(string.Format(@"Service address [{0}]", activator.ServiceHost.Description.Endpoints.First().Address));

      string serverName = ConfigurationMaster.Get(@"HttpServerName");
      string httpServerName = serverName + " HTTP Server";
      int httpBindingPort = int.Parse(ConfigurationMaster.Get(@"HttpServerPort"));

      Server server = null;
      server = new Server(httpServerName);
      server.Add(HttpListenerFactory.Create(IPAddress.Any, httpBindingPort));

      server.Add(new CameraListModule());
      server.Add(new CameraModule());
      server.Add(new CameraThumbnailModule());

      FileModule fileModule = new FileModule();
      EmbeddedResourceLoader embedded = new EmbeddedResourceLoader();
      embedded.Add("/", Assembly.GetExecutingAssembly(),
          Assembly.GetExecutingAssembly().GetName().Name,
          Assembly.GetExecutingAssembly().GetName().Name + @".Resources.favicon.ico");
      fileModule.Resources.Add(embedded);
      server.Add(fileModule);
      server.Add(new SimpleRouter("/favicon.ico", "/resources/favicon.ico"));

      server.Start(5);

      Console.WriteLine(string.Format("Start {0} on {1}.", httpServerName, httpBindingPort));

      Console.WriteLine();
      Console.WriteLine("Press any key to close service.");
      Console.ReadKey();

      server.Stop(true);
      activator.Stop();
    }
  }
}
