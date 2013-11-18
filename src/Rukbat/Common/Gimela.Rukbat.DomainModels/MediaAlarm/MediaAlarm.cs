using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Rukbat.DomainModels
{
  public class MediaAlarm
  {
    public MediaAlarm()
    {
    }

    public MediaAlarm(string description)
      : this()
    {
      this.AlarmDescription = description;
    }

    public string AlarmDescription { get; set; }
  }
}
