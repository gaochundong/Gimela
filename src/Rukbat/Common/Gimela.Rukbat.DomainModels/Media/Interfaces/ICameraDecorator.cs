using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using Gimela.Media.Vision.Motion;

namespace Gimela.Rukbat.DomainModels
{
  public interface ICameraDecorator
  {
    bool IsFlipX { get; set; }
    bool IsFlipY { get; set; }

    bool IsDisplayTimestamp { get; set; }
    string TimestampFormat { get; set; }
    string TimestampFontFamily { get; set; }
    float TimestampFontSize { get; set; }
    Color TimestampColor { get; set; }
    bool IsDisplayTimestampBackground { get; set; }
    Color TimestampBackgroundColor { get; set; }

    bool IsDisplayOnScreenDisplay { get; set; }
    string OnScreenDisplayText { get; set; }
    string OnScreenDisplayFontFamily { get; set; }
    float OnScreenDisplayFontSize { get; set; }
    Color OnScreenDisplayColor { get; set; }
    bool IsDisplayOnScreenDisplayBackground { get; set; }
    Color OnScreenDisplayBackgroundColor { get; set; }

    bool IsDisplayLogo { get; set; }
    Image LogoImage { get; set; }
    Point LogoPoint { get; set; }
    Size LogoSize { get; set; }

    bool IsDisplayProtectionMask { get; set; }
    Image ProtectionMaskImage { get; set; }
    Point ProtectionMaskPoint { get; set; }
    Size ProtectionMaskSize { get; set; }

    bool IsMotionDetection { get; set; }
    MotionDetector MotionDetector { get; set; }
  }
}
