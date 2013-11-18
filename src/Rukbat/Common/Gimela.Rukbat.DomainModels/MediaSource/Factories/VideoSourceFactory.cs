using System;
using System.Linq;
using System.Net.NetworkInformation;
using Gimela.Media.Video;
using Gimela.Media.Video.DirectShow;
using Gimela.Rukbat.DomainModels.MediaSource;

namespace Gimela.Rukbat.DomainModels
{
  public static class VideoSourceFactory
  {
    public static IVideoSource BuildVideoSource(VideoSourceDescription description)
    {
      if (description == null)
        throw new ArgumentNullException("description");

      switch (description.SourceType)
      {
        case VideoSourceType.Mock:
          return MakeMockVideoSource(description);
        case VideoSourceType.LocalCamera:
          return MakeLocalCameraVideoSource(description);
        case VideoSourceType.LocalAVIFile:
          return MakeLocalAVIFileVideoSource(description);
        case VideoSourceType.LocalDesktop:
          return MakeLocalDesktopVideoSource(description);
        case VideoSourceType.NetworkJPEG:
          return MakeNetworkJPEGVideoSource(description);
        case VideoSourceType.NetworkMJPEG:
          return MakeNetworkMJPEGVideoSource(description);
        case VideoSourceType.NetworkRtpStream:
          return MakeNetworkRtpStreamVideoSource(description);
        default:
          throw new NotSupportedException();
      }
    }

    private static IVideoSource MakeMockVideoSource(VideoSourceDescription description)
    {
      return new PictureVideoSource(description.SourceString);
    }

    private static IVideoSource MakeLocalCameraVideoSource(VideoSourceDescription description)
    {
      VideoCaptureDeviceVideoSource videoSource = new VideoCaptureDeviceVideoSource(description.SourceString);
      videoSource.DesiredFrameSize = new System.Drawing.Size(description.Resolution.Width, description.Resolution.Height);
      videoSource.DesiredFrameRate = description.FrameRate;
      return videoSource;
    }

    private static IVideoSource MakeLocalAVIFileVideoSource(VideoSourceDescription description)
    {
      FileVideoSource videoSource = new FileVideoSource(description.SourceString);
      return videoSource;
    }

    private static IVideoSource MakeLocalDesktopVideoSource(VideoSourceDescription description)
    {
      DesktopVideoSource videoSource = new DesktopVideoSource(Convert.ToInt32(description.SourceString));
      videoSource.FrameInterval = description.FrameInterval;
      videoSource.ResizeWidth = description.Resolution.Width;
      videoSource.ResizeHeight = description.Resolution.Height;
      return videoSource;
    }

    private static IVideoSource MakeNetworkJPEGVideoSource(VideoSourceDescription description)
    {
      JpegVideoSource videoSource = new JpegVideoSource(description.SourceString);
      videoSource.FrameInterval = description.FrameInterval;
      videoSource.Login = description.UserName;
      videoSource.Password = description.Password;
      videoSource.RequestTimeout = 20000;
      return videoSource;
    }

    private static IVideoSource MakeNetworkMJPEGVideoSource(VideoSourceDescription description)
    {
      MJpegVideoSource videoSource = new MJpegVideoSource(description.SourceString);
      videoSource.Login = description.UserName;
      videoSource.Password = description.Password;
      videoSource.HttpUserAgent = description.UserAgent;
      videoSource.RequestTimeout = 20000;
      return videoSource;
    }

    private static IVideoSource MakeNetworkRtpStreamVideoSource(VideoSourceDescription description)
    {
      int availablePort = 10000;

      // find an available UDP port
      while (IPGlobalProperties
        .GetIPGlobalProperties()
        .GetActiveUdpListeners()
        .Where(p => p.Port == availablePort)
        .Count() >= 1)
      {
        availablePort++;
      }

      RtpVideoSource videoSource = new RtpVideoSource(availablePort);
      return videoSource;
    }
  }
}
