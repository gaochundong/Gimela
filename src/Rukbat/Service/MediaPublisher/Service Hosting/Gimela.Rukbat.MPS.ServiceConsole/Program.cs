using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Gimela.Common.Configuration;
using Gimela.Common.Logging;
using Gimela.Rukbat.Communication;
using Gimela.Rukbat.MPS.Contracts.ServiceContracts;
using Gimela.Rukbat.MPS.ServiceImplementation;
using Gimela.ServiceModel.ManagedDiscovery;
using Gimela.ServiceModel.ManagedHosting;
using Gimela.ServiceModel.ManagedService;

namespace Gimela.Rukbat.MPS.ServiceConsole
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
      info.Contract = typeof(IMediaPublisherService);
      info.Service = typeof(MediaPublisherService);

      Console.WriteLine(string.Format(@"Service is starting on [{0}]", info.ToString()));

      ManagedServiceHostActivator<IMediaPublisherService> activator = new ManagedServiceHostActivator<IMediaPublisherService>(info);
      activator.Start();

      Console.WriteLine(string.Format(@"Service address [{0}]", activator.ServiceHost.Description.Endpoints.First().Address));

      Console.WriteLine();
      Console.WriteLine("Press any key to close service.");
      Console.ReadKey();

      activator.Stop();
    }
  }
}
