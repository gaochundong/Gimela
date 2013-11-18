using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;

namespace Gimela.Media.Video
{
  /// <summary>
  /// JPEG视频源
  /// </summary>
  public class JpegVideoSource : VideoSource
  {
    #region Ctors

    /// <summary>
    /// 初始化JPEG视频源的实例
    /// </summary>
    /// <param name="source">JPEG视频源URL</param>
    public JpegVideoSource(string source)
      : base(source)
    {
    }

    #endregion

    #region Properties
    
    /// <summary>
    /// 帧间隔，默认为0.
    /// </summary>
    /// <remarks>The property sets the interval in milliseconds betwen frames. If the property is
    /// set to 100, then the desired frame rate will be 10 frames per second. Default value is 0 -
    /// get new frames as fast as possible.</remarks>
    public int FrameInterval { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// 工作线程
    /// </summary>
    protected override void WorkerThread()
    {
      // buffer to read stream
      byte[] buffer = new byte[BUFFER_SIZE];
      // HTTP web request
      HttpWebRequest request = null;
      // web responce
      WebResponse response = null;
      // stream for JPEG downloading
      Stream stream = null;
      // random generator to add fake parameter for cache preventing
      Random rand = new Random((int)DateTime.Now.Ticks);
      // download start time and duration
      DateTime start;
      TimeSpan span;

      while (!stopEvent.WaitOne(0, false))
      {
        int read, total = 0;

        try
        {
          // set download start time
          start = DateTime.Now;

          // create request
          if (!PreventCaching)
          {
            // request without cache prevention
            request = (HttpWebRequest)WebRequest.Create(Source);
          }
          else
          {
            // request with cache prevention
            request = (HttpWebRequest)WebRequest.Create(Source + ((Source.IndexOf('?') == -1) ? '?' : '&') + "fake=" + rand.Next().ToString());
          }

          // set user agent
          if (!string.IsNullOrEmpty(HttpUserAgent))
          {
            request.UserAgent = HttpUserAgent;
          }

          // set proxy
          if (Proxy != null)
          {
            request.Proxy = Proxy;
          }

          // set timeout value for the request
          request.Timeout = RequestTimeout;

          // set login and password
          if (!string.IsNullOrEmpty(Login) && Password != null)
            request.Credentials = new NetworkCredential(Login, Password);

          // set connection group name
          if (UseSeparateConnectionGroup)
            request.ConnectionGroupName = GetHashCode().ToString();

          // get response
          response = request.GetResponse();

          // get response stream
          stream = response.GetResponseStream();

          // set read stream timeout
          stream.ReadTimeout = RequestTimeout;

          // loop
          while (!stopEvent.WaitOne(0, false))
          {
            // check total read
            if (total > BUFFER_SIZE - READ_SIZE)
            {
              total = 0;
            }

            // read next portion from stream
            if ((read = stream.Read(buffer, total, READ_SIZE)) == 0)
              break;

            total += read;

            // increment received bytes counter
            bytesReceived += read;
          }

          if (!stopEvent.WaitOne(0, false))
          {
            // increment frames counter
            framesReceived++;

            // provide new image to clients
            if (CanRaiseNewFrameEvent())
            {
              using (Bitmap bitmap = (Bitmap)Bitmap.FromStream(new MemoryStream(buffer, 0, total)))
              {
                // notify client
                RaiseNewFrameEvent(bitmap, DateTime.Now);
              }
            }
          }

          // wait for a while ?
          if (FrameInterval > 0)
          {
            // get download duration
            span = DateTime.Now.Subtract(start);

            // miliseconds to sleep
            int msec = FrameInterval - (int)span.TotalMilliseconds;

            // wait for a while
            if ((msec > 0) && (stopEvent.WaitOne(msec, false)))
              break;
          }
        }
        catch (ThreadAbortException)
        {
          break; // we abort this thread
        }
        catch (Exception ex)
        {
          // provide information to clients
          RaiseVideoSourceExceptionEvent(ex.Message);

          // wait for a while before the next try
          Thread.Sleep(250);
        }
        finally
        {
          // abort request
          if (request != null)
          {
            request.Abort();
            request = null;
          }
          // close response stream
          if (stream != null)
          {
            stream.Close();
            stream = null;
          }
          // close response
          if (response != null)
          {
            response.Close();
            response = null;
          }
        }

        // need to stop ?
        if (stopEvent.WaitOne(0, false))
          break;
      }

      // notify client that the video is finished
      RaiseVideoSourceFinishedEvent(VideoSourceFinishedReasonType.StoppedByUser);
    }

    #endregion
  }
}
