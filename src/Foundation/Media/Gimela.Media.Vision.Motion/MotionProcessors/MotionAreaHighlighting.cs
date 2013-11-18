using System;
using System.Drawing;
using System.Drawing.Imaging;
using Gimela.Media.Imaging;

namespace Gimela.Media.Vision.Motion
{
  /// <summary>
  /// Motion processing algorithm, which highlights motion areas.
  /// </summary>
  /// 
  /// <remarks><para>The aim of this motion processing algorithm is to highlight
  /// motion areas with grid pattern of the <see cref="HighlightColor">specified color</see>.
  /// </para>
  /// 
  /// <para>Sample usage:</para>
  /// <code>
  /// // create motion detector
  /// MotionDetector detector = new MotionDetector(
  ///     /* motion detection algorithm */,
  ///     new MotionAreaHighlighting( ) );
  /// 
  /// // continuously feed video frames to motion detector
  /// while ( ... )
  /// {
  ///     // process new video frame
  ///     detector.ProcessFrame( videoFrame );
  /// }
  /// </code>
  /// </remarks>
  /// 
  /// <seealso cref="MotionDetector"/>
  /// <seealso cref="IMotionDetector"/>
  /// 
  public class MotionAreaHighlighting : IMotionProcessing
  {
    private Color highlightColor = Color.Red;

    /// <summary>
    /// Color used to highlight motion regions.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>Default value is set to <b>red</b> color.</para>
    /// </remarks>
    /// 
    public Color HighlightColor
    {
      get { return highlightColor; }
      set { highlightColor = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionAreaHighlighting"/> class.
    /// </summary>
    /// 
    public MotionAreaHighlighting() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionAreaHighlighting"/> class.
    /// </summary>
    /// 
    /// <param name="highlightColor">Color used to highlight motion regions.</param>
    /// 
    public MotionAreaHighlighting(Color highlightColor)
    {
      this.highlightColor = highlightColor;
    }

    /// <summary>
    /// Process video and motion frames doing further post processing after
    /// performed motion detection.
    /// </summary>
    /// 
    /// <param name="videoFrame">Original video frame.</param>
    /// <param name="motionFrame">Motion frame provided by motion detection
    /// algorithm (see <see cref="IMotionDetector"/>).</param>
    /// 
    /// <remarks><para>Processes provided motion frame and highlights motion areas
    /// on the original video frame with <see cref="HighlightColor">specified color</see>.</para>
    /// </remarks>
    ///
    public unsafe void ProcessFrame(UnmanagedImage videoFrame, UnmanagedImage motionFrame)
    {
      int width = videoFrame.Width;
      int height = videoFrame.Height;

      if ((motionFrame.Width != width) || (motionFrame.Height != height))
        return;

      byte* src = (byte*)videoFrame.ImageData.ToPointer();
      byte* motion = (byte*)motionFrame.ImageData.ToPointer();

      int srcOffset = videoFrame.Stride - width * 3;
      int motionOffset = motionFrame.Stride - width;

      byte fillR = highlightColor.R;
      byte fillG = highlightColor.G;
      byte fillB = highlightColor.B;

      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++, motion++, src += 3)
        {
          if ((*motion != 0) && (((x + y) & 1) == 0))
          {
            src[RGB.R] = fillR;
            src[RGB.G] = fillG;
            src[RGB.B] = fillB;
          }
        }
        src += srcOffset;
        motion += motionOffset;
      }
    }

    /// <summary>
    /// Reset internal state of motion processing algorithm.
    /// </summary>
    /// 
    /// <remarks><para>The method allows to reset internal state of motion processing
    /// algorithm and prepare it for processing of next video stream or to restart
    /// the algorithm.</para></remarks>
    ///
    public void Reset()
    {
    }
  }
}
