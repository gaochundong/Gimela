using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Media.Vision.Motion
{
  /// <summary>
  /// Motion processing algorithm
  /// </summary>
  public enum MotionProcessorType
  {
    /// <summary>
    /// Motion processing algorithm, which counts separate moving objects and highlights them.
    /// </summary>
    BlobCountingObjects,
    /// <summary>
    /// Motion processing algorithm, which performs grid processing of motion frame.
    /// </summary>
    GridMotionArea,
    /// <summary>
    /// Motion processing algorithm, which highlights motion areas.
    /// </summary>
    MotionAreaHighlighting,
    /// <summary>
    /// Motion processing algorithm, which highlights border of motion areas.
    /// </summary>
    MotionBorderHighlighting,
  }
}
