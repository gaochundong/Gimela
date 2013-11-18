using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Rukbat.DVC.BusinessEntities;
using System.Globalization;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  [Serializable]
  public class FilterNotFoundException : Exception
  {
    public FilterNotFoundException()
    {
    }

    public FilterNotFoundException(string message)
      : base(message)
    {
    }

    public FilterNotFoundException(string message, Exception innerException) :
      base(message, innerException)
    {
    }

    protected FilterNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
