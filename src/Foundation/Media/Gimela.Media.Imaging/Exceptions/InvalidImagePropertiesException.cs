using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Media.Imaging
{
  /// <summary>
  /// Invalid image properties exception.
  /// </summary>
  /// 
  /// <remarks><para>The invalid image properties exception is thrown in the case when
  /// user provides an image with certain properties, which are treated as invalid by
  /// particular image processing routine. Another case when this exception is
  /// thrown is the case when user tries to access some properties of an image (or
  /// of a recently processed image by some routine), which are not valid for that image.</para>
  /// </remarks>
  /// 
  public class InvalidImagePropertiesException : ArgumentException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidImagePropertiesException"/> class.
    /// </summary>
    public InvalidImagePropertiesException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidImagePropertiesException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// 
    public InvalidImagePropertiesException(string message) :
      base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidImagePropertiesException"/> class.
    /// </summary>
    /// 
    /// <param name="message">Message providing some additional information.</param>
    /// <param name="paramName">Name of the invalid parameter.</param>
    /// 
    public InvalidImagePropertiesException(string message, string paramName) :
      base(message, paramName) { }
  }
}
