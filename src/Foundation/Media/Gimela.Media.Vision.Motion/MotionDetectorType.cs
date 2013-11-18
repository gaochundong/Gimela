using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Media.Vision.Motion
{
  /// <summary>
  /// Motion detector type
  /// </summary>
  public enum MotionDetectorType
  {
    /// <summary>
    /// Motion detector based on two continues frames difference.
    /// </summary>
    TwoFramesDifference,
    /// <summary>
    /// Motion detector based on difference with predefined background frame.
    /// </summary>
    CustomFrameDifference,
    /// <summary>
    /// Motion detector based on simple background modeling.
    /// </summary>
    SimpleBackgroundModeling,
  }
}
