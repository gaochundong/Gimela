using System;
using System.Drawing;

namespace Gimela.Rukbat.DomainModels
{
  public interface IFrame
  {
    bool IsLastFrameNull { get; }
    Bitmap LastFrame { get; }

    event EventHandler NewFrameEvent;
  }
}
