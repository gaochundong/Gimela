using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Gimela.Net.Http;
using Gimela.Net.Http.Modules;
using Gimela.Net.Http.Resources;
using Gimela.Net.Http.Routing;
using TestHttpServer.Modules;

namespace TestHttpServer
{
  class Program
  {
    static void Main(string[] args)
    {
      string serverName = "Dennis Gao";
      string httpServerName = serverName + " HTTP Server";
      int httpBindingPort = 8000;

      Server server = null;
      server = new Server(httpServerName);
      server.Add(HttpListenerFactory.Create(IPAddress.Any, httpBindingPort));

      FileModule fileModule = new FileModule();
      server.Add(fileModule);

      EmbeddedResourceLoader embedded = new EmbeddedResourceLoader();
      fileModule.Resources.Add(embedded);

      embedded.Add("/", Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Name + @".img.favicon.ico");
      embedded.Add("/", Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Name + @".img.camera.jpg");
      embedded.Add("/", Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Name + @".js.jquery-2-0-3.js");
      embedded.Add("/", Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Name + @".js.jquery-mobile-1-3-2.js");
      embedded.Add("/", Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Name + @".css.jquery-mobile-1-3-2.css");
      embedded.Add("/", Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Name + @".html.login.html");

      server.Add(new HomeModule());

      server.Add(new SimpleRouter("/", "/html/login.html"));
      server.Add(new SimpleRouter("/favicon.ico", "/img/favicon.ico"));      

      server.Start(5);

      Console.WriteLine(string.Format("Start {0} on {1}.", httpServerName, httpBindingPort));

      Console.WriteLine();
      Console.WriteLine("Press any key to close service.");
      Console.ReadKey();

      server.Stop(true);
    }
  }
}
