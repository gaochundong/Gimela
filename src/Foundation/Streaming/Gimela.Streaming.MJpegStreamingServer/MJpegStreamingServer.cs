using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Gimela.Common.Logging;
using Gimela.Net.Sockets;

namespace Gimela.Streaming.MJpegStreaming
{
  public class MJpegStreamingServer
  {
    private static string _contentLengthString = "__PayloadHeaderContentLength__";
    private AsyncTcpServer _server;
    private ConcurrentDictionary<string, TcpClient> _clients;

    public MJpegStreamingServer(int listenPort)
      : this(listenPort, "--dennisgao")
    {
    }

    public MJpegStreamingServer(int listenPort, string boundary)
    {
      Port = listenPort;
      Boundary = boundary;

      _server = new AsyncTcpServer(Port);
      _server.Encoding = Encoding.ASCII;
      _clients = new ConcurrentDictionary<string, TcpClient>();
    }

    /// <summary>
    /// 监听的端口
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// 分隔符
    /// </summary>
    public string Boundary { get; private set; }

    /// <summary>
    /// 流头部
    /// </summary>
    public string StreamHeader
    {
      get
      {
        return "HTTP/1.1 200 OK" +
               "\r\n" +
               "Content-Type: multipart/x-mixed-replace; boundary=" + this.Boundary +
               "\r\n";
      }
    }

    /// <summary>
    /// 图片头部
    /// </summary>
    public string PayloadHeader
    {
      get
      {
        return "\r\n" +
               this.Boundary +
               "\r\n" +
               "Content-Type: image/jpeg" +
               "\r\n" +
               "Content-Length: " + _contentLengthString +
               "\r\n\r\n";
      }
    }

    public void Start()
    {
      _server.Start(10);
      _server.ClientConnected += new EventHandler<TcpClientConnectedEventArgs>(OnClientConnected);
      _server.ClientDisconnected += new EventHandler<TcpClientDisconnectedEventArgs>(OnClientDisconnected);
    }

    public void Stop()
    {
      _server.Stop();
      _server.ClientConnected -= new EventHandler<TcpClientConnectedEventArgs>(OnClientConnected);
      _server.ClientDisconnected -= new EventHandler<TcpClientDisconnectedEventArgs>(OnClientDisconnected);
    }

    private void OnClientConnected(object sender, TcpClientConnectedEventArgs e)
    {
      _clients.AddOrUpdate(e.TcpClient.Client.RemoteEndPoint.ToString(), e.TcpClient, (n, o) => { return e.TcpClient; });
    }

    private void OnClientDisconnected(object sender, TcpClientDisconnectedEventArgs e)
    {
      TcpClient clientToBeThrowAway;
      _clients.TryRemove(e.TcpClient.Client.RemoteEndPoint.ToString(), out clientToBeThrowAway);
    }

    public void Write(Image image)
    {
      if (_server.IsRunning)
      {
        byte[] payload = BytesOf(image);

        WriteStreamHeader();
        WritePayload(payload);
      }
    }

    private void WriteStreamHeader()
    {
      if (_clients.Count > 0)
      {
        foreach (var item in _clients)
        {
          Logger.Debug(string.Format(CultureInfo.InvariantCulture,
            "Writing stream header, {0}, {1}{2}", item.Key, Environment.NewLine, StreamHeader));

          _server.SyncSend(item.Value, StreamHeader);

          TcpClient clientToBeThrowAway;
          _clients.TryRemove(item.Key, out clientToBeThrowAway);
        }
      }
    }

    private void WritePayload(byte[] payload)
    {
      string payloadHeader = this.PayloadHeader.Replace(_contentLengthString, payload.Length.ToString());
      string payloadTail = "\r\n";

      Logger.Debug(string.Format(CultureInfo.InvariantCulture,
        "Writing payload header, {0}{1}", Environment.NewLine, payloadHeader));

      byte[] payloadHeaderBytes = _server.Encoding.GetBytes(payloadHeader);
      byte[] payloadTailBytes = _server.Encoding.GetBytes(payloadTail);
      byte[] packet = new byte[payloadHeaderBytes.Length + payload.Length + payloadTail.Length];
      Buffer.BlockCopy(payloadHeaderBytes, 0, packet, 0, payloadHeaderBytes.Length);
      Buffer.BlockCopy(payload, 0, packet, payloadHeaderBytes.Length, payload.Length);
      Buffer.BlockCopy(payloadTailBytes, 0, packet, payloadHeaderBytes.Length + payload.Length, payloadTailBytes.Length);

      _server.SendToAll(packet);
    }

    private byte[] BytesOf(Image image)
    {
      MemoryStream ms = new MemoryStream();
      image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

      byte[] payload = ms.ToArray();

      return payload;
    }
  }
}
