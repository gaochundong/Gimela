using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Gimela.Common.Configuration;
using Gimela.Common.Logging;
using Gimela.ServiceModel.ManagedDiscovery;
using Gimela.ServiceModel.ManagedHosting;
using Gimela.ServiceModel.ManagedService;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Rukbat.ServiceDiscovery.Contracts.ServiceContracts;
using Gimela.Rukbat.SVD.ServiceImplementation;

namespace Gimela.Rukbat.SVD.ServiceConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      LogFactory.Assign(new ConsoleLogFactory());

      ServiceHostInfo info = new ServiceHostInfo();
      info.Name = ConfigurationMaster.Get(ServiceConfiguration.DefaultServiceConfigurationName);
      info.Address = Dns.GetHostName();
      info.Port = int.Parse(ConfigurationMaster.Get(ServiceConfiguration.DefaultServicePortConfigurationName));
      info.Binding = new NetTcpBinding(ServiceConfiguration.DefaultNetTcpBindingName);
      info.Contract = typeof(IServiceDiscoveryService);
      info.Service = typeof(ServiceDiscoveryService);

      Console.WriteLine(string.Format(@"Service is starting on [{0}]", info.ToString()));

      ManagedServiceHostActivator<IServiceDiscoveryService> activator = new ManagedServiceHostActivator<IServiceDiscoveryService>(info);
      activator.Start();

      Console.WriteLine(string.Format(@"Service address [{0}]", activator.ServiceHost.Description.Endpoints.First().Address));

      Console.WriteLine();
      Console.WriteLine("Press any key to close service.");
      Console.ReadKey();

      activator.Stop();
    }
  }
}
