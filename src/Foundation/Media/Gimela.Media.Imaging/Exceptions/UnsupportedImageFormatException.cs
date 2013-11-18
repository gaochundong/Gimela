using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Media.Imaging
{
  /// <summary>
  /// Unsupported image format exception.
  /// </summary>
  /// 
  /// <remarks><para>The unsupported image format exception is thrown in the case when
  /// user passes an image of certain format to an image processing routine, which does
  /// not support the format. Check documentation of image the image processing routine
  /// to discover which formats are supported by the routine.</para>
  /// </remarks>
  /// 
  public class UnsupportedImageFormatException : ArgumentException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class.
    /// </summary>
    public UnsupportedImageFormatException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// 
    public UnsupportedImageFormatException(string message) :
      base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// <param name="paramName">Name of the invalid parameter.</param>
    /// 
    public UnsupportedImageFormatException(string message, string paramName) :
      base(message, paramName) { }
  }
}
