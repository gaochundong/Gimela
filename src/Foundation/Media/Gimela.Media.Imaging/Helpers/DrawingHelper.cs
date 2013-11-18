using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Gimela.Media.Utilities;

namespace Gimela.Media.Imaging
{
  /// <summary>
  /// Drawing primitives.
  /// </summary>
  /// 
  /// <remarks><para>The class allows to do drawing of some primitives directly on
  /// locked image data or unmanaged image.</para>
  /// 
  /// <para>All methods of this class support drawing only on color 24/32 bpp images and
  /// on grayscale 8 bpp indexed images.</para>
  /// </remarks>
  /// 
  public static class DrawingHelper
  {
    /// <summary>
    /// Fill rectangle on the specified image.
    /// </summary>
    /// 
    /// <param name="imageData">Source image data to draw on.</param>
    /// <param name="rectangle">Rectangle's coordinates to fill.</param>
    /// <param name="color">Rectangle's color.</param>
    /// 
    /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
    /// 
    public static unsafe void FillRectangle(BitmapData imageData, Rectangle rectangle, Color color)
    {
      FillRectangle(new UnmanagedImage(imageData), rectangle, color);
    }

    /// <summary>
    /// Fill rectangle on the specified image.
    /// </summary>
    /// 
    /// <param name="image">Source image to draw on.</param>
    /// <param name="rectangle">Rectangle's coordinates to fill.</param>
    /// <param name="color">Rectangle's color.</param>
    /// 
    /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
    /// 
    public static unsafe void FillRectangle(UnmanagedImage image, Rectangle rectangle, Color color)
    {
      CheckPixelFormat(image.PixelFormat);

      int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;

      // image dimension
      int imageWidth = image.Width;
      int imageHeight = image.Height;
      int stride = image.Stride;

      // rectangle dimension and position
      int rectX1 = rectangle.X;
      int rectY1 = rectangle.Y;
      int rectX2 = rectangle.X + rectangle.Width - 1;
      int rectY2 = rectangle.Y + rectangle.Height - 1;

      // check if rectangle is in the image
      if ((rectX1 >= imageWidth) || (rectY1 >= imageHeight) || (rectX2 < 0) || (rectY2 < 0))
      {
        // nothing to draw
        return;
      }

      int startX = Math.Max(0, rectX1);
      int stopX = Math.Min(imageWidth - 1, rectX2);
      int startY = Math.Max(0, rectY1);
      int stopY = Math.Min(imageHeight - 1, rectY2);

      // do the job
      byte* ptr = (byte*)image.ImageData.ToPointer() + startY * stride + startX * pixelSize;

      if (image.PixelFormat == PixelFormat.Format8bppIndexed)
      {
        // grayscale image
        byte gray = (byte)(0.2125 * color.R + 0.7154 * color.G + 0.0721 * color.B);

        int fillWidth = stopX - startX + 1;

        for (int y = startY; y <= stopY; y++)
        {
          UnmanagedMemoryHelper.SetUnmanagedMemory(ptr, gray, fillWidth);
          ptr += stride;
        }
      }
      else
      {
        // color image
        byte red = color.R;
        byte green = color.G;
        byte blue = color.B;

        int offset = stride - (stopX - startX + 1) * pixelSize;

        for (int y = startY; y <= stopY; y++)
        {
          for (int x = startX; x <= stopX; x++, ptr += pixelSize)
          {
            ptr[RGB.R] = red;
            ptr[RGB.G] = green;
            ptr[RGB.B] = blue;
          }
          ptr += offset;
        }
      }
    }

    /// <summary>
    /// Draw rectangle on the specified image.
    /// </summary>
    /// 
    /// <param name="imageData">Source image data to draw on.</param>
    /// <param name="rectangle">Rectangle's coordinates to draw.</param>
    /// <param name="color">Rectangle's color.</param>
    /// 
    /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
    /// 
    public static unsafe void Rectangle(BitmapData imageData, Rectangle rectangle, Color color)
    {
      Rectangle(new UnmanagedImage(imageData), rectangle, color);
    }

    /// <summary>
    /// Draw rectangle on the specified image.
    /// </summary>
    /// 
    /// <param name="image">Source image to draw on.</param>
    /// <param name="rectangle">Rectangle's coordinates to draw.</param>
    /// <param name="color">Rectangle's color.</param>
    /// 
    /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
    /// 
    public static unsafe void Rectangle(UnmanagedImage image, Rectangle rectangle, Color color)
    {
      CheckPixelFormat(image.PixelFormat);

      int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;

      // image dimension
      int imageWidth = image.Width;
      int imageHeight = image.Height;
      int stride = image.Stride;

      // rectangle dimension and position
      int rectX1 = rectangle.X;
      int rectY1 = rectangle.Y;
      int rectX2 = rectangle.X + rectangle.Width - 1;
      int rectY2 = rectangle.Y + rectangle.Height - 1;

      // check if rectangle is in the image
      if ((rectX1 >= imageWidth) || (rectY1 >= imageHeight) || (rectX2 < 0) || (rectY2 < 0))
      {
        // nothing to draw
        return;
      }

      int startX = Math.Max(0, rectX1);
      int stopX = Math.Min(imageWidth - 1, rectX2);
      int startY = Math.Max(0, rectY1);
      int stopY = Math.Min(imageHeight - 1, rectY2);

      if (image.PixelFormat == PixelFormat.Format8bppIndexed)
      {
        // grayscale image
        byte gray = (byte)(0.2125 * color.R + 0.7154 * color.G + 0.0721 * color.B);

        // draw top horizontal line
        if (rectY1 >= 0)
        {
          byte* ptr = (byte*)image.ImageData.ToPointer() + rectY1 * stride + startX;

          UnmanagedMemoryHelper.SetUnmanagedMemory(ptr, gray, stopX - startX);
        }

        // draw bottom horizontal line
        if (rectY2 < imageHeight)
        {
          byte* ptr = (byte*)image.ImageData.ToPointer() + rectY2 * stride + startX;

          UnmanagedMemoryHelper.SetUnmanagedMemory(ptr, gray, stopX - startX);
        }

        // draw left vertical line
        if (rectX1 >= 0)
        {
          byte* ptr = (byte*)image.ImageData.ToPointer() + startY * stride + rectX1;

          for (int y = startY; y <= stopY; y++, ptr += stride)
          {
            *ptr = gray;
          }
        }

        // draw right vertical line
        if (rectX2 < imageWidth)
        {
          byte* ptr = (byte*)image.ImageData.ToPointer() + startY * stride + rectX2;

          for (int y = startY; y <= stopY; y++, ptr += stride)
          {
            *ptr = gray;
          }
        }
      }
      else
      {
        // color image
        byte red = color.R;
        byte green = color.G;
        byte blue = color.B;

        // draw top horizontal line
        if (rectY1 >= 0)
        {
          byte* ptr = (byte*)image.ImageData.ToPointer() + rectY1 * stride + startX * pixelSize;

          for (int x = startX; x <= stopX; x++, ptr += pixelSize)
          {
            ptr[RGB.R] = red;
            ptr[RGB.G] = green;
            ptr[RGB.B] = blue;
          }
        }

        // draw bottom horizontal line
        if (rectY2 < imageHeight)
        {
          byte* ptr = (byte*)image.ImageData.ToPointer() + rectY2 * stride + startX * pixelSize;

          for (int x = startX; x <= stopX; x++, ptr += pixelSize)
          {
            ptr[RGB.R] = red;
            ptr[RGB.G] = green;
            ptr[RGB.B] = blue;
          }
        }

        // draw left vertical line
        if (rectX1 >= 0)
        {
          byte* ptr = (byte*)image.ImageData.ToPointer() + startY * stride + rectX1 * pixelSize;

          for (int y = startY; y <= stopY; y++, ptr += stride)
          {
            ptr[RGB.R] = red;
            ptr[RGB.G] = green;
            ptr[RGB.B] = blue;
          }
        }

        // draw right vertical line
        if (rectX2 < imageWidth)
        {
          byte* ptr = (byte*)image.ImageData.ToPointer() + startY * stride + rectX2 * pixelSize;

          for (int y = startY; y <= stopY; y++, ptr += stride)
          {
            ptr[RGB.R] = red;
            ptr[RGB.G] = green;
            ptr[RGB.B] = blue;
          }
        }
      }
    }

    /// <summary>
    /// Draw a line on the specified image.
    /// </summary>
    /// 
    /// <param name="imageData">Source image data to draw on.</param>
    /// <param name="point1">The first point to connect.</param>
    /// <param name="point2">The second point to connect.</param>
    /// <param name="color">Line's color.</param>
    /// 
    /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
    /// 
    public static unsafe void Line(BitmapData imageData, IntPoint point1, IntPoint point2, Color color)
    {
      Line(new UnmanagedImage(imageData), point1, point2, color);
    }

    /// <summary>
    /// Draw a line on the specified image.
    /// </summary>
    /// 
    /// <param name="image">Source image to draw on.</param>
    /// <param name="point1">The first point to connect.</param>
    /// <param name="point2">The second point to connect.</param>
    /// <param name="color">Line's color.</param>
    /// 
    /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
    /// 
    public static unsafe void Line(UnmanagedImage image, IntPoint point1, IntPoint point2, Color color)
    {
      // TODO: faster line drawing algorithm may be implemented with integer math

      CheckPixelFormat(image.PixelFormat);

      int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;

      // image dimension
      int imageWidth = image.Width;
      int imageHeight = image.Height;
      int stride = image.Stride;

      // check if there is something to draw
      if (
          ((point1.X < 0) && (point2.X < 0)) ||
          ((point1.Y < 0) && (point2.Y < 0)) ||
          ((point1.X >= imageWidth) && (point2.X >= imageWidth)) ||
          ((point1.Y >= imageHeight) && (point2.Y >= imageHeight)))
      {
        // nothing to draw
        return;
      }

      CheckEndPoint(imageWidth, imageHeight, point1, ref point2);
      CheckEndPoint(imageWidth, imageHeight, point2, ref point1);

      // check again if there is something to draw
      if (
          ((point1.X < 0) && (point2.X < 0)) ||
          ((point1.Y < 0) && (point2.Y < 0)) ||
          ((point1.X >= imageWidth) && (point2.X >= imageWidth)) ||
          ((point1.Y >= imageHeight) && (point2.Y >= imageHeight)))
      {
        // nothing to draw
        return;
      }

      int startX = point1.X;
      int startY = point1.Y;
      int stopX = point2.X;
      int stopY = point2.Y;

      // compute pixel for grayscale image
      byte gray = 0;
      if (image.PixelFormat == PixelFormat.Format8bppIndexed)
      {
        gray = (byte)(0.2125 * color.R + 0.7154 * color.G + 0.0721 * color.B);
      }

      // draw the line
      int dx = stopX - startX;
      int dy = stopY - startY;

      if (Math.Abs(dx) >= Math.Abs(dy))
      {
        // the line is more horizontal, we'll plot along the X axis
        float slope = (dx != 0) ? (float)dy / dx : 0;
        int step = (dx > 0) ? 1 : -1;

        // correct dx so last point is included as well
        dx += step;

        if (image.PixelFormat == PixelFormat.Format8bppIndexed)
        {
          // grayscale image
          for (int x = 0; x != dx; x += step)
          {
            int px = startX + x;
            int py = (int)((float)startY + (slope * (float)x));

            byte* ptr = (byte*)image.ImageData.ToPointer() + py * stride + px;
            *ptr = gray;
          }
        }
        else
        {
          // color image
          for (int x = 0; x != dx; x += step)
          {
            int px = startX + x;
            int py = (int)((float)startY + (slope * (float)x));

            byte* ptr = (byte*)image.ImageData.ToPointer() + py * stride + px * pixelSize;

            ptr[RGB.R] = color.R;
            ptr[RGB.G] = color.G;
            ptr[RGB.B] = color.B;
          }
        }
      }
      else
      {
        // the line is more vertical, we'll plot along the y axis.
        float slope = (dy != 0) ? (float)dx / dy : 0;
        int step = (dy > 0) ? 1 : -1;

        // correct dy so last point is included as well
        dy += step;

        if (image.PixelFormat == PixelFormat.Format8bppIndexed)
        {
          // grayscale image
          for (int y = 0; y != dy; y += step)
          {
            int px = (int)((float)startX + (slope * (float)y));
            int py = startY + y;

            byte* ptr = (byte*)image.ImageData.ToPointer() + py * stride + px;
            *ptr = gray;
          }
        }
        else
        {
          // color image
          for (int y = 0; y != dy; y += step)
          {
            int px = (int)((float)startX + (slope * (float)y));
            int py = startY + y;

            byte* ptr = (byte*)image.ImageData.ToPointer() + py * stride + px * pixelSize;

            ptr[RGB.R] = color.R;
            ptr[RGB.G] = color.G;
            ptr[RGB.B] = color.B;
          }
        }
      }
    }

    /// <summary>
    /// Draw a polygon on the specified image.
    /// </summary>
    /// 
    /// <param name="imageData">Source image data to draw on.</param>
    /// <param name="points">Points of the polygon to draw.</param>
    /// <param name="color">Polygon's color.</param>
    /// 
    /// <remarks><para>The method draws a polygon by connecting all points from the
    /// first one to the last one and then connecting the last point with the first one.
    /// </para></remarks>
    /// 
    public static void Polygon(BitmapData imageData, List<IntPoint> points, Color color)
    {
      Polygon(new UnmanagedImage(imageData), points, color);
    }

    /// <summary>
    /// Draw a polygon on the specified image.
    /// </summary>
    /// 
    /// <param name="image">Source image to draw on.</param>
    /// <param name="points">Points of the polygon to draw.</param>
    /// <param name="color">Polygon's color.</param>
    /// 
    /// <remarks><para>The method draws a polygon by connecting all points from the
    /// first one to the last one and then connecting the last point with the first one.
    /// </para></remarks>
    /// 
    public static void Polygon(UnmanagedImage image, List<IntPoint> points, Color color)
    {
      for (int i = 1, n = points.Count; i < n; i++)
      {
        Line(image, points[i - 1], points[i], color);
      }
      Line(image, points[points.Count - 1], points[0], color);
    }

    /// <summary>
    /// Draw a polyline on the specified image.
    /// </summary>
    /// 
    /// <param name="imageData">Source image data to draw on.</param>
    /// <param name="points">Points of the polyline to draw.</param>
    /// <param name="color">polyline's color.</param>
    /// 
    /// <remarks><para>The method draws a polyline by connecting all points from the
    /// first one to the last one. Unlike <see cref="Polygon( BitmapData, List{IntPoint}, Color )"/>
    /// method, this method does not connect the last point with the first one.
    /// </para></remarks>
    /// 
    public static void Polyline(BitmapData imageData, List<IntPoint> points, Color color)
    {
      Polyline(new UnmanagedImage(imageData), points, color);
    }

    /// <summary>
    /// Draw a polyline on the specified image.
    /// </summary>
    /// 
    /// <param name="image">Source image to draw on.</param>
    /// <param name="points">Points of the polyline to draw.</param>
    /// <param name="color">polyline's color.</param>
    /// 
    /// <remarks><para>The method draws a polyline by connecting all points from the
    /// first one to the last one. Unlike <see cref="Polygon( UnmanagedImage, List{IntPoint}, Color )"/>
    /// method, this method does not connect the last point with the first one.
    /// </para></remarks>
    /// 
    public static void Polyline(UnmanagedImage image, List<IntPoint> points, Color color)
    {
      for (int i = 1, n = points.Count; i < n; i++)
      {
        Line(image, points[i - 1], points[i], color);
      }
    }

    // Check for supported pixel format
    private static void CheckPixelFormat(PixelFormat format)
    {
      // check pixel format
      if (
          (format != PixelFormat.Format24bppRgb) &&
          (format != PixelFormat.Format8bppIndexed) &&
          (format != PixelFormat.Format32bppArgb) &&
          (format != PixelFormat.Format32bppRgb)
          )
      {
        throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
      }
    }

    // Check end point and make sure it is in the image
    private static void CheckEndPoint(int width, int height, IntPoint start, ref IntPoint end)
    {
      if (end.X >= width)
      {
        int newEndX = width - 1;

        double c = (double)(newEndX - start.X) / (end.X - start.X);

        end.Y = (int)(start.Y + c * (end.Y - start.Y));
        end.X = newEndX;
      }

      if (end.Y >= height)
      {
        int newEndY = height - 1;

        double c = (double)(newEndY - start.Y) / (end.Y - start.Y);

        end.X = (int)(start.X + c * (end.X - start.X));
        end.Y = newEndY;
      }

      if (end.X < 0)
      {
        double c = (double)(0 - start.X) / (end.X - start.X);

        end.Y = (int)(start.Y + c * (end.Y - start.Y));
        end.X = 0;
      }

      if (end.Y < 0)
      {
        double c = (double)(0 - start.Y) / (end.Y - start.Y);

        end.X = (int)(start.X + c * (end.X - start.X));
        end.Y = 0;
      }
    }
  }
}
