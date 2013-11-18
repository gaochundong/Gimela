using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Media.Video;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public interface ICameraStream
  {
    IVideoSource Stream { get; }
  }
}
