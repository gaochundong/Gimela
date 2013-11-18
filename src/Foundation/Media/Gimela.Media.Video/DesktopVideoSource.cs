using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

namespace Gimela.Media.Video
{
  /// <summary>
  /// 桌面视频源
  /// </summary>
  public class DesktopVideoSource : JpegVideoSource
  {
    #region Ctors

    /// <summary>
    /// 初始化桌面视频源的实例
    /// </summary>
    /// <param name="screenIndex">桌面序号</param>
    public DesktopVideoSource(int screenIndex)
      : base(screenIndex.ToString())
    {
      ScreenIndex = screenIndex;
    }

    #endregion

    #region Properties

    /// <summary>
    /// 桌面序号
    /// </summary>
    public int ScreenIndex { get; private set; }

    /// <summary>
    /// 是否调整图片大小
    /// </summary>
    public bool IsResized { get; set; }

    /// <summary>
    /// 调整图片宽度
    /// </summary>
    public int ResizeWidth { get; set; }

    /// <summary>
    /// 调整图片高度
    /// </summary>
    public int ResizeHeight { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// 工作线程
    /// </summary>
    protected override void WorkerThread()
    {
      DateTime start;
      TimeSpan span;

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
              // get screen image
              Rectangle screenSize = Screen.AllScreens[ScreenIndex].Bounds;
              Bitmap snapshot = new Bitmap(screenSize.Width, screenSize.Height, PixelFormat.Format24bppRgb);
              using (Graphics g = Graphics.FromImage(snapshot))
              {
                g.CopyFromScreen(Screen.AllScreens[ScreenIndex].Bounds.X, Screen.AllScreens[ScreenIndex].Bounds.Y, 0, 0, new Size(screenSize.Width, screenSize.Height));
              }
                            
              // resize screen image
              if (IsResized)
              {
                Bitmap bitmap = new Bitmap(ResizeWidth, ResizeHeight, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage((Image)bitmap))
                {
                  g.DrawImage(snapshot, 0, 0, bitmap.Width, bitmap.Height);
                }

                snapshot.Dispose();
                snapshot = null;

                // notify client
                RaiseNewFrameEvent(bitmap, DateTime.Now);

                // release the image
                bitmap.Dispose();
                bitmap = null;
              }
              else
              {
                // notify client
                RaiseNewFrameEvent(snapshot, DateTime.Now);

                // release the image
                snapshot.Dispose();
                snapshot = null;
              }
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
