using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Gimela.Media.Video;
using Gimela.Rukbat.DVC.BusinessEntities;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public interface IStreamSender : IDisposable
  {
    bool IsRunning { get; }

    IStreamSender Start();

    IStreamSender Stop();

    IStreamSender Send(StreamPacket packet);
  }
}
