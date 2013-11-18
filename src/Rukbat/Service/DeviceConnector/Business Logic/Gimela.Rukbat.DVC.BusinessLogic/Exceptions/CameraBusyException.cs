using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  [Serializable]
  public class CameraBusyException : Exception
  {
    public CameraBusyException()
    {
    }

    public CameraBusyException(string message)
      : base(message)
    {
    }

    public CameraBusyException(string message, Exception innerException) :
      base(message, innerException)
    {
    }

    protected CameraBusyException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
