using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gimela.Rukbat.DVC.BusinessEntities;

namespace Gimela.Rukbat.DVC.BusinessLogic
{
  public interface IDeviceController
  {
    void Start();
    void Stop();
  }
}
