using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Gimela.Common.Logging;
using Gimela.Media.Video;
using Gimela.Streaming.MJpegStreaming;

namespace TestMJpegStreaming
{
  class Program
  {
    static MJpegStreamingServer server = new MJpegStreamingServer(8888);

    static void Main(string[] args)
    {
      LogFactory.Assign(new ConsoleLogFactory());

      server.Start();
      Logger.Debug("StreamingServer is running...");

      DesktopVideoSource source = new DesktopVideoSource(0);
      source.FrameInterval = 50;
      source.IsResized = true;
      source.ResizeWidth = 600;
      source.ResizeHeight = 450;
      source.NewFrame += new NewFrameEventHandler(OnNewFrame);
      source.Start();
      Logger.Debug("VideoSource is running...");

      Console.ReadLine();
    }

    static void OnNewFrame(object sender, NewFrameEventArgs e)
    {
      Logger.Debug("Writing frame on timestamp " + e.Timestamp.ToLocalTime().ToString(@"yyyy-MM-dd HH:mm:ss.ffffff"));
      server.Write(e.Frame);
    }
  }
}
