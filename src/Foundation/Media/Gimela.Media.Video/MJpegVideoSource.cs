using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Gimela.Media.Video
{
  /// <summary>
  /// MJPEG视频源
  /// </summary>
  public class MJpegVideoSource : VideoSource
  {
    #region Ctors

    /// <summary>
    /// 初始化MJPEG视频源的实例
    /// </summary>
    /// <param name="source">MJPEG视频源URL</param>
    public MJpegVideoSource(string source)
      : base(source)
    {
    }

    #endregion

    #region Overrides

    /// <summary>
    /// 工作线程
    /// </summary>
    protected override void WorkerThread()
    {
      // buffer to read stream
      byte[] buffer = new byte[BUFFER_SIZE];
      // JPEG magic number
      byte[] jpegMagic = new byte[] { 0xFF, 0xD8, 0xFF };
      // JPEG magic number length
      int jpegMagicLength = 3;
      // default encoding
      ASCIIEncoding encoding = new ASCIIEncoding();

      while (!stopEvent.WaitOne(0, false))
      {
        // HTTP web request
        HttpWebRequest request = null;
        // web responce
        WebResponse response = null;
        // stream for MJPEG downloading
        Stream stream = null;
        // boundary betweeen images (string and binary versions)
        byte[] boundary = null;
        string boudaryStr = null;
        // length of boundary
        int boundaryLen;
        // flag signaling if boundary was checked or not
        bool boundaryIsChecked = false;
        // read amounts and positions
        int read, todo = 0, total = 0, pos = 0, align = 1;
        int start = 0, stop = 0;

        // align
        //  1 = searching for image start
        //  2 = searching for image end

        try
        {
          // create request
          request = (HttpWebRequest)WebRequest.Create(Source);

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

          // check content type
          string contentType = response.ContentType;
          string[] contentTypeArray = contentType.Split('/');

          // "application/octet-stream"
          if ((contentTypeArray[0] == "application") && (contentTypeArray[1] == "octet-stream"))
          {
            boundaryLen = 0;
            boundary = new byte[0];
          }
          else if ((contentTypeArray[0] == "multipart") && (contentType.Contains("mixed")))
          {
            // get boundary
            int boundaryIndex = contentType.IndexOf("boundary", 0);
            if (boundaryIndex != -1)
            {
              boundaryIndex = contentType.IndexOf("=", boundaryIndex + 8);
            }

            if (boundaryIndex == -1)
            {
              // try same scenario as with octet-stream, i.e. without boundaries
              boundaryLen = 0;
              boundary = new byte[0];
            }
            else
            {
              boudaryStr = contentType.Substring(boundaryIndex + 1);
              // remove spaces and double quotes, which may be added by some IP cameras
              boudaryStr = boudaryStr.Trim(' ', '"');

              boundary = encoding.GetBytes(boudaryStr);
              boundaryLen = boundary.Length;
              boundaryIsChecked = false;
            }
          }
          else
          {
            throw new VideoSourceException("Invalid content type.");
          }

          // get response stream
          stream = response.GetResponseStream();
          stream.ReadTimeout = RequestTimeout;

          // loop
          while (!stopEvent.WaitOne(0, false))
          {
            // check total read
            if (total > BUFFER_SIZE - READ_SIZE)
            {
              total = pos = todo = 0;
            }

            // read next portion from stream
            if ((read = stream.Read(buffer, total, READ_SIZE)) == 0)
              throw new VideoSourceException("Cannot read stream from source.");

            total += read;
            todo += read;

            // increment received bytes counter
            bytesReceived += read;

            // do we need to check boundary ?
            if ((boundaryLen != 0) && (!boundaryIsChecked))
            {
              // some IP cameras, like AirLink, claim that boundary is "myboundary",
              // when it is really "--myboundary". this needs to be corrected.

              pos = Find(buffer, boundary, 0, todo);
              // continue reading if boudary was not found
              if (pos == -1)
                continue;

              for (int i = pos - 1; i >= 0; i--)
              {
                byte ch = buffer[i];

                if ((ch == (byte)'\n') || (ch == (byte)'\r'))
                {
                  break;
                }

                boudaryStr = (char)ch + boudaryStr;
              }

              boundary = encoding.GetBytes(boudaryStr);
              boundaryLen = boundary.Length;
              boundaryIsChecked = true;
            }

            // search for image start
            if ((align == 1) && (todo >= jpegMagicLength))
            {
              start = Find(buffer, jpegMagic, pos, todo);
              if (start != -1)
              {
                // found JPEG start
                pos = start + jpegMagicLength;
                todo = total - pos;
                align = 2;
              }
              else
              {
                // delimiter not found
                todo = jpegMagicLength - 1;
                pos = total - todo;
              }
            }

            // search for image end ( boundaryLen can be 0, so need extra check )
            while ((align == 2) && (todo != 0) && (todo >= boundaryLen))
            {
              stop = Find(buffer, (boundaryLen != 0) ? boundary : jpegMagic, pos, todo);

              if (stop != -1)
              {
                pos = stop;
                todo = total - pos;

                // increment frames counter
                framesReceived++;

                // image at stop
                if (CanRaiseNewFrameEvent() && (!stopEvent.WaitOne(0, false)))
                {
                  using (Bitmap bitmap = (Bitmap)Bitmap.FromStream(new MemoryStream(buffer, start, stop - start)))
                  {
                    // notify client
                    RaiseNewFrameEvent(bitmap, DateTime.Now);
                  }
                }

                // shift array
                pos = stop + boundaryLen;
                todo = total - pos;
                Array.Copy(buffer, pos, buffer, 0, todo);

                total = todo;
                pos = 0;
                align = 1;
              }
              else
              {
                // boundary not found
                if (boundaryLen != 0)
                {
                  todo = boundaryLen - 1;
                  pos = total - todo;
                }
                else
                {
                  todo = 0;
                  pos = total;
                }
              }
            }
          }
        }
        catch (ApplicationException)
        {
          // do nothing for Application Exception, which we raised on our own
          // wait for a while before the next try
          Thread.Sleep(250);
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

    #region Private Methods

    /// <summary>
    /// Find subarray in the source array.
    /// </summary>
    /// <param name="array">Source array to search for needle.</param>
    /// <param name="needle">Needle we are searching for.</param>
    /// <param name="startIndex">Start index in source array.</param>
    /// <param name="sourceLength">Number of bytes in source array, where the needle is searched for.</param>
    /// <returns>Returns starting position of the needle if it was found or <b>-1</b> otherwise.</returns>
    private static int Find(byte[] array, byte[] needle, int startIndex, int sourceLength)
    {
      int needleLen = needle.Length;
      int index;

      while (sourceLength >= needleLen)
      {
        // find needle's starting element
        index = Array.IndexOf(array, needle[0], startIndex, sourceLength - needleLen + 1);

        // if we did not find even the first element of the needls, then the search is failed
        if (index == -1)
          return -1;

        int i, p;
        // check for needle
        for (i = 0, p = index; i < needleLen; i++, p++)
        {
          if (array[p] != needle[i])
          {
            break;
          }
        }

        if (i == needleLen)
        {
          // needle was found
          return index;
        }

        // continue to search for needle
        sourceLength -= (index - startIndex + 1);
        startIndex = index + 1;
      }
      return -1;
    }

    #endregion
  }
}
