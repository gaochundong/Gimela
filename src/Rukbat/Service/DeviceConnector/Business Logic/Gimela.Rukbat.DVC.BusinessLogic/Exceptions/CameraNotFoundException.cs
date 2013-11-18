using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  [Serializable]
  public class CameraNotFoundException : Exception
  {
    public CameraNotFoundException()
    {
    }

    public CameraNotFoundException(string message)
      : base(message)
    {
    }

    public CameraNotFoundException(string message, Exception innerException) :
      base(message, innerException)
    {
    }

    protected CameraNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
