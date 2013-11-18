using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Gimela.Media.Video
{
  /// <summary>
  /// 本地图片文件夹视频源
  /// </summary>
  public class PictureVideoSource : JpegVideoSource
  {
    #region Ctors

    /// <summary>
    /// 本地图片文件夹视频源
    /// </summary>
    /// <param name="folder">本地图片文件夹</param>
    public PictureVideoSource(string folder)
      : base(folder)
    {
      Folder = folder;
      SearchPattern = @"*.jpg";
    }

    #endregion

    #region Properties

    /// <summary>
    /// 本地图片文件夹
    /// </summary>
    public string Folder { get; private set; }

    /// <summary>
    /// 文件搜索匹配规则，默认*.jpg
    /// </summary>
    public string SearchPattern { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// 工作线程
    /// </summary>
    protected override void WorkerThread()
    {
      DateTime start;
      TimeSpan span;

      // picture file index
      int index = 0;

      while (true)
      {
        try
        {
          start = DateTime.Now;
          if (!stopEvent.WaitOne(0, true))
          {
            // increment frames counter
            framesReceived++;

            // provide new image to clients
            if (CanRaiseNewFrameEvent())
            {
              // get pictures from folder
              string[] files = Directory.GetFiles(Folder, SearchPattern);
              if (index > files.Length - 1) index = 0;

              // must have files in specified folder
              if (files.Length > 0)
              {
                Bitmap frame = new Bitmap(files[index]);

                // notify client
                RaiseNewFrameEvent(frame, DateTime.Now);

                // release the image
                frame.Dispose();
                frame = null;
              }

              // index for next picture
              index++;
            }
          }

          // wait for a while ?
          if (FrameInterval > 0)
          {
            // get wait duration
            span = DateTime.Now.Subtract(start);

            // miliseconds to sleep
            int msec = FrameInterval - (int)span.TotalMilliseconds;

            while ((msec > 0) && (stopEvent.WaitOne(0, true) == false))
            {
              // sleeping ...
              Thread.Sleep((msec < 100) ? msec : 100);
              msec -= 100;
            }
          }
        }
        catch (Exception ex)
        {
          // provide information to clients
          RaiseVideoSourceExceptionEvent(ex.Message);

          // wait for a while before the next try
          Thread.Sleep(250);
        }

        // need to stop ?
        if (stopEvent.WaitOne(0, true))
          break;
      }

      // notify client that the video is finished
      RaiseVideoSourceFinishedEvent(VideoSourceFinishedReasonType.StoppedByUser);
    }

    #endregion
  }
}
