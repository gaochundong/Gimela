using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Gimela.Common.ExceptionHandling;

namespace Gimela.Net.Sockets
{
  /// <summary>
  /// UDP接收器
  /// </summary>
  public class UdpReceiver : IDisposable
  {
    private bool disposed = false;
    private UdpClient udpClient;

    /// <summary>
    /// UDP接收器
    /// </summary>
    /// <param name="listenPort">监听的端口</param>
    public UdpReceiver(int listenPort)
    {
      this.Encoding = Encoding.Default;
      this.ListenPort = listenPort;
      udpClient = new UdpClient(listenPort);
      udpClient.AllowNatTraversal(true);
    }

    /// <summary>
    /// 监听的端口
    /// </summary>
    public int ListenPort { get; private set; }
    /// <summary>
    /// 接收器是否正在运行
    /// </summary>
    public bool IsRunning { get; private set; }
    /// <summary>
    /// 接收器所使用的编码
    /// </summary>
    public Encoding Encoding { get; set; }

    /// <summary>
    /// 启动接收器
    /// </summary>
    /// <returns>UDP接收器</returns>
    public UdpReceiver Start()
    {
      if (!IsRunning)
      {
        IsRunning = true;
        udpClient.BeginReceive(new AsyncCallback(HandleDatagramReceived), new UdpState(udpClient, new IPEndPoint(IPAddress.Any, 0)));
      }
      return this;
    }

    /// <summary>
    /// 停止接收器
    /// </summary>
    /// <returns>UDP接收器</returns>
    public UdpReceiver Stop()
    {
      IsRunning = false;
      return this;
    }

    private void HandleDatagramReceived(IAsyncResult ar)
    {
      if (IsRunning)
      {
        UdpClient client = (UdpClient)((UdpState)ar.AsyncState).UdpClient;
        IPEndPoint remoteSender = (IPEndPoint)((UdpState)(ar.AsyncState)).RemoteEndPoint;

        // received byte and trigger event notification
        byte[] receivedBytes = client.EndReceive(ar, ref remoteSender);
        RaiseDatagramReceived(remoteSender, receivedBytes);
        RaisePlaintextReceived(remoteSender, receivedBytes);

        // continue listening for udp datagram packets
        client.BeginReceive(new AsyncCallback(HandleDatagramReceived), ar.AsyncState);
      }
    }

    /// <summary>
    /// 接收到数据报文事件
    /// </summary>
    public event EventHandler<UdpDatagramReceivedEventArgs<byte[]>> DatagramReceived;
    /// <summary>
    /// 接收到数据报文明文事件
    /// </summary>
    public event EventHandler<UdpDatagramReceivedEventArgs<string>> PlaintextReceived;

    private void RaiseDatagramReceived(IPEndPoint remoteEndPoint, byte[] datagram)
    {
      if (DatagramReceived != null)
      {
        DatagramReceived(this, new UdpDatagramReceivedEventArgs<byte[]>(remoteEndPoint, datagram));
      }
    }

    private void RaisePlaintextReceived(IPEndPoint remoteEndPoint, byte[] datagram)
    {
      if (PlaintextReceived != null)
      {
        PlaintextReceived(this, new UdpDatagramReceivedEventArgs<string>(remoteEndPoint, this.Encoding.GetString(datagram, 0, datagram.Length)));
      }
    }

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        if (disposing)
        {
          try
          {
            Stop();

            if (udpClient != null)
            {
              udpClient.Close();
              udpClient = null;
            }
          }
          catch (SocketException ex)
          {
            ExceptionHandler.Handle(ex);
          }
        }

        disposed = true;
      }
    }

    #endregion
  }
}
