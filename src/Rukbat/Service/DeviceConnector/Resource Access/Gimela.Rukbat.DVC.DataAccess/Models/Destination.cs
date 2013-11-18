using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gimela.Rukbat.DVC.DataAccess.Models
{
  [Serializable]
  public class Destination
  {
    public string Address { get; set; }

    public int Port { get; set; }
  }
}
