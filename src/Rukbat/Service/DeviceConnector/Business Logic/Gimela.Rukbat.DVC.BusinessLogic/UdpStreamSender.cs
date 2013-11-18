using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Gimela.Common.Consts;
using Gimela.Net.Rtp;
using Gimela.Net.Sockets;
using Gimela.Rukbat.DVC.BusinessEntities;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public class UdpStreamSender : IStreamSender
  {
    private bool _disposed = false;
    private Thread _senderThread;
    private UdpClient _udpClient;
    private ConcurrentQueue<StreamPacket> _queue;
    private readonly ManualResetEvent _waiter;

    public UdpStreamSender()
    {
      _queue = new ConcurrentQueue<StreamPacket>();
      _waiter = new ManualResetEvent(false);

      _udpClient = new UdpClient();
      _udpClient.AllowNatTraversal(true);

      _senderThread = new Thread(new ThreadStart(WorkThread));
    }

    #region IStreamSender Members

    public bool IsRunning { get; private set; }

    public IStreamSender Start()
    {
      if (!IsRunning)
      {
        IsRunning = true;
        _senderThread.Start();
      }
      return this;
    }

    public IStreamSender Stop()
    {
      IsRunning = false;
      return this;
    }

    public IStreamSender Send(StreamPacket packet)
    {
      if (!IsRunning)
        throw new InvalidProgramException("This sender has not been started.");

      _queue.Enqueue(packet);
      _waiter.Set();

      return this;
    }

    #endregion

    private void WorkThread()
    {
      while (IsRunning)
      {
        _waiter.WaitOne();
        _waiter.Reset();

        while (_queue.Count > 0)
        {
          StreamPacket packet = null;
          if (_queue.TryDequeue(out packet))
          {
            RtpPacket rtpPacket = RtpPacket.FromImage(
              RtpPayloadType.JPEG, 
              packet.SequenceNumber, 
              (long)Epoch.GetDateTimeTotalMillisecondsByYesterday(packet.Timestamp),
              packet.Frame);

            byte[] datagram = rtpPacket.ToArray(); // max UDP packet length limited to 65,535 bytes
            packet.Frame.Dispose();

            // split udp packet to many packets for reduce the size to 65507 limit by underlying IPv4 protocol
            ICollection<UdpPacket> udpPackets = UdpPacketSplitter.Split(packet.SequenceNumber, datagram, 65507 - UdpPacket.HeaderSize);
            foreach (var udpPacket in udpPackets)
            {
              byte[] udpPacketDatagram = udpPacket.ToArray();
              // async sending
              _udpClient.BeginSend(
                udpPacketDatagram, udpPacketDatagram.Length,
                packet.Destination.Address,
                packet.Destination.Port,
                SendCompleted, _udpClient);
            }
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

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!this._disposed)
      {
        if (disposing)
        {
          Stop();
          _waiter.Close();

          if (_senderThread != null)
          {
            try
            {
              _senderThread.Abort();
            }
            catch (ThreadStateException) { }
            finally
            {
              _senderThread = null;
            }
          }

          if (_udpClient != null)
          {
            _udpClient.Close();
            _udpClient = null;
          }
        }

        _disposed = true;
      }
    }

    #endregion
  }
}
