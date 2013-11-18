using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Gimela.Net.Sockets;
using Gimela.Common.Logging;

namespace TestTcpClient
{
  class Program
  {
    static AsyncTcpClient client;

    static void Main(string[] args)
    {
      LogFactory.Assign(new ConsoleLogFactory());

      IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
      IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9998); // 测试用，可以不指定由系统选择端口
      client = new AsyncTcpClient(remoteEP, localEP);
      client.Encoding = Encoding.UTF8;
      client.ServerExceptionOccurred += new EventHandler<TcpServerExceptionOccurredEventArgs>(client_ServerExceptionOccurred);
      client.ServerConnected += new EventHandler<TcpServerConnectedEventArgs>(client_ServerConnected);
      client.ServerDisconnected += new EventHandler<TcpServerDisconnectedEventArgs>(client_ServerDisconnected);
      client.PlaintextReceived += new EventHandler<TcpDatagramReceivedEventArgs<string>>(client_PlaintextReceived);
      client.Connect();

      Console.WriteLine("TCP client has connected to server.");
      Console.WriteLine("Type something to send to server...");
      while (true)
      {
        try
        {
          string text = Console.ReadLine();
          client.Send(text);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
    }

    static void client_ServerExceptionOccurred(object sender, TcpServerExceptionOccurredEventArgs e)
    {
      Logger.Debug(string.Format(CultureInfo.InvariantCulture, "TCP server {0} exception occurred, {1}.", e.ToString(), e.Exception.Message));
    }

    static void client_ServerConnected(object sender, TcpServerConnectedEventArgs e)
    {
      Logger.Debug(string.Format(CultureInfo.InvariantCulture, "TCP server {0} has connected.", e.ToString()));
    }

    static void client_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
    {
      Logger.Debug(string.Format(CultureInfo.InvariantCulture, "TCP server {0} has disconnected.", e.ToString()));
    }

    static void client_PlaintextReceived(object sender, TcpDatagramReceivedEventArgs<string> e)
    {
      Console.Write(string.Format("Server : {0} --> ", e.TcpClient.Client.RemoteEndPoint.ToString()));
      Console.WriteLine(string.Format("{0}", e.Datagram));
    }
  }
}
