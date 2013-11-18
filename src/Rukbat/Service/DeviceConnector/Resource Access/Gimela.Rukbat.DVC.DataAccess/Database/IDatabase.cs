using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Data.Magpie;

namespace Gimela.Rukbat.DVC.DataAccess
{
  public interface IDatabase
  {
    MagpieDatabase Instance { get; }

    void Shutdown();
  }
}
