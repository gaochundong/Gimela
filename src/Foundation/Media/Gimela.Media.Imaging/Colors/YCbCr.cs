using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Media.Imaging
{
  /// <summary>
  /// YCbCr components.
  /// </summary>
  /// 
  /// <remarks>The class encapsulates <b>YCbCr</b> color components.</remarks>
  /// 
  public class YCbCr
  {
    /// <summary>
    /// Index of <b>Y</b> component.
    /// </summary>
    public const short YIndex = 0;

    /// <summary>
    /// Index of <b>Cb</b> component.
    /// </summary>
    public const short CbIndex = 1;

    /// <summary>
    /// Index of <b>Cr</b> component.
    /// </summary>
    public const short CrIndex = 2;

    /// <summary>
    /// <b>Y</b> component.
    /// </summary>
    public float Y;

    /// <summary>
    /// <b>Cb</b> component.
    /// </summary>
    public float Cb;

    /// <summary>
    /// <b>Cr</b> component.
    /// </summary>
    public float Cr;

    /// <summary>
    /// Initializes a new instance of the <see cref="YCbCr"/> class.
    /// </summary>
    public YCbCr() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="YCbCr"/> class.
    /// </summary>
    /// 
    /// <param name="y"><b>Y</b> component.</param>
    /// <param name="cb"><b>Cb</b> component.</param>
    /// <param name="cr"><b>Cr</b> component.</param>
    /// 
    public YCbCr(float y, float cb, float cr)
    {
      this.Y = Math.Max(0.0f, Math.Min(1.0f, y));
      this.Cb = Math.Max(-0.5f, Math.Min(0.5f, cb));
      this.Cr = Math.Max(-0.5f, Math.Min(0.5f, cr));
    }

    /// <summary>
    /// Convert from RGB to YCbCr color space (Rec 601-1 specification). 
    /// </summary>
    /// 
    /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
    /// <param name="ycbcr">Destination color in <b>YCbCr</b> color space.</param>
    /// 
    public static void FromRGB(RGB rgb, YCbCr ycbcr)
    {
      float r = (float)rgb.Red / 255;
      float g = (float)rgb.Green / 255;
      float b = (float)rgb.Blue / 255;

      ycbcr.Y = (float)(0.2989 * r + 0.5866 * g + 0.1145 * b);
      ycbcr.Cb = (float)(-0.1687 * r - 0.3313 * g + 0.5000 * b);
      ycbcr.Cr = (float)(0.5000 * r - 0.4184 * g - 0.0816 * b);
    }

    /// <summary>
    /// Convert from RGB to YCbCr color space (Rec 601-1 specification).
    /// </summary>
    /// 
    /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
    /// 
    /// <returns>Returns <see cref="YCbCr"/> instance, which represents converted color value.</returns>
    /// 
    public static YCbCr FromRGB(RGB rgb)
    {
      YCbCr ycbcr = new YCbCr();
      FromRGB(rgb, ycbcr);
      return ycbcr;
    }

    /// <summary>
    /// Convert from YCbCr to RGB color space.
    /// </summary>
    /// 
    /// <param name="ycbcr">Source color in <b>YCbCr</b> color space.</param>
    /// <param name="rgb">Destination color in <b>RGB</b> color spacs.</param>
    /// 
    public static void ToRGB(YCbCr ycbcr, RGB rgb)
    {
      // don't warry about zeros. compiler will remove them
      float r = Math.Max(0.0f, Math.Min(1.0f, (float)(ycbcr.Y + 0.0000 * ycbcr.Cb + 1.4022 * ycbcr.Cr)));
      float g = Math.Max(0.0f, Math.Min(1.0f, (float)(ycbcr.Y - 0.3456 * ycbcr.Cb - 0.7145 * ycbcr.Cr)));
      float b = Math.Max(0.0f, Math.Min(1.0f, (float)(ycbcr.Y + 1.7710 * ycbcr.Cb + 0.0000 * ycbcr.Cr)));

      rgb.Red = (byte)(r * 255);
      rgb.Green = (byte)(g * 255);
      rgb.Blue = (byte)(b * 255);
    }

    /// <summary>
    /// Convert the color to <b>RGB</b> color space.
    /// </summary>
    /// 
    /// <returns>Returns <see cref="RGB"/> instance, which represents converted color value.</returns>
    /// 
    public RGB ToRGB()
    {
      RGB rgb = new RGB();
      ToRGB(this, rgb);
      return rgb;
    }
  }
}
