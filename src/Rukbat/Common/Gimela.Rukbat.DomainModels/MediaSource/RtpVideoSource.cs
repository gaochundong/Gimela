using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using Gimela.Common.Consts;
using Gimela.Media.Video;
using Gimela.Net.Rtp;
using Gimela.Net.Sockets;

namespace Gimela.Rukbat.DomainModels.MediaSource
{
  public class RtpVideoSource : IVideoSource
  {
    #region Fields

    private UdpReceiver receiver;
    protected int framesReceived;
    protected int bytesReceived;

    private ConcurrentDictionary<long, List<UdpPacket>> packetCache;

    #endregion

    #region Ctor
    
    public RtpVideoSource(int listenPort)
    {
      packetCache = new ConcurrentDictionary<long, List<UdpPacket>>();

      receiver = new UdpReceiver(listenPort);
      receiver.DatagramReceived += new EventHandler<UdpDatagramReceivedEventArgs<byte[]>>(OnDatagramReceived);
    }

    #endregion

    #region IVideoSource Members

    public int BytesReceived
    {
      get { return bytesReceived; }
    }

    public int FramesReceived
    {
      get { return framesReceived; }
    }

    public string Source
    {
      get { return this.receiver.ListenPort.ToString(CultureInfo.InvariantCulture); }
    }

    public bool IsRunning
    {
      get { return receiver.IsRunning; }
    }

    public void Start()
    {
      if (!IsRunning)
      {
        framesReceived = 0;
        bytesReceived = 0;

        receiver.Start();
      }
    }

    public void Stop()
    {
      if (this.IsRunning)
      {
        receiver.Stop();

        // notify client that the video is finished
        RaiseVideoSourceFinishedEvent(VideoSourceFinishedReasonType.StoppedByUser);
      }
    }

    public void SignalToStop()
    {
      Stop();
    }

    public void WaitForStop()
    {
      Stop();
    }

    public event NewFrameEventHandler NewFrame;

    public event VideoSourceExceptionEventHandler VideoSourceException;

    public event VideoSourceFinishedEventHandler VideoSourceFinished;

    #endregion

    #region Raise Events

    private void RaiseNewFrameEvent(Bitmap frame, DateTime timestamp)
    {
      if (NewFrame != null)
      {
        NewFrame(this, new NewFrameEventArgs(frame, timestamp));
      }
    }

    private void RaiseVideoSourceExceptionEvent(string description)
    {
      if (VideoSourceException != null)
      {
        VideoSourceException(this, new VideoSourceExceptionEventArgs(description));
      }
    }

    private void RaiseVideoSourceFinishedEvent(VideoSourceFinishedReasonType reason)
    {
      if (VideoSourceFinished != null)
      {
        VideoSourceFinished(this, new VideoSourceFinishedEventArgs(reason));
      }
    }

    #endregion

    #region Private Methods

    private void OnDatagramReceived(object sender, UdpDatagramReceivedEventArgs<byte[]> e)
    {
      try
      {
        UdpPacket udpPacket = UdpPacket.FromArray(e.Datagram);

        if (udpPacket.Total == 1)
        {
          RtpPacket packet = new RtpPacket(udpPacket.Payload, udpPacket.PayloadSize);
          Bitmap bitmap = packet.ToBitmap();
          RaiseNewFrameEvent(bitmap, Epoch.GetDateTimeByYesterdayTotalMilliseconds(packet.Timestamp));
        }
        else
        {
          // rearrage packets to one packet
          if (packetCache.ContainsKey(udpPacket.Sequence))
          {
            List<UdpPacket> udpPackets = null;
            if (packetCache.TryGetValue(udpPacket.Sequence, out udpPackets))
            {
              udpPackets.Add(udpPacket);

              if (udpPackets.Count == udpPacket.Total)
              {
                packetCache.TryRemove(udpPacket.Sequence, out udpPackets);

                udpPackets = udpPackets.OrderBy(u => u.Order).ToList();
                int rtpPacketLength = udpPackets.Sum(u => u.PayloadSize);
                int maxPacketLength = udpPackets.Select(u => u.PayloadSize).Max();

                byte[] rtpPacket = new byte[rtpPacketLength];
                foreach (var item in udpPackets)
                {
                  Buffer.BlockCopy(item.Payload, 0, rtpPacket, (item.Order - 1) * maxPacketLength, item.PayloadSize);
                }

                RtpPacket packet = new RtpPacket(rtpPacket, rtpPacket.Length);
                Bitmap bitmap = packet.ToBitmap();
                RaiseNewFrameEvent(bitmap, Epoch.GetDateTimeByYesterdayTotalMilliseconds(packet.Timestamp));

                packetCache.Clear();
              }
            }
          }
          else
          {
            List<UdpPacket> udpPackets = new List<UdpPacket>();
            udpPackets.Add(udpPacket);
            packetCache.AddOrUpdate(udpPacket.Sequence, udpPackets, (k, v) => { return udpPackets; });
          }
        }
      }
      catch (Exception ex)
      {
        RaiseVideoSourceExceptionEvent(ex.Message);
      }
    }

    #endregion
  }
}
