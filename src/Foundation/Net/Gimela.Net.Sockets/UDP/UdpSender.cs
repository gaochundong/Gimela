using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Gimela.Net.Sockets
{
  /// <summary>
  /// UDP发送器
  /// </summary>
  public class UdpSender : IDisposable
  {
    private bool disposed = false;
    private Thread senderThread;
    private UdpClient udpClient;
    private ConcurrentQueue<byte[]> queue;
    private readonly ManualResetEvent waiter;

    /// <summary>
    /// UDP发送器
    /// </summary>
    /// <param name="sentToAddress">发送目的地址</param>
    /// <param name="sentToPort">发送目的端口</param>
    public UdpSender(string sentToAddress, int sentToPort)
    {
      Address = sentToAddress;
      Port = sentToPort;

      this.Encoding = Encoding.Default;

      queue = new ConcurrentQueue<byte[]>();
      waiter = new ManualResetEvent(false);

      udpClient = new UdpClient();
      udpClient.AllowNatTraversal(true);

      senderThread = new Thread(new ThreadStart(WorkThread));
    }

    /// <summary>
    /// 发送目的地址
    /// </summary>
    public string Address { get; private set; }
    /// <summary>
    /// 发送目的端口
    /// </summary>
    public int Port { get; private set; }
    /// <summary>
    /// 发送器是否正在运行
    /// </summary>
    public bool IsRunning { get; private set; }
    /// <summary>
    /// 发送器所使用的编码
    /// </summary>
    public Encoding Encoding { get; set; }

    /// <summary>
    /// 启动发送器
    /// </summary>
    /// <returns>UDP发送器</returns>
    public UdpSender Start()
    {
      if (!IsRunning)
      {
        IsRunning = true;
        senderThread.Start();
      }
      return this;
    }

    /// <summary>
    /// 停止发送器
    /// </summary>
    /// <returns>UDP发送器</returns>
    public UdpSender Stop()
    {
      IsRunning = false;
      return this;
    }

    /// <summary>
    /// 发送数据报文
    /// </summary>
    /// <param name="datagram">数据报文</param>
    /// <returns>UDP发送器</returns>
    public UdpSender Send(byte[] datagram)
    {
      if (!IsRunning)
        throw new InvalidProgramException("This sender has not been started.");

      queue.Enqueue(datagram);
      waiter.Set();

      return this;
    }

    /// <summary>
    /// 发送数据报文
    /// </summary>
    /// <param name="datagram">数据报文</param>
    /// <returns>UDP发送器</returns>
    public UdpSender Send(string datagram)
    {
      if (!IsRunning)
        throw new InvalidProgramException("This sender has not been started.");

      queue.Enqueue(this.Encoding.GetBytes(datagram));
      waiter.Set();

      return this;
    }

    private void WorkThread()
    {
      while (IsRunning)
      {
        waiter.WaitOne();
        waiter.Reset();

        while (queue.Count > 0)
        {
          byte[] datagram = null;
          if (queue.TryDequeue(out datagram))
          {
            udpClient.BeginSend(datagram, datagram.Length, Address, Port, SendCompleted, udpClient);
          }
        }
      }
    }

    private void SendCompleted(IAsyncResult ar)
    {
      UdpClient udp = ar.AsyncState as UdpClient;
      if (udp != null)
      {
        udp.EndSend(ar);
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
          Stop();
          waiter.Close();

          if (senderThread != null)
          {
            try
            {
              senderThread.Abort();
            }
            catch (ThreadStateException) { }
            finally
            {
              senderThread = null;
            }
          }

          if (udpClient != null)
          {
            udpClient.Close();
            udpClient = null;
          }
        }

        disposed = true;
      }
    }

    #endregion
  }
}
