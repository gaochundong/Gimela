using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Gimela.Rukbat.DVC.DataAccess
{
  public static class DatabaseSettings
  {
    internal const string DatabasePathKey = "DatabasePath";
    internal const string DatabaseNameKey = "DatabaseName";
    internal const string DefaultDatabaseName = @"DVC";

    public static string DatabasePath
    {
      get
      {
        string path = ConfigurationManager.AppSettings.Get(DatabaseSettings.DatabasePathKey);
        if (string.IsNullOrEmpty(path) || path == @".")
        {
          path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
        else if (path.StartsWith(@"./", StringComparison.OrdinalIgnoreCase) || path.StartsWith(@".\", StringComparison.OrdinalIgnoreCase))
        {
          path = path.TrimStart('.', '\\', '/');
          path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
        }

        return path;
      }
    }

    public static string DatabaseName
    {
      get
      {
        string name = ConfigurationManager.AppSettings.Get(DatabaseSettings.DatabaseNameKey);
        if (string.IsNullOrEmpty(name))
        {
          return DefaultDatabaseName;
        }
        return name;
      }
    }
  }
}
