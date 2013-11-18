using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gimela.Rukbat.DPS.Contracts.DataContracts
{
  [DataContract]
  public class Camera
  {
    [DataMember(Order = 1)]
    public string Id { get; set; }

    [DataMember(Order = 2)]
    public string Name { get; set; }

    [DataMember(Order = 3)]
    public string Url { get; set; }

    [DataMember(Order = 4)]
    public int Port { get; set; }
  }
}
