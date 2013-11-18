using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.MPS.BusinessLogic
{
  [Serializable]
  public class PushCameraVideoErrorException : Exception
  {
    public PushCameraVideoErrorException()
    {
    }

    public PushCameraVideoErrorException(string message)
      : base(message)
    {
    }

    public PushCameraVideoErrorException(string message, Exception innerException) :
      base(message, innerException)
    {
    }

    protected PushCameraVideoErrorException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
