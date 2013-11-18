using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Media.Imaging
{
  /// <summary>
  /// HSL components.
  /// </summary>
  /// 
  /// <remarks>The class encapsulates <b>HSL</b> color components.</remarks>
  /// 
  public class HSL
  {
    /// <summary>
    /// Hue component.
    /// </summary>
    /// 
    /// <remarks>Hue is measured in the range of [0, 359].</remarks>
    /// 
    public int Hue;

    /// <summary>
    /// Saturation component.
    /// </summary>
    /// 
    /// <remarks>Saturation is measured in the range of [0, 1].</remarks>
    /// 
    public float Saturation;

    /// <summary>
    /// Luminance value.
    /// </summary>
    /// 
    /// <remarks>Luminance is measured in the range of [0, 1].</remarks>
    /// 
    public float Luminance;

    /// <summary>
    /// Initializes a new instance of the <see cref="HSL"/> class.
    /// </summary>
    public HSL() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HSL"/> class.
    /// </summary>
    /// 
    /// <param name="hue">Hue component.</param>
    /// <param name="saturation">Saturation component.</param>
    /// <param name="luminance">Luminance component.</param>
    /// 
    public HSL(int hue, float saturation, float luminance)
    {
      this.Hue = hue;
      this.Saturation = saturation;
      this.Luminance = luminance;
    }

    /// <summary>
    /// Convert from RGB to HSL color space.
    /// </summary>
    /// 
    /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
    /// <param name="hsl">Destination color in <b>HSL</b> color space.</param>
    /// 
    /// <remarks><para>See <a href="http://en.wikipedia.org/wiki/HSI_color_space#Conversion_from_RGB_to_HSL_or_HSV">HSL and HSV Wiki</a>
    /// for information about the algorithm to convert from RGB to HSL.</para></remarks>
    /// 
    public static void FromRGB(RGB rgb, HSL hsl)
    {
      float r = (rgb.Red / 255.0f);
      float g = (rgb.Green / 255.0f);
      float b = (rgb.Blue / 255.0f);

      float min = Math.Min(Math.Min(r, g), b);
      float max = Math.Max(Math.Max(r, g), b);
      float delta = max - min;

      // get luminance value
      hsl.Luminance = (max + min) / 2;

      if (delta == 0)
      {
        // gray color
        hsl.Hue = 0;
        hsl.Saturation = 0.0f;
      }
      else
      {
        // get saturation value
        hsl.Saturation = (hsl.Luminance <= 0.5) ? (delta / (max + min)) : (delta / (2 - max - min));

        // get hue value
        float hue;

        if (r == max)
        {
          hue = ((g - b) / 6) / delta;
        }
        else if (g == max)
        {
          hue = (1.0f / 3) + ((b - r) / 6) / delta;
        }
        else
        {
          hue = (2.0f / 3) + ((r - g) / 6) / delta;
        }

        // correct hue if needed
        if (hue < 0)
          hue += 1;
        if (hue > 1)
          hue -= 1;

        hsl.Hue = (int)(hue * 360);
      }
    }

    /// <summary>
    /// Convert from RGB to HSL color space.
    /// </summary>
    /// 
    /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
    /// 
    /// <returns>Returns <see cref="HSL"/> instance, which represents converted color value.</returns>
    /// 
    public static HSL FromRGB(RGB rgb)
    {
      HSL hsl = new HSL();
      FromRGB(rgb, hsl);
      return hsl;
    }

    /// <summary>
    /// Convert from HSL to RGB color space.
    /// </summary>
    /// 
    /// <param name="hsl">Source color in <b>HSL</b> color space.</param>
    /// <param name="rgb">Destination color in <b>RGB</b> color space.</param>
    /// 
    public static void ToRGB(HSL hsl, RGB rgb)
    {
      if (hsl.Saturation == 0)
      {
        // gray values
        rgb.Red = rgb.Green = rgb.Blue = (byte)(hsl.Luminance * 255);
      }
      else
      {
        float v1, v2;
        float hue = (float)hsl.Hue / 360;

        v2 = (hsl.Luminance < 0.5) ?
            (hsl.Luminance * (1 + hsl.Saturation)) :
            ((hsl.Luminance + hsl.Saturation) - (hsl.Luminance * hsl.Saturation));
        v1 = 2 * hsl.Luminance - v2;

        rgb.Red = (byte)(255 * Hue_2_RGB(v1, v2, hue + (1.0f / 3)));
        rgb.Green = (byte)(255 * Hue_2_RGB(v1, v2, hue));
        rgb.Blue = (byte)(255 * Hue_2_RGB(v1, v2, hue - (1.0f / 3)));
      }
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

    #region Private members
    // HSL to RGB helper routine
    private static float Hue_2_RGB(float v1, float v2, float vH)
    {
      if (vH < 0)
        vH += 1;
      if (vH > 1)
        vH -= 1;
      if ((6 * vH) < 1)
        return (v1 + (v2 - v1) * 6 * vH);
      if ((2 * vH) < 1)
        return v2;
      if ((3 * vH) < 2)
        return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);
      return v1;
    }
    #endregion
  }
}
