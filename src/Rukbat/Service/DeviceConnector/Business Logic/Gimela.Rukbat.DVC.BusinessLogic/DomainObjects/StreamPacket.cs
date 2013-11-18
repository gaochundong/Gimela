using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Gimela.Rukbat.DVC.BusinessEntities;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public class StreamPacket
  {
    public StreamPacket(Bitmap frame, int sequenceNumber, DateTime timestamp, PublishDestination destination)
    {
      Frame = frame;
      SequenceNumber = sequenceNumber;
      Timestamp = timestamp;
      Destination = destination;
    }

    public Bitmap Frame { get; private set; }
    public int SequenceNumber { get; private set; }
    public DateTime Timestamp { get; private set; }
    public PublishDestination Destination { get; private set; }
  }
}
