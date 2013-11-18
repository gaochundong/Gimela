using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using Gimela.Common.Configuration;

namespace Gimela.Rukbat.GUI.Cultures
{
  public static class CultureHelper
  {
    internal const string CultureKey = @"Culture";

    public static string Component
    {
      get
      {
        return Assembly.GetAssembly(typeof(CultureHelper)).GetName().Name;
      }
    }

    public static string Culture
    {
      get
      {
        return ConfigurationMaster.Get(CultureHelper.CultureKey);
      }
    }
  }
}
