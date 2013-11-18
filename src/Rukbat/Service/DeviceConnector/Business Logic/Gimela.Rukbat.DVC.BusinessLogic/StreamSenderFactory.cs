using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Rukbat.DVC.BusinessEntities;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  internal static class StreamSenderFactory
  {
    internal static IStreamSender GetSender()
    {
      return new UdpStreamSender();
    }
  }
}
